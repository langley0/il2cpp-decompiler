namespace ILSpy.Enums
{
	public enum SecurityAction : short
	{
		ActionMask = 0x001F,
		ActionNil = 0x0000,
		Request = 0x0001,
		Demand = 0x0002,
		Assert = 0x0003,
		Deny = 0x0004,
		PermitOnly = 0x0005,
		LinktimeCheck = 0x0006,
		LinkDemand = LinktimeCheck,
		InheritanceCheck = 0x0007,
		InheritDemand = InheritanceCheck,
		RequestMinimum = 0x0008,
		RequestOptional = 0x0009,
		RequestRefuse = 0x000A,
		PrejitGrant = 0x000B,
		PreJitGrant = PrejitGrant,
		PrejitDenied = 0x000C,
		PreJitDeny = PrejitDenied,
		NonCasDemand = 0x000D,
		NonCasLinkDemand = 0x000E,
		NonCasInheritance = 0x000F,

		MaximumValue = 0x000F,
	}
}