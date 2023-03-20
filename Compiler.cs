﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    enum Opcode
    {
        PUSH = 0x10,
        POP = 0x11,
        ADD = 0x20,
        SUB = 0x21,
        READ = 0x30,
        WRITE = 0x31,
        STORE = 0x40,
        LOAD = 0x41,
        CMP = 0x50,
        JZ = 0x60,
        JNZ = 0x61,
        JMP = 0x60,
        SWP = 0x70,
        DUP = 0x71,
        HLT = 0xff
    }
    internal class Compiler
    {
    }
}
