using UnityEngine;
using System.Collections;
using Noesis;

public class ControlGalleryLogic : MonoBehaviour
{
    FrameworkElement _root;

    ResourceDictionary _noesisStyleResources;
    ResourceDictionary _simpleStyleResources;
    ResourceDictionary _windowsStyleResources;
    ResourceDictionary _orbStyleResources;
    ComboBox _styleSelector;
    Panel _container;
    TreeView _samples;

    Border _container1;
    Border _container2;
    Border _descHost1;
    Border _descHost2;

    Storyboard _showContainer1;
    Storyboard _showContainer2;

    int _visibleContainer;
    string _lastSample;

    // Use this for initialization
    void Start ()
    {
        NoesisGUIPanel gui = GetComponent<NoesisGUIPanel>();

        _root = gui.GetRoot<FrameworkElement>();

        _noesisStyleResources = Noesis.Kernel.LoadXaml<ResourceDictionary>(
            "Assets/NoesisGUI/Themes/NoesisStyle.xaml");
        _simpleStyleResources = Noesis.Kernel.LoadXaml<ResourceDictionary>(
            "Assets/NoesisGUI/Themes/SimpleStyle.xaml");
        _windowsStyleResources = Noesis.Kernel.LoadXaml<ResourceDictionary>(
            "Assets/NoesisGUI/Themes/WindowsStyle.xaml");
        _orbStyleResources = Noesis.Kernel.LoadXaml<ResourceDictionary>(
            "Assets/NoesisGUI/Themes/OrbStyle.xaml");

        _styleSelector = _root.FindName<ComboBox>("StyleSelector");
        _styleSelector.SelectionChanged += OnStyleSelectionChanged;

        _samples = _root.FindName<TreeView>("Samples");
        _samples.SelectedItemChanged += OnSamplesSelectionChanged;

        _container = _root.FindName<Panel>("Container");

        _container1 = _root.FindName<Border>("Container1");
        _container2 = _root.FindName<Border>("Container2");
        _descHost1 = _root.FindName<Border>("DescriptionHost1");
        _descHost2 = _root.FindName<Border>("DescriptionHost2");

        _showContainer1 = _root.FindStringResource<Storyboard>("ShowContainer1");
        _showContainer1.Completed += OnShowContainer1Completed;

        _showContainer2 = _root.FindStringResource<Storyboard>("ShowContainer2");
        _showContainer2.Completed += OnShowContainer2Completed;

        // initially load Button sample
        UIElement sample, desc;
        LoadSample("Button.xaml", out sample, out desc);

        _container1.SetChild(sample);
        _container1.SetVisibility(Visibility.Visible);

        _descHost1.SetChild(desc);
        _descHost1.SetVisibility(Visibility.Visible);
    }

    void OnStyleSelectionChanged(BaseComponent sender, SelectionChangedEventArgs args)
    {
        switch (_styleSelector.GetSelectedIndex())
        {
            case 0:
            {
                _container.SetResources(_noesisStyleResources);
                break;
            }
            case 1:
            {
                _container.SetResources(_simpleStyleResources);
                break;
            }
            case 2:
            {
                _container.SetResources(_windowsStyleResources);
                break;
            }
            case 3:
            {
                _container.SetResources(_orbStyleResources);
                break;
            }
        }

        args.handled = true;
    }

    void OnSamplesSelectionChanged(BaseComponent oldValue, BaseComponent newValue)
    {
        TreeViewItem tvi = newValue.As<TreeViewItem>();
        if (tvi != null && !tvi.GetHasItems())
        {
            string sampleName = tvi.GetTag().AsString();
            if (_lastSample != sampleName)
            {
                LoadSample(sampleName);
                _lastSample = sampleName;
            }
        }
    }

    void OnShowContainer1Completed(BaseComponent sender, TimelineEventArgs args)
    {
        _container2.SetChild(null);
        _samples.SetIsEnabled(true);
    }

    void OnShowContainer2Completed(BaseComponent sender, TimelineEventArgs args)
    {
        _container1.SetChild(null);
        _samples.SetIsEnabled(true);
    }

    void LoadSample(string sampleName)
    {
        UIElement sample, desc;
        LoadSample(sampleName, out sample, out desc);

        _samples.SetIsEnabled(false);

        if (_visibleContainer == 1)
        {
            _container2.SetChild(sample);
            _descHost2.SetChild(desc);
            _showContainer2.Begin(_root);
            _visibleContainer = 2;
        }
        else
        {
            _container1.SetChild(sample);
            _descHost1.SetChild(desc);
            _showContainer1.Begin(_root);
            _visibleContainer = 1;
        }
    }

    void LoadSample(string sampleName, out UIElement sample, out UIElement desc)
    {
        sample = Noesis.Kernel.LoadXaml<UIElement>(
            "Assets/NoesisGUI/Samples/ControlGallery/Samples/" + sampleName);

        desc = Noesis.Kernel.LoadXaml<UIElement>(
            "Assets/NoesisGUI/Samples/ControlGallery/Desc/" + sampleName);
    }
}
