using System.Diagnostics;
using ILSpy.PE;
using ILSpy.IO;
using ILSpy.Enums;

namespace ILSpy.Meta
{
    sealed class ENCMetadata : MetadataBase
    {
        static readonly UTF8String DeletedName = "_Deleted";
        bool hasMethodPtr, hasFieldPtr, hasParamPtr, hasEventPtr, hasPropertyPtr;
        bool hasDeletedFields;
        bool hasDeletedNonFields;
        readonly CLRRuntimeReaderKind runtime;
        readonly Dictionary<TableType, SortedTable> sortedTables = [];

        /// <inheritdoc/>
        public override bool IsCompressed => false;

        /// <inheritdoc/>
        public ENCMetadata(IPEImage peImage, ImageCor20Header cor20Header, MetadataHeader mdHeader, CLRRuntimeReaderKind runtime)
            : base(peImage, cor20Header, mdHeader)
        {
            this.runtime = runtime;
        }

        /// <inheritdoc/>
        internal ENCMetadata(MetadataHeader mdHeader, bool isStandalonePortablePdb, CLRRuntimeReaderKind runtime)
            : base(mdHeader, isStandalonePortablePdb)
        {
            this.runtime = runtime;
        }

        /// <inheritdoc/>
        protected override void InitializeInternal(DataReaderFactory mdReaderFactory, uint metadataBaseOffset)
        {
            DotNetStream? dns = null;
            var newAllStreams = new List<DotNetStream>(allStreams ?? throw new InvalidOperationException());
            var streamHeaders = mdHeader?.StreamHeaders ?? throw new InvalidOperationException();
            bool forceAllBig = false;

            try
            {
                if (runtime == CLRRuntimeReaderKind.Mono)
                {
                    for (int i = streamHeaders.Count - 1; i >= 0; i--)
                    {
                        var sh = streamHeaders[i];
                        switch (sh.Name)
                        {
                            case "#Strings":
                                if (stringsStream is null)
                                {
                                    stringsStream = new StringsStream(mdReaderFactory, metadataBaseOffset, sh);
                                    newAllStreams.Add(stringsStream);
                                    continue;
                                }
                                break;

                            case "#US":
                                if (usStream is null)
                                {
                                    usStream = new USStream(mdReaderFactory, metadataBaseOffset, sh);
                                    newAllStreams.Add(usStream);
                                    continue;
                                }
                                break;

                            case "#Blob":
                                if (blobStream is null)
                                {
                                    blobStream = new BlobStream(mdReaderFactory, metadataBaseOffset, sh);
                                    newAllStreams.Add(blobStream);
                                    continue;
                                }
                                break;

                            case "#GUID":
                                if (guidStream is null)
                                {
                                    guidStream = new GuidStream(mdReaderFactory, metadataBaseOffset, sh);
                                    newAllStreams.Add(guidStream);
                                    continue;
                                }
                                break;

                            case "#~":
                            case "#-":
                                if (tablesStream is null)
                                {
                                    tablesStream = new TablesStream(mdReaderFactory, metadataBaseOffset, sh, runtime);
                                    newAllStreams.Add(tablesStream);
                                    continue;
                                }
                                break;

                            case "#Pdb":
                                if (isStandalonePortablePdb && pdbStream is null)
                                {
                                    pdbStream = new PdbStream(mdReaderFactory, metadataBaseOffset, sh);
                                    newAllStreams.Add(pdbStream);
                                    continue;
                                }
                                break;

                            case "#JTD":
                                forceAllBig = true;
                                continue;
                        }
                        dns = new CustomDotNetStream(mdReaderFactory, metadataBaseOffset, sh);
                        newAllStreams.Add(dns);
                        dns = null;
                    }
                    newAllStreams.Reverse();
                    allStreams = newAllStreams;
                }
                else
                {
                    Debug.Assert(runtime == CLRRuntimeReaderKind.CLR);
                    foreach (var sh in streamHeaders)
                    {
                        switch (sh.Name.ToUpperInvariant())
                        {
                            case "#STRINGS":
                                if (stringsStream is null)
                                {
                                    stringsStream = new StringsStream(mdReaderFactory, metadataBaseOffset, sh);
                                    allStreams.Add(stringsStream);
                                    continue;
                                }
                                break;

                            case "#US":
                                if (usStream is null)
                                {
                                    usStream = new USStream(mdReaderFactory, metadataBaseOffset, sh);
                                    allStreams.Add(usStream);
                                    continue;
                                }
                                break;

                            case "#BLOB":
                                if (blobStream is null)
                                {
                                    blobStream = new BlobStream(mdReaderFactory, metadataBaseOffset, sh);
                                    allStreams.Add(blobStream);
                                    continue;
                                }
                                break;

                            case "#GUID":
                                if (guidStream is null)
                                {
                                    guidStream = new GuidStream(mdReaderFactory, metadataBaseOffset, sh);
                                    allStreams.Add(guidStream);
                                    continue;
                                }
                                break;

                            case "#~":  // Only if #Schema is used
                            case "#-":
                                if (tablesStream is null)
                                {
                                    tablesStream = new TablesStream(mdReaderFactory, metadataBaseOffset, sh, runtime);
                                    allStreams.Add(tablesStream);
                                    continue;
                                }
                                break;

                            case "#PDB":
                                // Case sensitive comparison since it's a stream that's not read by the CLR,
                                // only by other libraries eg. System.Reflection.Metadata.
                                if (isStandalonePortablePdb && pdbStream is null && sh.Name == "#Pdb")
                                {
                                    pdbStream = new PdbStream(mdReaderFactory, metadataBaseOffset, sh);
                                    allStreams.Add(pdbStream);
                                    continue;
                                }
                                break;

                            case "#JTD":
                                forceAllBig = true;
                                continue;
                        }
                        dns = new CustomDotNetStream(mdReaderFactory, metadataBaseOffset, sh);
                        allStreams.Add(dns);
                        dns = null;
                    }
                }
            }
            finally
            {
                dns?.Dispose();
            }

            if (tablesStream is null)
                throw new BadImageFormatException("Missing MD stream");

            if (pdbStream is not null)
                tablesStream.Initialize(pdbStream.TypeSystemTableRows, forceAllBig);
            else
                tablesStream.Initialize(null, forceAllBig);

            // The pointer tables are used iff row count != 0
            hasFieldPtr = !tablesStream.FieldPtrTable.IsEmpty;
            hasMethodPtr = !tablesStream.MethodPtrTable.IsEmpty;
            hasParamPtr = !tablesStream.ParamPtrTable.IsEmpty;
            hasEventPtr = !tablesStream.EventPtrTable.IsEmpty;
            hasPropertyPtr = !tablesStream.PropertyPtrTable.IsEmpty;

            switch (runtime)
            {
                case CLRRuntimeReaderKind.CLR:
                    hasDeletedFields = tablesStream.HasDelete;
                    hasDeletedNonFields = tablesStream.HasDelete;
                    break;

                case CLRRuntimeReaderKind.Mono:
                    hasDeletedFields = true;
                    hasDeletedNonFields = false;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <inheritdoc/>
        public override RidList GetTypeDefRidList()
        {
            if (!hasDeletedNonFields)
            {
                return base.GetTypeDefRidList();
            }

            if (tablesStream is null)
            {
                throw new InvalidOperationException("TablesStream is null");
            }
            if (stringsStream is null)
            {
                throw new InvalidOperationException("StringsStream is null");
            }

            uint rows = tablesStream.TypeDefTable.Rows;
            var list = new List<uint>((int)rows);
            for (uint rid = 1; rid <= rows; rid++)
            {
                if (!tablesStream.TryReadTypeDefRow(rid, out var row))
                    continue;   // Should never happen since rid is valid

                // RTSpecialName is ignored by the CLR. It's only the name that indicates
                // whether it's been deleted.
                // It's not possible to delete the global type (<Module>)
                if (rid != 1 && stringsStream.ReadNoNull(row.Name).StartsWith(DeletedName))
                    continue;   // ignore this deleted row
                list.Add(rid);
            }
            return RidList.Create(list);
        }

        public override RidList GetExportedTypeRidList()
        {
            if (!hasDeletedNonFields)
            {
                return base.GetExportedTypeRidList();
            }


            if (tablesStream is null)
            {
                throw new InvalidOperationException("TablesStream is null");
            }
            if (stringsStream is null)
            {
                throw new InvalidOperationException("StringsStream is null");
            }

            uint rows = tablesStream.ExportedTypeTable.Rows;
            var list = new List<uint>((int)rows);
            for (uint rid = 1; rid <= rows; rid++)
            {
                if (!tablesStream.TryReadExportedTypeRow(rid, out var row))
                    continue;   // Should never happen since rid is valid

                // RTSpecialName is ignored by the CLR. It's only the name that indicates
                // whether it's been deleted.
                if (stringsStream.ReadNoNull(row.TypeName).StartsWith(DeletedName))
                    continue;   // ignore this deleted row
                list.Add(rid);
            }
            return RidList.Create(list);
        }

        uint ToFieldRid(uint listRid)
        {
            if (!hasFieldPtr)
            {
                return listRid;
            }
            if (tablesStream is null)
            {
                throw new InvalidOperationException("TablesStream is null");
            }

            return tablesStream.TryReadColumn24(tablesStream.FieldPtrTable, listRid, 0, out uint listValue) ? listValue : 0;
        }

        uint ToMethodRid(uint listRid)
        {
            if (!hasMethodPtr)
            {
                return listRid;
            }

            Debug.Assert(tablesStream != null);
            return tablesStream.TryReadColumn24(tablesStream.MethodPtrTable, listRid, 0, out uint listValue) ? listValue : 0;
        }

        uint ToParamRid(uint listRid)
        {
            if (!hasParamPtr)
            {
                return listRid;
            }

            Debug.Assert(tablesStream != null);
            return tablesStream.TryReadColumn24(tablesStream.ParamPtrTable, listRid, 0, out uint listValue) ? listValue : 0;
        }


        uint ToEventRid(uint listRid)
        {
            if (!hasEventPtr)
            {
                return listRid;
            }

            Debug.Assert(tablesStream != null);
            return tablesStream.TryReadColumn24(tablesStream.EventPtrTable, listRid, 0, out uint listValue) ? listValue : 0;
        }

        uint ToPropertyRid(uint listRid)
        {
            if (!hasPropertyPtr)
            {
                return listRid;
            }

            Debug.Assert(tablesStream != null);
            return tablesStream.TryReadColumn24(tablesStream.PropertyPtrTable, listRid, 0, out uint listValue) ? listValue : 0;
        }

        public override RidList GetFieldRidList(uint typeDefRid)
        {
            var list = GetRidList(tablesStream?.TypeDefTable, typeDefRid, 4, tablesStream?.FieldTable);
            if (list.Count == 0 || (!hasFieldPtr && !hasDeletedFields))
            {
                return list;
            }

            var destTable = tablesStream?.FieldTable;

            Debug.Assert(destTable is not null);
            Debug.Assert(tablesStream is not null);
            Debug.Assert(stringsStream is not null);

            var newList = new List<uint>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var rid = ToFieldRid(list[i]);
                if (destTable.IsInvalidRID(rid))
                    continue;
                if (hasDeletedFields)
                {
                    // It's a deleted row if RTSpecialName is set and name is "_Deleted"
                    if (!tablesStream.TryReadFieldRow(rid, out var row))
                        continue;   // Should never happen since rid is valid
                    if (runtime == CLRRuntimeReaderKind.CLR)
                    {
                        if ((row.Flags & (uint)FieldAttributes.RTSpecialName) != 0)
                        {
                            if (stringsStream.ReadNoNull(row.Name).StartsWith(DeletedName))
                                continue;   // ignore this deleted row
                        }
                    }
                    else
                    {
                        if ((row.Flags & (uint)(FieldAttributes.SpecialName | FieldAttributes.RTSpecialName)) == (uint)(FieldAttributes.SpecialName | FieldAttributes.RTSpecialName))
                        {
                            if (stringsStream.ReadNoNull(row.Name) == DeletedName)
                                continue;   // ignore this deleted row
                        }
                    }
                }
                // It's a valid non-deleted rid so add it
                newList.Add(rid);
            }
            return RidList.Create(newList);
        }

        /// <inheritdoc/>
        public override RidList GetMethodRidList(uint typeDefRid)
        {
            var list = GetRidList(tablesStream?.TypeDefTable, typeDefRid, 5, tablesStream?.MethodTable);
            if (list.Count == 0 || (!hasMethodPtr && !hasDeletedNonFields))
                return list;

            var destTable = tablesStream?.MethodTable;
            var newList = new List<uint>(list.Count);

            Debug.Assert(destTable is not null);
            Debug.Assert(tablesStream is not null);
            Debug.Assert(stringsStream is not null);

            for (int i = 0; i < list.Count; i++)
            {
                var rid = ToMethodRid(list[i]);
                if (destTable.IsInvalidRID(rid))
                    continue;
                if (hasDeletedNonFields)
                {
                    // It's a deleted row if RTSpecialName is set and name is "_Deleted"
                    if (!tablesStream.TryReadMethodRow(rid, out var row))
                        continue;   // Should never happen since rid is valid
                    if ((row.Flags & (uint)MethodAttributes.RTSpecialName) != 0)
                    {
                        if (stringsStream.ReadNoNull(row.Name).StartsWith(DeletedName))
                            continue;   // ignore this deleted row
                    }
                }
                // It's a valid non-deleted rid so add it
                newList.Add(rid);
            }
            return RidList.Create(newList);
        }

        public override RidList GetParamRidList(uint methodRid)
        {
            var list = GetRidList(tablesStream?.MethodTable, methodRid, 5, tablesStream?.ParamTable);
            if (list.Count == 0 || !hasParamPtr)
            {
                return list;
            }

            var destTable = tablesStream?.ParamTable;
            var newList = new List<uint>(list.Count);
            Debug.Assert(destTable is not null);
            Debug.Assert(tablesStream is not null);
            Debug.Assert(stringsStream is not null);

            for (int i = 0; i < list.Count; i++)
            {
                var rid = ToParamRid(list[i]);
                if (destTable.IsInvalidRID(rid))
                {
                    continue;
                }
                newList.Add(rid);
            }
            return RidList.Create(newList);
        }

        public override RidList GetEventRidList(uint eventMapRid)
        {
            var list = GetRidList(tablesStream?.EventMapTable, eventMapRid, 1, tablesStream?.EventTable);
            if (list.Count == 0 || (!hasEventPtr && !hasDeletedNonFields))
            {
                return list;
            }

            var destTable = tablesStream?.EventTable;
            var newList = new List<uint>(list.Count);
            Debug.Assert(destTable is not null);
            Debug.Assert(tablesStream is not null);
            Debug.Assert(stringsStream is not null);

            for (int i = 0; i < list.Count; i++)
            {
                var rid = ToEventRid(list[i]);
                if (destTable.IsInvalidRID(rid))
                    continue;
                if (hasDeletedNonFields)
                {
                    // It's a deleted row if RTSpecialName is set and name is "_Deleted"
                    if (!tablesStream.TryReadEventRow(rid, out var row))
                    {
                        continue;   // Should never happen since rid is valid
                    }
                    if ((row.EventFlags & (uint)EventAttributes.RTSpecialName) != 0)
                    {
                        if (stringsStream.ReadNoNull(row.Name).StartsWith(DeletedName))
                        {
                            continue;   // ignore this deleted row
                        }
                    }
                }
                // It's a valid non-deleted rid so add it
                newList.Add(rid);
            }
            return RidList.Create(newList);
        }

        /// <inheritdoc/>
        public override RidList GetPropertyRidList(uint propertyMapRid)
        {
            var list = GetRidList(tablesStream?.PropertyMapTable, propertyMapRid, 1, tablesStream?.PropertyTable);
            if (list.Count == 0 || (!hasPropertyPtr && !hasDeletedNonFields))
            {
                return list;
            }

            var destTable = tablesStream?.PropertyTable;
            var newList = new List<uint>(list.Count);
            Debug.Assert(destTable is not null);
            Debug.Assert(tablesStream is not null);
            Debug.Assert(stringsStream is not null);

            for (int i = 0; i < list.Count; i++)
            {
                var rid = ToPropertyRid(list[i]);
                if (destTable.IsInvalidRID(rid))
                {
                    continue;
                }

                if (hasDeletedNonFields)
                {
                    // It's a deleted row if RTSpecialName is set and name is "_Deleted"
                    if (!tablesStream.TryReadPropertyRow(rid, out var row))
                    {
                        continue;   // Should never happen since rid is valid
                    }
                    if ((row.PropFlags & (uint)PropertyAttributes.RTSpecialName) != 0)
                    {
                        if (stringsStream.ReadNoNull(row.Name).StartsWith(DeletedName))
                        {
                            continue;   // ignore this deleted row
                        }
                    }
                }
                // It's a valid non-deleted rid so add it
                newList.Add(rid);
            }
            return RidList.Create(newList);
        }

        /// <inheritdoc/>
        public override RidList GetLocalVariableRidList(uint localScopeRid) => GetRidList(tablesStream?.LocalScopeTable, localScopeRid, 2, tablesStream?.LocalVariableTable);

        /// <inheritdoc/>
        public override RidList GetLocalConstantRidList(uint localScopeRid) => GetRidList(tablesStream?.LocalScopeTable, localScopeRid, 3, tablesStream?.LocalConstantTable);

        RidList GetRidList(Table? tableSource, uint tableSourceRid, int colIndex, Table? tableDest)
        {
            Debug.Assert(tableSource != null);
            Debug.Assert(tableDest != null);
            Debug.Assert(tablesStream != null);

            var column = tableSource.TableInfo?.Columns[colIndex];
            if (!tablesStream.TryReadColumn24(tableSource, tableSourceRid, column, out uint startRid))
            {
                return RidList.Empty;
            }
            bool hasNext = tablesStream.TryReadColumn24(tableSource, tableSourceRid + 1, column, out uint nextListRid);
            uint lastRid = tableDest.Rows + 1;
            if (startRid == 0 || startRid >= lastRid)
            {
                return RidList.Empty;
            }
            uint endRid = hasNext && nextListRid != 0 ? nextListRid : lastRid;
            if (endRid < startRid)
            {
                endRid = startRid;
            }
            if (endRid > lastRid)
            {
                endRid = lastRid;
            }
            return RidList.Create(startRid, endRid - startRid);
        }

        /// <inheritdoc/>
        protected override uint BinarySearch(Table? tableSource, int keyColIndex, uint key)
        {
            Debug.Assert(tableSource != null);
            Debug.Assert(tablesStream != null);

            var keyColumn = tableSource.TableInfo?.Columns[keyColIndex];
            uint ridLo = 1, ridHi = tableSource.Rows;
            while (ridLo <= ridHi)
            {
                uint rid = (ridLo + ridHi) / 2;
                if (!tablesStream.TryReadColumn24(tableSource, rid, keyColumn, out uint key2))
                {
                    break;  // Never happens since rid is valid
                }
                if (key == key2)
                {
                    return rid;
                }
                if (key2 > key)
                {
                    ridHi = rid - 1;
                }
                else
                {
                    ridLo = rid + 1;
                }
            }

            if (tableSource.TableType == TableType.GenericParam && !tablesStream.IsSorted(tableSource))
                return LinearSearch(tableSource, keyColIndex, key);

            return 0;
        }

        uint LinearSearch(Table? tableSource, int keyColIndex, uint key)
        {
            Debug.Assert(tablesStream != null);

            if (tableSource is null)
            {
                return 0;
            }

            var keyColumn = tableSource.TableInfo?.Columns[keyColIndex];
            for (uint rid = 1; rid <= tableSource.Rows; rid++)
            {
                if (!tablesStream.TryReadColumn24(tableSource, rid, keyColumn, out uint key2))
                {
                    break;  // Never happens since rid is valid
                }
                if (key == key2)
                {
                    return rid;
                }
            }
            return 0;
        }

        /// <inheritdoc/>
        protected override RidList FindAllRowsUnsorted(Table? tableSource, int keyColIndex, uint key)
        {
            Debug.Assert(tablesStream != null);
            Debug.Assert(tableSource != null);

            if (tablesStream.IsSorted(tableSource))
                return FindAllRows(tableSource, keyColIndex, key);
            if (!sortedTables.TryGetValue(tableSource.TableType, out SortedTable? sortedTable))
            {
                sortedTables[tableSource.TableType] = sortedTable = new SortedTable(tableSource, keyColIndex);
            }

            return sortedTable.FindAllRows(key);
        }
    }
}