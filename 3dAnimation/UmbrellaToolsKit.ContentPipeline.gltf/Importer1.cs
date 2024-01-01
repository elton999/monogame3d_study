using Microsoft.Xna.Framework.Content.Pipeline;
using glTFLoader;

using TImport = glTFLoader.Schema.Gltf;

namespace UmbrellaToolsKit.ContentPipeline.gltf
{
    [ContentImporter(".gltf", DisplayName = "gltf - UmbrellaToolsKit", DefaultProcessor = "Processor1")]
    public class Importer1 : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            glTFLoader.Schema.Gltf gltf = Interface.LoadModel(filename);
            return gltf;
        }
    }
}