using System.Collections.Generic;

namespace Calcifer.Engine.Content.Data
{
    public interface IContentProvider
    {
        IEnumerable<AssetEntry> GetAssets();
    }
}