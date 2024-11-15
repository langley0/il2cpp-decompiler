using ILSpy.Enums;
using ILSpy.Interfaces;
using ILSpy.Pdb;

namespace ILSpy.DotNet
{
    public sealed class Instruction
    {
        public OpCode OpCode;

        public object? Operand;

        public uint Offset;

        public SequencePoint? SequencePoint;

        public Instruction()
        : this(OpCodes.Nop)
        {
        }

        public Instruction(OpCode opCode)
            : this(opCode, null)
        {
        }


        public Instruction(OpCode opCode, object? operand)
        {
            OpCode = opCode;
            Operand = operand;
            SequencePoint = null;
        }

        public static Instruction Create(OpCode opCode)
        {
            if (opCode.OperandType != OperandType.InlineNone)
            {
                throw new ArgumentException("Must be a no-operand opcode", nameof(opCode));
            }
            return new Instruction(opCode);
        }

        public static Instruction Create(OpCode opCode, byte value)
        {
            if (opCode.Code != Code.Unaligned)
            {
                throw new ArgumentException("Opcode does not have a byte operand", nameof(opCode));
            }
            return new Instruction(opCode, value);
        }

        public static Instruction Create(OpCode opCode, sbyte value)
        {
            if (opCode.Code != Code.Ldc_I4_S)
            {
                throw new ArgumentException("Opcode does not have a sbyte operand", nameof(opCode));
            }
            return new Instruction(opCode, value);
        }

        public static Instruction Create(OpCode opCode, int value)
        {
            if (opCode.OperandType != OperandType.InlineI)
            {
                throw new ArgumentException("Opcode does not have an int32 operand", nameof(opCode));
            }
            return new Instruction(opCode, value);
        }

        public static Instruction Create(OpCode opCode, long value)
        {
            if (opCode.OperandType != OperandType.InlineI8)
            {
                throw new ArgumentException("Opcode does not have an int64 operand", nameof(opCode));
            }
            return new Instruction(opCode, value);
        }

        public static Instruction Create(OpCode opCode, float value)
        {
            if (opCode.OperandType != OperandType.ShortInlineR)
            {
                throw new ArgumentException("Opcode does not have a real4 operand", nameof(opCode));
            }
            return new Instruction(opCode, value);
        }

        public static Instruction Create(OpCode opCode, double value)
        {
            if (opCode.OperandType != OperandType.InlineR)
            {
                throw new ArgumentException("Opcode does not have a real8 operand", nameof(opCode));
            }
            return new Instruction(opCode, value);
        }

        public static Instruction Create(OpCode opCode, string s)
        {
            if (opCode.OperandType != OperandType.InlineString)
            {
                throw new ArgumentException("Opcode does not have a string operand", nameof(opCode));
            }
            return new Instruction(opCode, s);
        }

        public static Instruction Create(OpCode opCode, Instruction target)
        {
            if (opCode.OperandType != OperandType.ShortInlineBrTarget && opCode.OperandType != OperandType.InlineBrTarget)
            {
                throw new ArgumentException("Opcode does not have an instruction operand", nameof(opCode));
            }
            return new Instruction(opCode, target);
        }

        public static Instruction Create(OpCode opCode, IList<Instruction> targets)
        {
            if (opCode.OperandType != OperandType.InlineSwitch)
            {
                throw new ArgumentException("Opcode does not have a targets array operand", nameof(opCode));
            }
            return new Instruction(opCode, targets);
        }

        public static Instruction Create(OpCode opCode, ITypeDefOrRef type)
        {
            if (opCode.OperandType != OperandType.InlineType && opCode.OperandType != OperandType.InlineTok)
            {
                throw new ArgumentException("Opcode does not have a type operand", nameof(opCode));
            }
            return new Instruction(opCode, type);
        }

        public static Instruction Create(OpCode opCode, ICorLibTypeSig type) => Create(opCode, type.TypeDefOrRef);

