using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Noesis;

public class UnityObject : BaseComponent
{
    private HandleRef swigCPtr;
    
    // Type property
    public string Type
    {
        get { return mType; }
        set
        {
            if (mType != value)
            {
                mType = value;
                Noesis.Extend.PropertyChanged(typeof(UnityObject), swigCPtr.Handle, "Type");
            }
        }
    }
    
    // Color property
    public SolidColorBrush Color
    {
        get { return mColor; }
        set
        {
            if (mColor != value)
            {
                mColor = value;
                Noesis.Extend.PropertyChanged(typeof(UnityObject), swigCPtr.Handle, "Color");
            }
        }
    }
    
    public string Scale
    {
        get { return mScale; }
        set
        {
            if (mScale != value)
            {
                mScale = value;
                Noesis.Extend.PropertyChanged(typeof(UnityObject), swigCPtr.Handle, "Scale");
            }
        }
    }
    
    public string Pos
    {
        get { return mPos; }
        set
        {
            if (mPos != value)
            {
                mPos = value;
                Noesis.Extend.PropertyChanged(typeof(UnityObject), swigCPtr.Handle, "Pos");
            }
        }
    }
    
    public GameObject MyObject
    {
        set { mObject = value; }
        get { return mObject; }    
    }
    
    public static void Register()
    {
    }
    
    public UnityObject(IntPtr cPtr, bool cMemoryOwn)
        : base(cPtr, cMemoryOwn)
    {
        swigCPtr = new HandleRef(this, cPtr);
    }
    
    public UnityObject()
        : this(Noesis.Extend.New(typeof(UnityObject)), true)
    {
        Noesis.Extend.Register(typeof(UnityObject), swigCPtr.Handle, this);

    }
    
    public override void Dispose()
    {
        lock (this)
        {
            if (swigCPtr.Handle != IntPtr.Zero)
            {
                if (swigCMemOwn)
                {
                    swigCMemOwn = false;
                    if (Noesis.Kernel.IsInitialized())
                    {
                        Noesis.Extend.Delete(typeof(UnityObject), swigCPtr.Handle);
                    }
                }
                swigCPtr = new HandleRef(null, IntPtr.Zero);
            }
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }

    // X scale
    //@{
    public void SetScaleX(float val)
    {
        mScaleX = val;
        UpdateScale();
    }
    public float GetScaleX()
    {
        return mScaleX;
    }
    //@}
    
    // Y scale
    //@{
    public void SetScaleY(float val)
    {
        mScaleY = val;
        UpdateScale();
    }
    public float GetScaleY()
    {
        return mScaleY;
    }
    //@}
    
    // Z scale
    //@{
    public void SetScaleZ(float val)
    {
        mScaleZ = val;
        UpdateScale();
    }
    public float GetScaleZ()
    {
        return mScaleZ;
    }
    //@}
    
    // X position
    //@{
    public void SetPosX(float val)
    {
        mPosX = val;
        UpdatePos();
    }
    public float GetPosX()
    {
        return mPosX;
    }
    //@}
    
    // Y position
    //@{
    public void SetPosY(float val)
    {
        mPosY = val;
        UpdatePos();
    }
    public float GetPosY()
    {
        return mPosY;
    }
    //@}
    
    // Z position
    //@{
    public void SetPosZ(float val)
    {
        mPosZ = val;
        UpdatePos();
    }
    
    public float GetPosZ()
    {
        return mPosZ;
    }
    //@}
    
    private void UpdatePos()
    {
        Pos = String.Format("Position: (X:{0:0.00}, Y:{1:0.00}, Z:{2:0.00})",
            mPosX, mPosY, mPosZ);
    }
    
    private void UpdateScale()
    {
        Scale = String.Format("Scale: (X:{0:0.00}, Y:{1:0.00}, Z:{2:0.00})",
            mScaleX, mScaleY, mScaleZ);
    }
    
    // Type
    private string mType;
    
    // Color
    private SolidColorBrush mColor;
    
    // Scale string
    private string mScale;
    
    // Position string
    private string mPos;
    
    // Scale
    private float mScaleX;
    private float mScaleY;
    private float mScaleZ;
    
    // Position
    private float mPosX;
    private float mPosY;
    private float mPosZ;
    
    private GameObject mObject;    
}

////////////////////////////////////////////////////////////////////////////////////////////
public class PrimitivesController : MonoBehaviour 
{
    
