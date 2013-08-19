using UnityEngine;

public class CameraControllerInGame : MonoBehaviour{
    private int ScrollArea = 50;
    private int ScrollSpeed = 50;
    private int DragSpeed = 100;

    private int ZoomSpeed = 25;
    private int ZoomMin;
    private int ZoomMax;

    private int PanSpeed = 50;
    private int PanAngleMin = 25;
    private int PanAngleMax = 80;
	
	private bool pannoramicCamera = false;
	private bool middleMouseButtonMovement = false;
	
	private bool  rotation = true;
	private float rotationTime = 1;
	private float rotationFactor = 0;
	private bool  rotatingRight = false;
	private bool  rotatingLeft = false;
	private float rotationTimeCount = 0;
	private float rotationInitialEullerY;
	
	private bool enable = false;
	
	private float leftMargin;
	private float rightMargin;
	private float topMargin;
	private float bottomMargin;
	
	private Map map;
	
	private GameObject cameraGameObject;
	
	public CameraControllerInGame(){
	}
	
	void Awake( ){
		//enable = false;
		
		leftMargin = 0;
		rightMargin = 0;
		topMargin = 0;
		bottomMargin = 0;
		
		ZoomMin = -1;
		ZoomMax= -1;
		
		map = null;
		
		cameraGameObject = this.gameObject; //Consider that it'll always be attached to the Camera object
		
		cameraGameObject.transform.position = Vector3.zero;
		cameraGameObject.transform.rotation = Quaternion.identity;
	}
	
	void Start () {
	}

