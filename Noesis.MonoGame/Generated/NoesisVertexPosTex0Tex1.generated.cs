// <auto-generated/>
using System;
using System.Diagnostics;
﻿using System.Runtime.InteropServices;

namespace Noesis.MonoGame.Generated
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NoesisVertexPosTex0Tex1 : Microsoft.Xna.Framework.Graphics.IVertexType
    {
        private static readonly Microsoft.Xna.Framework.Graphics.VertexDeclaration _vertexDeclaration;
        Microsoft.Xna.Framework.Graphics.VertexDeclaration  Microsoft.Xna.Framework.Graphics.IVertexType.VertexDeclaration => _vertexDeclaration;        

		public Microsoft.Xna.Framework.Vector2 Pos;
		public Microsoft.Xna.Framework.Vector2 Tex0;
		public Microsoft.Xna.Framework.Vector2 Tex1;

        static NoesisVertexPosTex0Tex1()
		{
			Microsoft.Xna.Framework.Graphics.VertexElement[] elements = new Microsoft.Xna.Framework.Graphics.VertexElement[] 
            { 
				new (0, Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2, Microsoft.Xna.Framework.Graphics.VertexElementUsage.Position, 0),
				new (8, Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2, Microsoft.Xna.Framework.Graphics.VertexElementUsage.TextureCoordinate, 0),
				new (16, Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2, Microsoft.Xna.Framework.Graphics.VertexElementUsage.TextureCoordinate, 1),

            };			
			_vertexDeclaration = new Microsoft.Xna.Framework.Graphics.VertexDeclaration(elements);

            Debug.Assert(_vertexDeclaration.VertexStride == Marshal.SizeOf<NoesisVertexPosTex0Tex1>());
		}
    }
}