    FrameworkElement mRoot;
    
    ColorPicker colorPicker;
    
    Slider mX;
    Slider mY;
    Slider mZ;
    
    Slider mScaleX;
    Slider mScaleY;
    Slider mScaleZ;
    
    Button mTypeSphere;
    Button mTypeCapsule;
    Button mTypeCylinder;
    Button mTypeCube;
    Button mTypePlane;
    
    GameObject mDirLight;
    
    GameObject mSelectedObject;
    
    Slider mSunDir;
    
    Border mUpdateGB;
    TextBlock mUpdateGBHeader;
    
    TextBlock mSelLbl;
    TranslateTransform mSelLblTrans;
    
    System.Collections.Generic.Dictionary<GameObject, UnityObject> mObjs;
    
    ListBox mListBox;
    
    bool mouseOnGUI;
    
    Collection mCollection;
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    // Use this for initialization
    void Start () 
    {
        // Access to the NoesisGUIPanel component
        NoesisGUIPanel noesisGUI = GetComponent<NoesisGUIPanel>();

        // Obtain the root of the loaded UI resource, in this case it is a Grid element
        this.mRoot = noesisGUI.GetRoot<FrameworkElement>();
        this.mRoot.FindName<DockPanel>("ControlPanel").MouseEnter += this.OnMouseEnter;
        this.mRoot.FindName<DockPanel>("ControlPanel").MouseLeave += this.OnMouseLeave;
        
        this.mSunDir = this.mRoot.FindName<Slider>("sliderSun");
        this.mSunDir.ValueChanged += this.OnSunDirChanged;
        
        this.colorPicker = this.mRoot.FindName<ColorPicker>("ColorPicker");
        
        this.mX = this.mRoot.FindName<Slider>("Xval");
        this.mX.ValueChanged += this.OnXPosChanged;
        this.mY = this.mRoot.FindName<Slider>("Yval");
        this.mY.ValueChanged += this.OnYPosChanged;
        this.mZ = this.mRoot.FindName<Slider>("Zval");
        this.mZ.ValueChanged += this.OnZPosChanged;
        
        this.mScaleX = this.mRoot.FindName<Slider>("ScaleXval");
        this.mScaleX.ValueChanged += this.OnScaleXChanged;
        this.mScaleY = this.mRoot.FindName<Slider>("ScaleYval");
        this.mScaleY.ValueChanged += this.OnScaleYChanged;
        this.mScaleZ = this.mRoot.FindName<Slider>("ScaleZval");
        this.mScaleZ.ValueChanged += this.OnScaleZChanged;
        
        this.mTypeSphere = this.mRoot.FindName<Button>("TypeSphere");
        this.mTypeSphere.Click += this.OnCreateSphere;
        this.mTypeCapsule = this.mRoot.FindName<Button>("TypeCapsule");
        this.mTypeCapsule.Click += this.OnCreateCapsule;
        this.mTypeCylinder = this.mRoot.FindName<Button>("TypeCylinder");
        this.mTypeCylinder.Click += this.OnCreateCylinder;
        this.mTypeCube = this.mRoot.FindName<Button>("TypeCube");
        this.mTypeCube.Click += this.OnCreateCube;
        this.mTypePlane = this.mRoot.FindName<Button>("TypePlane");
        this.mTypePlane.Click += this.OnCreatePlane;
        
        this.mUpdateGB = this.mRoot.FindName<Border>("UpdateGB");
        this.mUpdateGB.SetIsEnabled(false);
        this.mUpdateGBHeader = this.mRoot.FindName<TextBlock>("UpdateGBHeader");
        
        this.mSelLbl = this.mRoot.FindName<TextBlock>("SelectedLbl");
        this.mSelLbl.SetVisibility(Visibility.Hidden);
        
        this.mSelLblTrans = this.mRoot.FindName<TranslateTransform>("selectLblPos");
        
        this.mObjs = new System.Collections.Generic.Dictionary<GameObject, UnityObject>();
        
        this.mDirLight = GameObject.Find("Key light");
        this.mSelectedObject = null;
        
        this.mListBox = this.mRoot.FindName<ListBox>("MainLB");
        
        this.mouseOnGUI = false;
        
        this.mCollection = new Collection();
        this.mListBox.SetItemsSource(this.mCollection);
        this.mListBox.SetSelectionMode(SelectionMode.Single);
        this.mListBox.SelectionChanged += this.OnSelectionChanged;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    // Update is called once per frame
    void Update () 
    {        
        if (!this.mouseOnGUI && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000.0f))
            {
                this.mSelectedObject = hit.collider.gameObject;
                this.mUpdateGB.SetIsEnabled(true);
                this.mUpdateGBHeader.SetText(this.mSelectedObject.name);
                this.mSelLbl.SetVisibility(Visibility.Visible);
                
                this.FillDataFromSelObj();
                
                UnityObject obj = this.mObjs[this.mSelectedObject];
                this.mListBox.SetSelectedItem(obj.As<BaseComponent>());
            }
            else
            {
                this.mSelectedObject = null;
                this.mUpdateGB.SetIsEnabled(false);
                this.mSelLbl.SetVisibility(Visibility.Hidden);
                this.mListBox.SetSelectedItem(null);
            }
           }
        