        public static Instruction Create(OpCode opCode, IMemberRef mr)
        {
            if (opCode.OperandType != OperandType.InlineField && opCode.OperandType != OperandType.InlineMethod && opCode.OperandType != OperandType.InlineTok)
            {
                throw new ArgumentException("Opcode does not have a field operand", nameof(opCode));
            }
            return new Instruction(opCode, mr);
        }

        public static Instruction Create(OpCode opCode, IField field)
        {
            if (opCode.OperandType != OperandType.InlineField && opCode.OperandType != OperandType.InlineTok)
            {
                throw new ArgumentException("Opcode does not have a field operand", nameof(opCode));
            }
            return new Instruction(opCode, field);
        }

        public static Instruction Create(OpCode opCode, IMethod method)
        {
            if (opCode.OperandType != OperandType.InlineMethod && opCode.OperandType != OperandType.InlineTok)
            {
                throw new ArgumentException("Opcode does not have a method operand", nameof(opCode));
            }
            return new Instruction(opCode, method);
        }

        public static Instruction Create(OpCode opCode, ITokenOperand token)
        {
            if (opCode.OperandType != OperandType.InlineTok)
            {
                throw new ArgumentException("Opcode does not have a token operand", nameof(opCode));
            }
            return new Instruction(opCode, token);
        }

        public static Instruction Create(OpCode opCode, IMethodSig methodSig)
        {
            if (opCode.OperandType != OperandType.InlineSig)
            {
                throw new ArgumentException("Opcode does not have a method sig operand", nameof(opCode));
            }
            return new Instruction(opCode, methodSig);
        }

        public static Instruction Create(OpCode opCode, IParameter parameter)
        {
            if (opCode.OperandType != OperandType.ShortInlineVar && opCode.OperandType != OperandType.InlineVar)
            {
                throw new ArgumentException("Opcode does not have a method parameter operand", nameof(opCode));
            }

            return new Instruction(opCode, parameter);
        }

        public static Instruction Create(OpCode opCode, ILocal local)
        {
            if (opCode.OperandType != OperandType.ShortInlineVar && opCode.OperandType != OperandType.InlineVar)
            {
                throw new ArgumentException("Opcode does not have a method local operand", nameof(opCode));
            }
            return new Instruction(opCode, local);
        }

        public static Instruction CreateLdcI4(int value)
        {
            switch (value)
            {
                case -1: return OpCodes.Ldc_I4_M1.ToInstruction();
                case 0: return OpCodes.Ldc_I4_0.ToInstruction();
                case 1: return OpCodes.Ldc_I4_1.ToInstruction();
                case 2: return OpCodes.Ldc_I4_2.ToInstruction();
                case 3: return OpCodes.Ldc_I4_3.ToInstruction();
                case 4: return OpCodes.Ldc_I4_4.ToInstruction();
                case 5: return OpCodes.Ldc_I4_5.ToInstruction();
                case 6: return OpCodes.Ldc_I4_6.ToInstruction();
                case 7: return OpCodes.Ldc_I4_7.ToInstruction();
                case 8: return OpCodes.Ldc_I4_8.ToInstruction();
            }
            if (sbyte.MinValue <= value && value <= sbyte.MaxValue)
            {
                return new Instruction(OpCodes.Ldc_I4_S, (sbyte)value);
            }
            return new Instruction(OpCodes.Ldc_I4, value);
        }

        public int GetSize()
        {
            var opCode = OpCode;
            switch (opCode.OperandType)
            {
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    return opCode.Size + 4;

                case OperandType.InlineI8:
                case OperandType.InlineR:
                    return opCode.Size + 8;

                case OperandType.InlineNone:
                case OperandType.InlinePhi:
                default:
                    return opCode.Size;

                case OperandType.InlineSwitch:
                    var targets = Operand as IList<Instruction>;
                    return opCode.Size + 4 + (targets is null ? 0 : targets.Count * 4);

                case OperandType.InlineVar:
                    return opCode.Size + 2;

                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    return opCode.Size + 1;
            }
        }

