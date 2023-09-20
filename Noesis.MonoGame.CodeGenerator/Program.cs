using Noesis.MonoGame.CodeGenerator;

class Program
{
    static void Main(string[] args)
    {
        VertexSourceGenerator vertexSourceGenerator = new();
        vertexSourceGenerator.Execute("../../../../../Noesis.MonoGame/Generated");

        ShadersGenerator shadersGenerator = new();
        shadersGenerator.Execute("../../../../../Noesis.MonoGame/Content");
    }
}