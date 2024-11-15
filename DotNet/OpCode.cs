using ILSpy.Enums;
using ILSpy.Interfaces;

namespace ILSpy.DotNet
{
    public sealed class OpCode
    {
        public readonly string Name;

        public readonly Code Code;

        public readonly OperandType OperandType;

        public readonly FlowControl FlowControl;

        public readonly OpCodeType OpCodeType;

        public readonly StackBehaviour StackBehaviourPush;

        public readonly StackBehaviour StackBehaviourPop;

        public short Value => (short)Code;

        public int Size => Code < (Code)0x100 || Code == Code.UNKNOWN1 ? 1 : 2;

        public OpCode(string name, byte first, byte second, OperandType operandType, FlowControl flowControl, StackBehaviour push, StackBehaviour pop)
            : this(name, (Code)((first << 8) | second), operandType, flowControl, OpCodeType.Experimental, push, pop, true)
        {
        }

        internal OpCode(string name, Code code, OperandType operandType, FlowControl flowControl, OpCodeType opCodeType, StackBehaviour push, StackBehaviour pop, bool experimental = false)
        {
            Name = name;
            Code = code;
            OperandType = operandType;
            FlowControl = flowControl;
            OpCodeType = opCodeType;
            StackBehaviourPush = push;
            StackBehaviourPop = pop;
            if (!experimental)
            {
                if (((ushort)code >> 8) == 0)
                    OpCodes.OneByteOpCodes[(byte)code] = this;
                else if (((ushort)code >> 8) == 0xFE)
                    OpCodes.TwoByteOpCodes[(byte)code] = this;
            }
        }

        public Instruction ToInstruction() => Instruction.Create(this);

        public Instruction ToInstruction(byte value) => Instruction.Create(this, value);

        public Instruction ToInstruction(sbyte value) => Instruction.Create(this, value);

        public Instruction ToInstruction(int value) => Instruction.Create(this, value);

        public Instruction ToInstruction(long value) => Instruction.Create(this, value);

        public Instruction ToInstruction(float value) => Instruction.Create(this, value);

        public Instruction ToInstruction(double value) => Instruction.Create(this, value);

        public Instruction ToInstruction(string s) => Instruction.Create(this, s);

        public Instruction ToInstruction(Instruction target) => Instruction.Create(this, target);

        public Instruction ToInstruction(IList<Instruction> targets) => Instruction.Create(this, targets);

        public Instruction ToInstruction(ITypeDefOrRef type) => Instruction.Create(this, type);

        public Instruction ToInstruction(ICorLibTypeSig type) => Instruction.Create(this, type.TypeDefOrRef);

        public Instruction ToInstruction(IMemberRef mr) => Instruction.Create(this, mr);

        public Instruction ToInstruction(IField field) => Instruction.Create(this, field);

        public Instruction ToInstruction(IMethod method) => Instruction.Create(this, method);

        public Instruction ToInstruction(ITokenOperand token) => Instruction.Create(this, token);

        public Instruction ToInstruction(IMethodSig methodSig) => Instruction.Create(this, methodSig);

        public Instruction ToInstruction(IParameter parameter) => Instruction.Create(this, parameter);

        public Instruction ToInstruction(ILocal local) => Instruction.Create(this, local);

        /// <inheritdoc/>
        public override string ToString() => Name;
    }
}