        static bool IsSystemVoid(ITypeSig type) => type.RemovePinnedAndModifiers().GetElementType() == ElementType.Void;

        public void UpdateStack(ref int stack) => UpdateStack(ref stack, false);

        public void UpdateStack(ref int stack, bool methodHasReturnValue)
        {
            CalculateStackUsage(methodHasReturnValue, out int pushes, out int pops);
            if (pops == -1)
                stack = 0;
            else
                stack += pushes - pops;
        }

        public void CalculateStackUsage(out int pushes, out int pops) => CalculateStackUsage(false, out pushes, out pops);

        public void CalculateStackUsage(bool methodHasReturnValue, out int pushes, out int pops)
        {
            var opCode = OpCode;
            if (opCode.FlowControl == FlowControl.Call)
                CalculateStackUsageCall(opCode.Code, out pushes, out pops);
            else
                CalculateStackUsageNonCall(opCode, methodHasReturnValue, out pushes, out pops);
        }

        void CalculateStackUsageCall(Code code, out int pushes, out int pops)
        {
            pushes = 0;
            pops = 0;

            // It doesn't push or pop anything. The stack should be empty when JMP is executed.
            if (code == Code.Jmp)
                return;

            IMethodSig? sig;
            var op = Operand;
            if (op is IMethod method)
            {
                sig = method.MethodSig;
            }
            else
            {
                sig = op as IMethodSig;  // calli instruction
            }

            if (sig is null)
            {
                return;
            }

            bool implicitThis = sig.ImplicitThis;
            if (!IsSystemVoid(sig.RetType) || (code == Code.Newobj && sig.HasThis))
                pushes++;

            pops += sig.Params.Count;
            var paramsAfterSentinel = sig.ParamsAfterSentinel;
            if (paramsAfterSentinel is not null)
                pops += paramsAfterSentinel.Count;
            if (implicitThis && code != Code.Newobj)
                pops++;
            if (code == Code.Calli)
                pops++;
        }

        void CalculateStackUsageNonCall(OpCode opCode, bool hasReturnValue, out int pushes, out int pops)
        {
            pushes = opCode.StackBehaviourPush switch
            {
                StackBehaviour.Push0 => 0,
                StackBehaviour.Push1 or StackBehaviour.Pushi or StackBehaviour.Pushi8 or StackBehaviour.Pushr4 or StackBehaviour.Pushr8 or StackBehaviour.Pushref => 1,
                StackBehaviour.Push1_push1 => 2,
                // only call, calli, callvirt which are handled elsewhere
                _ => 0,
            };
            switch (opCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    pops = 0;
                    break;

                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    pops = 1;
                    break;

                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    pops = 2;
                    break;

                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                case StackBehaviour.Popref_popi_pop1:
                    pops = 3;
                    break;

                case StackBehaviour.PopAll:
                    pops = -1;
                    break;

                case StackBehaviour.Varpop: // call, calli, callvirt, newobj (all handled elsewhere), and ret
                    if (hasReturnValue)
                        pops = 1;
                    else
                        pops = 0;
                    break;

                default:
                    pops = 0;
                    break;
            }
        }

        public bool IsLeave() => OpCode == OpCodes.Leave || OpCode == OpCodes.Leave_S;

        public bool IsBr() => OpCode == OpCodes.Br || OpCode == OpCodes.Br_S;

        public bool IsBrfalse() => OpCode == OpCodes.Brfalse || OpCode == OpCodes.Brfalse_S;

        public bool IsBrtrue() => OpCode == OpCodes.Brtrue || OpCode == OpCodes.Brtrue_S;

