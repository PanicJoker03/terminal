using System;
using System.IO;
using System.Reflection;
using System.Threading;
/// <summary>
/// Cada dia mas trabajador
/// al chile lo admito
/// </summary>
/// 
namespace GolemSharp
{
    class Program
    {
        //todo: refactorize
        static void Main(string[] args)
        {
            GolemSharp golem = new GolemSharp();
            golem.Act();
        }
    }
}
