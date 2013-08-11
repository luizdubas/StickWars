using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface PathFinding{	
	void clearStartAndTarget();
	void ClearLogic();
	
	bool PathFind( Vector2 start, Vector2 target );
	
	List<Square> getPath();
}