        public bool IsConditionalBranch()
        {
            return OpCode.Code switch
            {
                Code.Bge or Code.Bge_S or Code.Bge_Un or Code.Bge_Un_S or Code.Blt or Code.Blt_S or Code.Blt_Un or Code.Blt_Un_S or Code.Bgt or Code.Bgt_S or Code.Bgt_Un or Code.Bgt_Un_S or Code.Ble or Code.Ble_S or Code.Ble_Un or Code.Ble_Un_S or Code.Brfalse or Code.Brfalse_S or Code.Brtrue or Code.Brtrue_S or Code.Beq or Code.Beq_S or Code.Bne_Un or Code.Bne_Un_S => true,
                _ => false,
            };
        }

        public bool IsLdcI4()
        {
            return OpCode.Code switch
            {
                Code.Ldc_I4_M1 or Code.Ldc_I4_0 or Code.Ldc_I4_1 or Code.Ldc_I4_2 or Code.Ldc_I4_3 or Code.Ldc_I4_4 or Code.Ldc_I4_5 or Code.Ldc_I4_6 or Code.Ldc_I4_7 or Code.Ldc_I4_8 or Code.Ldc_I4_S or Code.Ldc_I4 => true,
                _ => false,
            };
        }

        public int GetLdcI4Value() =>
            OpCode.Code switch
            {
                Code.Ldc_I4_M1 => -1,
                Code.Ldc_I4_0 => 0,
                Code.Ldc_I4_1 => 1,
                Code.Ldc_I4_2 => 2,
                Code.Ldc_I4_3 => 3,
                Code.Ldc_I4_4 => 4,
                Code.Ldc_I4_5 => 5,
                Code.Ldc_I4_6 => 6,
                Code.Ldc_I4_7 => 7,
                Code.Ldc_I4_8 => 8,
                Code.Ldc_I4_S => (sbyte)(Operand ?? throw new InvalidOperationException($"No operand: {this}")),
                Code.Ldc_I4 => (int)(Operand ?? throw new InvalidOperationException($"No operand: {this}")),
                _ => throw new InvalidOperationException($"Not a ldc.i4 instruction: {this}"),
            };

        public bool IsLdarg()
        {
            return OpCode.Code switch
            {
                Code.Ldarg or Code.Ldarg_S or Code.Ldarg_0 or Code.Ldarg_1 or Code.Ldarg_2 or Code.Ldarg_3 => true,
                _ => false,
            };
        }

        public bool IsLdloc()
        {
            return OpCode.Code switch
            {
                Code.Ldloc or Code.Ldloc_0 or Code.Ldloc_1 or Code.Ldloc_2 or Code.Ldloc_3 or Code.Ldloc_S => true,
                _ => false,
            };
        }

        public bool IsStarg()
        {
            return OpCode.Code switch
            {
                Code.Starg or Code.Starg_S => true,
                _ => false,
            };
        }


        public bool IsStloc()
        {
            return OpCode.Code switch
            {
                Code.Stloc or Code.Stloc_0 or Code.Stloc_1 or Code.Stloc_2 or Code.Stloc_3 or Code.Stloc_S => true,
                _ => false,
            };
        }

        public ILocal? GetLocal(IList<ILocal> locals)
        {
            int index;
            var code = OpCode.Code;
            switch (code)
            {
                case Code.Ldloc:
                case Code.Ldloc_S:
                case Code.Stloc:
                case Code.Stloc_S:
                case Code.Ldloca:
                case Code.Ldloca_S:
                    return Operand as ILocal;

                case Code.Ldloc_0:
                case Code.Ldloc_1:
                case Code.Ldloc_2:
                case Code.Ldloc_3:
                    index = code - Code.Ldloc_0;
                    break;

                case Code.Stloc_0:
                case Code.Stloc_1:
                case Code.Stloc_2:
                case Code.Stloc_3:
                    index = code - Code.Stloc_0;
                    break;

                default:
                    return null;
            }

            if ((uint)index < (uint)locals.Count)
                return locals[index];
            return null;
        }

