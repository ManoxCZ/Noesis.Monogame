
namespace Noesis.MonoGame;

public class NoesisRenderTarget : RenderTarget
{
    public string Label { get; }
    public Microsoft.Xna.Framework.Graphics.RenderTarget2D RenderTarget2D { get; }
    public override Texture Texture { get; }


    public NoesisRenderTarget(string label, Microsoft.Xna.Framework.Graphics.RenderTarget2D renderTarget2D)
    {
        Label = label;
        RenderTarget2D = renderTarget2D;
     
        Texture = new NoesisTexture(Label + "_Texture", RenderTarget2D);
    }
}