    void Update(){		
		if( enable ){
	        // Init camera translation for this frame.
	        Vector3 translation = Vector3.zero;
			float zoomDelta = 0;
	        
			// Zoom in or out
			zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
	        if (zoomDelta!=0){
	            translation -= Vector3.up * ZoomSpeed * zoomDelta;
	        }
			
			if( rotation ){
				if( rotatingLeft || rotatingRight ){
					rotationTimeCount += Time.deltaTime;
					
					if( rotationTimeCount <= rotationTime ){
						if( rotatingRight ){
							camera.transform.eulerAngles = new Vector3( camera.transform.eulerAngles.x, camera.transform.eulerAngles.y + ( rotationFactor * Time.deltaTime ), camera.transform.eulerAngles.z );
						}else if( rotatingLeft ){
							camera.transform.eulerAngles = new Vector3( camera.transform.eulerAngles.x, camera.transform.eulerAngles.y + ( rotationFactor * Time.deltaTime ), camera.transform.eulerAngles.z );
						}
					}else{
						float aux;
						if( rotatingRight ){
							camera.transform.eulerAngles = new Vector3( camera.transform.eulerAngles.x, rotationInitialEullerY  - 90, camera.transform.eulerAngles.z );

						}else if( rotatingLeft ){
							camera.transform.eulerAngles = new Vector3( camera.transform.eulerAngles.x, rotationInitialEullerY + 90, camera.transform.eulerAngles.z );
						}

						rotationTimeCount = 0;
						rotatingLeft = false;
						rotatingRight = false;
					}
				}else{
					if( Input.GetKeyDown("e") ){
						rotationTimeCount = 0;
						rotatingRight = true;
						rotationFactor = -90 / rotationTime;
						rotationInitialEullerY = camera.transform.eulerAngles.y;
					}
					if( Input.GetKeyDown("q") ){
						rotationTimeCount = 0;
						rotatingLeft = true;
						rotationFactor = 90 / rotationTime;
						rotationInitialEullerY = camera.transform.eulerAngles.y;
					}
				}
			}
	
	        // Start panning camera if zooming in close to the ground or if just zooming out.
			if( pannoramicCamera ){
				float pan = camera.transform.eulerAngles.x - zoomDelta * PanSpeed;
				pan = Mathf.Clamp(pan, PanAngleMin, PanAngleMax);
				if (zoomDelta < 0 || camera.transform.position.y < (ZoomMax / 2)){
					camera.transform.eulerAngles = new Vector3(pan, 0, 0);
				}
			}
	
	        // Move camera with arrow keys
	        translation += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			
			if( middleMouseButtonMovement ){
	        	// Move camera with mouse
	        	if (Input.GetMouseButton(2)){
	            	// Hold button and drag camera around
	            	translation -= new Vector3(Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime, 0, Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime);
				}
	        }
	        // Move camera if mouse pointer reaches screen borders
	        if (Input.mousePosition.x < ScrollArea){
				switch( (int) camera.transform.eulerAngles.y % 360 ){
				case 0:
					translation += Vector3.right * -ScrollSpeed * Time.deltaTime;
					break;
				case 90:
					translation += Vector3.back * -ScrollSpeed * Time.deltaTime;
					break;
				case 180:
					translation += Vector3.left * -ScrollSpeed * Time.deltaTime;
					break;
				case 270:
					translation += Vector3.forward * -ScrollSpeed * Time.deltaTime;
					break;
				}
	        }
	
	        if (Input.mousePosition.x >= Screen.width - ScrollArea){
				switch( (int) camera.transform.eulerAngles.y % 360 ){
				case 0:
					translation += Vector3.right * ScrollSpeed * Time.deltaTime;
					break;
				case 90:
					translation += Vector3.back * ScrollSpeed * Time.deltaTime;
					break;
				case 180:
					translation += Vector3.left * ScrollSpeed * Time.deltaTime;
					break;
				case 270:
					translation += Vector3.forward * ScrollSpeed * Time.deltaTime;
					break;
				}
	            
	        }
	
	        if (Input.mousePosition.y < ScrollArea){
				switch( (int) camera.transform.eulerAngles.y % 360 ){
				case 0:
					translation += Vector3.forward * -ScrollSpeed * Time.deltaTime;
					break;
				case 90:
					translation += Vector3.right * -ScrollSpeed * Time.deltaTime;
					break;
				case 180:
					translation += Vector3.back * -ScrollSpeed * Time.deltaTime;
					break;
				case 270:
					translation += Vector3.left * -ScrollSpeed * Time.deltaTime;
					break;
				}
	            
	        }
	
	        if (Input.mousePosition.y > Screen.height - ScrollArea){
				switch( (int) camera.transform.eulerAngles.y % 360 ){
				case 0:
					translation += Vector3.forward * ScrollSpeed * Time.deltaTime;
					break;
				case 90:
					translation += Vector3.right * ScrollSpeed * Time.deltaTime;
					break;
				case 180:
					translation += Vector3.back * ScrollSpeed * Time.deltaTime;
					break;
				case 270:
					translation += Vector3.left * ScrollSpeed * Time.deltaTime;
					break;
				}
	        }
	
	        // Keep camera within level and zoom area
	        Vector3 desiredPosition = camera.transform.position + translation;
			
	        if (desiredPosition.x <  map.StartLeft + leftMargin || map.EndRight + rightMargin < desiredPosition.x){
				translation.x = 0;
	        }
	        if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y){
	            translation.y = 0;
	        }
	        if (desiredPosition.z < map.EndBottom + bottomMargin || map.StartTop + topMargin < desiredPosition.z){
	            translation.z = 0;
	        }
	
	        // Finally move camera parallel to world axis
	        camera.transform.position += translation;
		}
    }
	
	public void setCameraPosition( Vector3 position ){
		cameraGameObject.transform.position = position;
	}
	
	public void setCameraRotation( Vector3 rotation ){
		cameraGameObject.transform.rotation = Quaternion.Euler(rotation);	
	}
	
	public void setMap( Map map ){
		this.map = map;
	}
	
	public void setMargins(float left, float top, float right, float bottom){
		leftMargin = left;
		topMargin = top;
		rightMargin = right;
		bottomMargin = bottom;
	}
	
	public void setMinMaxZoom( int minZoom, int maxZoom ){
		ZoomMin = minZoom;
		ZoomMax = maxZoom;
	}
	
	public bool activate(){
		bool validConfiguration = true;
		
		if( cameraGameObject.transform.position == Vector3.zero ){
			validConfiguration = false;
		}
		if( cameraGameObject.transform.rotation == Quaternion.identity ){
			validConfiguration = false;
		}
		if( map == null ){
			validConfiguration = false;
		}
		
		if( ZoomMin == -1 || ZoomMax == -1 ){
			validConfiguration = false;
		}
		
		if( validConfiguration ){
			enable = true;
		}
		
		return validConfiguration;
	}
	
	public void deactivate(){
		enable = false;	
	}
}