        public int GetParameterIndex()
        {
            switch (OpCode.Code)
            {
                case Code.Ldarg_0: return 0;
                case Code.Ldarg_1: return 1;
                case Code.Ldarg_2: return 2;
                case Code.Ldarg_3: return 3;

                case Code.Starg:
                case Code.Starg_S:
                case Code.Ldarga:
                case Code.Ldarga_S:
                case Code.Ldarg:
                case Code.Ldarg_S:
                    var parameter = Operand as IParameter;
                    if (parameter is not null)
                    {
                        return parameter.Index;
                    }
                    break;
            }

            return -1;
        }

        public IParameter? GetParameter(IList<IParameter> parameters)
        {
            int i = GetParameterIndex();
            if ((uint)i < (uint)parameters.Count)
            {
                return parameters[i];
            }
            return null;
        }

        public ITypeSig? GetArgumentType(IMethodSig? methodSig, ITypeDefOrRef? declaringType)
        {
            // if (methodSig is null)
            // {
            //     return null;
            // }
            // int index = GetParameterIndex();
            // if (index == 0 && methodSig.ImplicitThis)
            // {
            //     if (declaringType is null)
            //     {
            //         return null;
            //     }
            //     ITypeSig declSig;
            //     bool isValueType;
            //     if (declaringType is ITypeSpec spec)
            //     {
            //         declSig = spec.TypeSig;
            //         isValueType = declSig.IsValueType;
            //     }
            //     else
            //     {
            //         // Consistent with ParameterList.UpdateThisParameterType
            //         var td = declaringType.ResolveTypeDef();
            //         if (td is null)
            //         {
            //             return declaringType.ToTypeSig();
            //         }
            //         isValueType = td.IsValueType;
            //         ClassOrValueTypeSig cvSig = isValueType ? new ValueTypeSig(td) : new ClassSig(td);
            //         if (td.HasGenericParameters)
            //         {
            //             int gpCount = td.GenericParameters.Count;
            //             var genArgs = new List<ITypeSig>(gpCount);
            //             for (int i = 0; i < gpCount; i++)
            //                 genArgs.Add(new GenericVar(i, td));
            //             declSig = new GenericInstSig(cvSig, genArgs);
            //         }
            //         else
            //             declSig = cvSig;
            //     }
            //     return isValueType ? new ByRefSig(declSig) : declSig;
            // }
            // if (methodSig.ImplicitThis)
            //     index--;
            // if ((uint)index < (uint)methodSig.Params.Count)
            //     return methodSig.Params[index];
            // return null;
            throw new NotImplementedException();
        }


        public Instruction Clone()
        {
            return new()
            {
                Offset = Offset,
                OpCode = OpCode,
                Operand = Operand,
                SequencePoint = SequencePoint,
            };
        }


        // public override string ToString() => InstructionPrinter.ToString(this);
    }

    // static partial class Extensions
    // {
    //     /// <summary>
    //     /// Gets the opcode or <see cref="OpCodes.UNKNOWN1"/> if <paramref name="self"/> is <c>null</c>
    //     /// </summary>
    //     /// <param name="self">this</param>
    //     /// <returns></returns>
    //     public static OpCode GetOpCode(this Instruction self) => self?.OpCode ?? OpCodes.UNKNOWN1;

    //     /// <summary>
    //     /// Gets the operand or <c>null</c> if <paramref name="self"/> is <c>null</c>
    //     /// </summary>
    //     /// <param name="self">this</param>
    //     /// <returns></returns>
    //     public static object GetOperand(this Instruction self) => self?.Operand;

    //     /// <summary>
    //     /// Gets the offset or 0 if <paramref name="self"/> is <c>null</c>
    //     /// </summary>
    //     /// <param name="self">this</param>
    //     /// <returns></returns>
    //     public static uint GetOffset(this Instruction self) => self?.Offset ?? 0;

    //     /// <summary>
    //     /// Gets the sequence point or <c>null</c> if <paramref name="self"/> is <c>null</c>
    //     /// </summary>
    //     /// <param name="self">this</param>
    //     /// <returns></returns>
    //     public static SequencePoint GetSequencePoint(this Instruction self) => self?.SequencePoint;
    // }
}