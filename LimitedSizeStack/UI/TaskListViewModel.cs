using System;
using System.Linq;
using ReactiveUI;

namespace LimitedSizeStack.UI;

public class TaskListViewModel : ReactiveObject
{
    private readonly ListModel<string> _model;
    private string[] _items;
    private int _selectedIndex = -1;

    public TaskListViewModel(ListModel<string> listModel)
    {
        _model = listModel;
        Update();
    }

    public string[] Items
    {
        get => _items;
        private set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }

    public bool CanUndo => _model.CanUndo();

    public void AddItem(string item)
    {
        _model.AddItem(item);
        Update();
    }

    public void RemoveItem(int index)
    {
        _model.RemoveItem(index);
        Update();
    }

    public void Undo()
    {
        _model.Undo();
        Update();
    }

    public void MoveUpItem(int index)
    {
        if (index > 0 && index < Items.Length)
        {
            _model.MoveUpItem(index);
            Update();
        }
    }

    private void Update()
    {
        Items = _model.Items.ToArray();
        this.RaisePropertyChanged(nameof(CanUndo));
    }
}