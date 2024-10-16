using UnityEngine;
using System.Collections.Generic;

/* hexagonal helper class for inter coordinate conversion & screen to/fro coordinate conversions 
 * details can be found in my tutorial on the same
 * https://gamedevelopment.tutsplus.com/tutorials/introduction-to-axial-coordinates-for-hexagonal-tile-based-games--cms-28820
 * 
 * essentially we deal with 3 coordinate systems
 * 1. offset coordinate which is the values ofr a 2 dimensional array which could store the level values in file
 * 2. axial coordinate which is derived from the offset coordinate
 * 3. cubic coordinate which retains both values of the axial coordinate and adds a third value based on logic, x+y+z=0
 * 
 * The code only works for the vertically aligned hexagonal grid layout with odd offset
 * ie, odd colums are offset vertically and tiles are vertically aligned (pointy ends sidewise)
 * apart from this layout there are 3 other layouts possible for a hexagonal grid.
 */
public static class HexHelperVertical 
{
	public static float rootThree=Mathf.Sqrt(3);//square root of three

	/*
	 * screen point to axial coordinate conversion
	 * */
	public static Vector2 screenToAxial(Vector2 screenPoint, float sideLength){
		var axialPoint=new Vector2();
		axialPoint.y=screenPoint.x/(1.5f*sideLength);
		axialPoint.x=(screenPoint.y-(screenPoint.x/rootThree))/(rootThree*sideLength);
		var cubicZ=calculateCubicZ(axialPoint);
		var round_x=Mathf.Round(axialPoint.x);
		var round_y=Mathf.Round(axialPoint.y);
		var round_z=Mathf.Round(cubicZ);
		if(round_x+round_y+round_z==0){
			screenPoint.x=round_x;
			screenPoint.y=round_y;
		}else{
			var delta_x=Mathf.Abs(axialPoint.x-round_x);
			var delta_y=Mathf.Abs(axialPoint.y-round_y);
			var delta_z=Mathf.Abs(cubicZ-round_z);
			if(delta_x>delta_y && delta_x>delta_z){
				screenPoint.x=-round_y-round_z;
				screenPoint.y=round_y;
			}else if(delta_y>delta_x && delta_y>delta_z){
				screenPoint.x=round_x;
				screenPoint.y=-round_x-round_z;
			}else if(delta_z>delta_x && delta_z>delta_y){
				screenPoint.x=round_x;
				screenPoint.y=round_y;
			}
		}
		return screenPoint;
	}
	/*
	 * axial coordinate to screen position conversion
	 * */
	public static Vector2 axialToScreen(Vector2 axialPoint, float sideLength){
		var tileY=rootThree*sideLength*(axialPoint.x+(axialPoint.y/2));
		var tileX=3*sideLength/2*axialPoint.y;
		axialPoint.x=tileX;
		axialPoint.y=tileY;
		return axialPoint;
	}
	/*
	 * offset coordinate to axial coordinate conversion
	 * */
	public static Vector2 offsetToAxial(Vector2 offsetPt){
		offsetPt.x=(offsetPt.x-(Mathf.Floor(offsetPt.y/2)));
		return offsetPt;
	}
	/*
	 * axial coordinate to offset coordinate conversion
	 * */
	public static Vector2 axialToOffset(Vector2 axialPt){
		axialPt.x=(axialPt.x+(Mathf.Floor(axialPt.y/2)));
		return axialPt;
	}
	/*
	 * find the third value of the cubic coordinate with the logic that x+y+z=0
	 * */
	public static float calculateCubicZ(Vector2 newAxialPoint){
		return -newAxialPoint.x-newAxialPoint.y;
	}
	/*
	 * find the neighbors as a list of axial points for the given axial coordinate
	 * */
	public static List<Vector2> getNeighbors(Vector2 axialPoint){//assign 6 neighbors
		Vector2 neighbourPoint=new Vector2();
		List<Vector2> neighbors=new List<Vector2>();
		neighbourPoint.x=axialPoint.x+1;//top
		neighbourPoint.y=axialPoint.y;
		neighbors.Add(neighbourPoint);
		neighbourPoint.x=axialPoint.x;//top right
		neighbourPoint.y=axialPoint.y+1;
		neighbors.Add(neighbourPoint);
		neighbourPoint.x=axialPoint.x-1;//bottom right
		neighbourPoint.y=axialPoint.y+1;
		neighbors.Add(neighbourPoint);
		neighbourPoint.x=axialPoint.x-1;//bottom
		neighbourPoint.y=axialPoint.y;
		neighbors.Add(neighbourPoint);
		neighbourPoint.x=axialPoint.x;//bottom left
		neighbourPoint.y=axialPoint.y-1;
		neighbors.Add(neighbourPoint);
		neighbourPoint.x=axialPoint.x+1;//top left
		neighbourPoint.y=axialPoint.y-1;
		neighbors.Add(neighbourPoint);
		return neighbors;
	}
}