        if (mSelectedObject != null)
        {        
            Vector3 scPos = Camera.main.WorldToScreenPoint(this.mSelectedObject.transform.position);
            this.mSelLblTrans.SetX(scPos.x - 28);
            this.mSelLblTrans.SetY(Camera.main.pixelHeight - scPos.y - 7);
            
            SolidColorBrush brush = colorPicker.Color.As<SolidColorBrush>();
            mSelectedObject.renderer.material.SetColor ("_Color",
                new UnityEngine.Color(brush.GetColor().GetRedF(), 
                brush.GetColor().GetGreenF(), 
                brush.GetColor().GetBlueF(), 
                brush.GetColor().GetAlphaF()));
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnSunDirChanged(float oldValue, float newValue)
    {        
        this.mDirLight.transform.localEulerAngles = new Vector3(50, -newValue, 0);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnRedChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            UnityEngine.Color c = 
                mSelectedObject.renderer.material.GetColor ("_Color");
            
            mSelectedObject.renderer.material.SetColor ("_Color",
                new UnityEngine.Color(newValue / 255.0f, c.g, c.b, 1));
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            SolidColorBrush brush = obj.Color;
            brush.SetColor(new Noesis.Color(newValue / 255.0f, c.g, c.b, 1.0f));
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnGreenChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            UnityEngine.Color c = 
                mSelectedObject.renderer.material.GetColor ("_Color");
            
            mSelectedObject.renderer.material.SetColor ("_Color",
                new UnityEngine.Color(c.r, newValue / 255.0f, c.b, 1));
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            SolidColorBrush brush = obj.Color;
            brush.SetColor(new Noesis.Color(c.r, newValue / 255.0f, c.b, 1.0f));
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnBlueChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {            
            UnityEngine.Color c = 
                mSelectedObject.renderer.material.GetColor ("_Color");
            
            mSelectedObject.renderer.material.SetColor ("_Color",
                new UnityEngine.Color(c.r, c.g, newValue / 255.0f, 1));
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            SolidColorBrush brush = obj.Color;
            brush.SetColor(new Noesis.Color(c.r, c.g, newValue / 255.0f, 1.0f));
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnXPosChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {            
            mSelectedObject.transform.position = new Vector3(newValue,
                mSelectedObject.transform.position.y,
                mSelectedObject.transform.position.z);
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            obj.SetPosX(newValue);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnYPosChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            mSelectedObject.transform.position = new Vector3(
                mSelectedObject.transform.position.x,
                newValue,
                mSelectedObject.transform.position.z);
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            obj.SetPosY(newValue);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnZPosChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            mSelectedObject.transform.position = new Vector3(
                mSelectedObject.transform.position.x,
                mSelectedObject.transform.position.y,
                newValue);
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            obj.SetPosZ(newValue);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnScaleXChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            mSelectedObject.transform.localScale = new Vector3(
                newValue,
                mSelectedObject.transform.localScale.y,
                mSelectedObject.transform.localScale.z);
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            obj.SetScaleX(newValue);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnScaleYChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            mSelectedObject.transform.localScale = new Vector3(
                mSelectedObject.transform.localScale.x,
                newValue,
                mSelectedObject.transform.localScale.z);
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            obj.SetScaleY(newValue);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnScaleZChanged(float oldValue, float newValue)
    {
        if (mSelectedObject != null)
        {
            mSelectedObject.transform.localScale = new Vector3(
                mSelectedObject.transform.localScale.x,
                mSelectedObject.transform.localScale.y,
                newValue);
            
            UnityObject obj = this.mObjs[this.mSelectedObject];
            obj.SetScaleZ(newValue);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnMouseEnter(BaseComponent e, MouseEventArgs a)
    {
        this.mouseOnGUI = true;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnMouseLeave(BaseComponent e, MouseEventArgs a)
    {
        this.mouseOnGUI = false;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnCreateSphere(BaseComponent sender, RoutedEventArgs e)
    {    
        CreatePrimitiveObject(PrimitiveType.Sphere);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnCreateCapsule(BaseComponent sender, RoutedEventArgs e)
    {    
        CreatePrimitiveObject(PrimitiveType.Capsule);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnCreateCylinder(BaseComponent sender, RoutedEventArgs e)
    {    
        CreatePrimitiveObject(PrimitiveType.Cylinder);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnCreateCube(BaseComponent sender, RoutedEventArgs e)
    {    
        CreatePrimitiveObject(PrimitiveType.Cube);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnCreatePlane(BaseComponent sender, RoutedEventArgs e)
    {    
        CreatePrimitiveObject(PrimitiveType.Plane);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void CreatePrimitiveObject(PrimitiveType primitiveType)
    {
        GameObject obj = GameObject.CreatePrimitive(primitiveType);
        obj.transform.position = new Vector3(0, 0, 0);
        obj.transform.localScale = new Vector3(30, 30, 30);
        
        obj.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        obj.renderer.material.SetColor ("_Color", UnityEngine.Color.white);
        
        UnityObject myObj = new UnityObject();
        
        myObj.Color = new SolidColorBrush(new Noesis.Color(255, 255, 255, 255));
        
        myObj.SetScaleX(30);
        myObj.SetScaleY(30);
        myObj.SetScaleZ(30);
        
        myObj.SetPosX(0);
        myObj.SetPosY(0);
        myObj.SetPosZ(0);
        
        myObj.Type = obj.name;
        
        myObj.MyObject = obj;

        this.mObjs.Add(obj, myObj);
        
        this.mCollection.Add(myObj);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void FillDataFromSelObj()
    {    
        colorPicker.Color = this.mObjs[this.mSelectedObject].Color;
        
        this.mX.SetValue(this.mSelectedObject.transform.position.x);
        this.mY.SetValue(this.mSelectedObject.transform.position.y);
        this.mZ.SetValue(this.mSelectedObject.transform.position.z);
        
        this.mScaleX.SetValue(this.mSelectedObject.transform.localScale.x);
        this.mScaleY.SetValue(this.mSelectedObject.transform.localScale.y);
        this.mScaleZ.SetValue(this.mSelectedObject.transform.localScale.z);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    void OnSelectionChanged(BaseComponent c, SelectionChangedEventArgs args)
    {
        int idxSel = this.mListBox.GetSelectedIndex();
        if (idxSel < 0)
        {
            this.mSelectedObject = null;
            this.mUpdateGB.SetIsEnabled(false);
            this.mSelLbl.SetVisibility(Visibility.Hidden);
        }
        else
        {
            this.mSelectedObject = this.mListBox.GetSelectedItem().As<UnityObject>().MyObject;
    
            this.FillDataFromSelObj();
    
            this.mUpdateGB.SetIsEnabled(true);
            this.mUpdateGBHeader.SetText(this.mSelectedObject.name);
            this.mSelLbl.SetVisibility(Visibility.Visible);
        }
    }
}
