using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;

namespace LimitedSizeStack.UI;

public partial class MainWindow : Window
{
    private readonly TaskListViewModel model;

    public MainWindow()
    {
        InitializeComponent();

        var list = new List<string> { "Составить список дел на сегодня", "Домашка по C#", "Решить задачу 1519" };
        var listModel = new ListModel<string>(list, 20);

        model = new TaskListViewModel(listModel);

        //Add
        ButtonAdd.Content = "Добавить";
        ButtonAdd.Click += (_, __) => { AddTask(); };

        //Undo
        ButtonUndo.Content = "Отменить";
        ButtonUndo.IsEnabled = model.CanUndo;
        ButtonUndo.Click += (_, __) =>
        {
            if (model.CanUndo)
                model.Undo();
        };

        //Remove
        ButtonRemove.Content = "Удалить";
        ButtonRemove.IsEnabled = false;
        ButtonRemove.Click += (_, __) =>
        {
            var index = TasksList.SelectedIndex;
            if (index == -1) return;

            model.RemoveItem(index);
        };

        //MoveUp
        ButtonMoveUp.Content = "Вверх";
        ButtonMoveUp.IsEnabled = false;
        ButtonMoveUp.Click += (_, __) =>
        {
            var index = TasksList.SelectedIndex;
            if (index > 0)
                model.MoveUpItem(index);
        };

        TextBox.KeyDown += (_, args) =>
        {
            if (args.Key != Key.Enter) return;

            AddTask();
            args.Handled = true;
        };

        TasksList.DataContext = model;
        TasksList.SelectionChanged += (_, __) =>
        {
            UpdateButtonsState(); 
        };

        foreach (var button in new[] { ButtonRemove, ButtonUndo, ButtonAdd, ButtonMoveUp })
            button.Click += (_, __) => UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        ButtonUndo.IsEnabled = model.CanUndo;

        int selectedIndex = TasksList.SelectedIndex;
        ButtonRemove.IsEnabled = selectedIndex != -1;
        ButtonMoveUp.IsEnabled = selectedIndex > 0;
    }

    private void AddTask()
    {
        model.AddItem(string.IsNullOrWhiteSpace(TextBox.Text)
            ? "(empty)"
            : TextBox.Text);
        TextBox.Text = "";
    }
}