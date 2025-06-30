
using System.Threading.Tasks;

public class CombineTilesUndoAction : IUndoAction
{
    private readonly IColorSource _sourceA;
    private readonly IColorSource _sourceB;
    private readonly ColorVector _originalColorA;
    private readonly ColorVector _originalColorB;
    private readonly bool _hadResultColor;

    public CombineTilesUndoAction(IColorSource sourceA,
                               IColorSource sourceB,
                               ColorVector originalColorA,
                               ColorVector originalColorB,
                               bool hadResultColor)
    {
        _sourceA = sourceA;
        _sourceB = sourceB;
        _originalColorA = originalColorA;
        _originalColorB = originalColorB;
        _hadResultColor = hadResultColor;
    }

    public async Task UndoAsync()
    {
        await Task.CompletedTask;
    }
}

