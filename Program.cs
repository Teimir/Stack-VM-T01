using System;
using Test;

class VirtualMachine
{
    private int[] memory;
    private int pc;
    private int sp;
    private bool zeroFlag;
    private bool negativeFlag;
    DiscController controller;
    private int[] stack;


    public VirtualMachine(int memSize, int stackSize, string diskname)
    {
        controller = new DiscController(diskname);
        memory = new int[memSize];
        stack = new int[stackSize];
        sp = -1;
        pc = 0;
    }

    public void LoadProgram(int[] program)
    {
        Array.Copy(program, memory, program.Length);
    }

    public void Run()
    {
        bool isRunning = true;
        while (isRunning)
        {
            int opcode = memory[pc];
            try
            {
                switch (opcode)
                {
                    case 0x10: // Push
                        int value = memory[Pop()];
                        Push(value);
                        pc += 1;
                        break;
                    case 0x11: // Pop
                        int val = Pop();
                        memory[Pop()] = val;
                        pc += 1;
                        break;
                    case 0x12: // Push next cell
                        int value2 = memory[pc + 1];
                        Push(value2);
                        pc += 2;
                        break;
                    case 0x20: // Add
                        int a = Pop();
                        int b = Pop();
                        Push(a + b);
                        pc += 1;
                        break;
                    case 0x21: // Substract
                        a = Pop();
                        b = Pop();
                        Push(a - b);
                        pc += 1;
                        break;
                    case 0x30: // read symbol
                        Push(GetChar());
                        pc += 1;
                        break;
                    case 0x31: // write symbol
                        PrintAsciiChar();
                        pc += 1;
                        break;
                    case 0x40: // write to disk
                        byte dw = ((byte)Pop());
                        controller.WriteToDisk(Pop(), Pop(), dw);
                        pc += 1;
                        break;
                    case 0x41: // read from disc
                        Push(controller.ReadFromDisk(Pop(), Pop()));
                        pc += 1;
                        break;
                    case 0x50: // Compare
                        a = Pop();
                        b = Pop();
                        int result = b - a;
                        if (result == 0)
                        {
                            zeroFlag = true;
                            negativeFlag = false;
                        }
                        else if (result < 0)
                        {
                            zeroFlag = false;
                            negativeFlag = true;
                        }
                        else
                        {
                            zeroFlag = false;
                            negativeFlag = false;
                        }
                        pc += 1;
                        break;
                    case 0x60: // Jump if zero
                        if (zeroFlag == true)
                        {
                            pc = memory[Pop()];
                        }
                        else
                        {
                            pc += 1;
                        }
                        break;
                    case 0x61: // Jump if less
                        if (negativeFlag == true)
                        {
                            pc = memory[Pop()];
                        }
                        else
                        {
                            pc += 1;
                        }
                        break;
                    case 0x62: //jump
                        pc = memory[Pop()];
                        break;
                    case 0x70: //Swap
                        Swap();
                        pc += 1;
                        break;
                    case 0x71: //Dup
                        Dup();
                        pc += 1;
                        break;
                    case 0xff: // Stop instruction
                        isRunning = false;
                        break;
                    default:
                        throw new Exception("Invalid opcode, В программе ошибка");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }


    // Выполнение команд в реальном времени
    /*
    public void Exec()
    {
        bool isRunning = true;
        while (isRunning)
        {
            string[] c = Console.ReadLine().Split(' ');
            int c1 = int.Parse(c[0]);
            int c2 = int.Parse(c[1]);
            int c3 = int.Parse(c[2]);
            try
            {
                switch (c1)
                {
                    case 0x10: // Push
                        int value = c2;
                        Push(value);
                        break;
                    case 0x11: // Push acc
                        Push(acc);
                        break;
                    case 0x20: // Pop
                        int val = Pop();
                        memory[c2] = val;
                        break;
                    case 0x21: // Pop
                        int val1 = Pop();
                        acc = val1;
                        break;
                    case 0x30: // Add
                        int a = Pop();
                        int b = Pop();
                        acc = a + b;
                        break;
                    case 0x31: // Substract
                        a = Pop();
                        b = Pop();
                        acc = a - b;
                        break;
                    case 0x40: // read symbol
                        Push(GetChar());
                        break;
                    case 0x42:
                        PrintAsciiChar();
                        break;
                    case 0x50: // Load accumulator
                        acc = memory[c2];
                        break;
                    case 0x60:
                        byte dw = ((byte)Pop());
                        controller.WriteToDisk(Pop(), Pop(), dw);
                        break;
                    case 0x61:
                         Push(controller.ReadFromDisk(Pop(),Pop()));
                        break;
                    case 0x70: // Compare
                        a = Pop();
                        b = Pop();
                        int result = b - a;
                        if (result == 0)
                        {
                            zeroFlag = true;
                            negativeFlag = false;
                        }
                        else if (result < 0)
                        {
                            zeroFlag = false;
                            negativeFlag = true;
                        }
                        else
                        {
                            zeroFlag = false;
                            negativeFlag = false;
                        }
                        break;
                    case 0xff: // Stop instruction
                        isRunning = false;
                        break;
                    default:
                        throw new Exception("Invalid opcode, Неправильная команда");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
    */



    private void Push(int value)
    {
        if (sp < stack.Length)
        {
            sp++;
            stack[sp] = value;
        }
        else throw new Exception("Stack overflow, Стек переполнен");
    }

    private int Pop()
    {
        if (sp == -1)
            throw new Exception("Stack is empty, Стек пуст");
        int value = stack[sp];
        sp--;
        return value;
    }

    public int GetChar()
    {
            int value = (int)Console.ReadKey().KeyChar;
            return value;

    }

    public void PrintAsciiChar()
    {
        int asciiCode = Pop(); // извлекаем значение из стека
        Console.Write((char)asciiCode); // выводим ASCII символ на экран
    }

    public void Swap()
    {
        int a = Pop();
        int b = Pop();
        Push(a);
        Push(b);
    }

    public void Dup()
    {
        int a = Pop();
        Push(a);
        Push(a);
    }

}

//Вывод данных в терминал (Сделано)
//Интерпретатор в реальном времени (Сделано, частично, отложено)
//Дисковое пространство (Сделано)
//БОльшая работа с памятью
//БИОС с проверкой всех систем



class Program
{
    static void Main()
    {
        const string filename = "disc0.txt";
        int[] program = {0x30, 0x12, 0x0a, 0x31, 0x31, 0x12, 0x0a, 0x31, 0xff };
        VirtualMachine vm = new VirtualMachine(256, 50, filename);
        vm.LoadProgram(program);
        vm.Run();
        Console.WriteLine("Процесс завершён нажмите любую клавишу для продолжения");
        Console.ReadKey();
    }
}