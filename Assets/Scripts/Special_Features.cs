using System;

public class Special_Features {
	private Game_Controller.SpecialFeatureList feature;
	private bool isActive;
	private float counter;
	private float limit;

	public Special_Features(Game_Controller.SpecialFeatureList pFeature, float pCounter, float pLimit,bool pIsActive){
		this.Feature = pFeature;
		this.Counter = pCounter;
		this.limit = pLimit;
		this.isActive = pIsActive;
	}

	public Game_Controller.SpecialFeatureList Feature {
		get {
			return feature;
		}
		set {
			feature = value;
		}
	}

	public float Counter {
		get {
			return counter;
		}
		set {
			counter = value;
		}
	}

	public float Limit {
		get {
			return limit;
		}
		set {
			limit = value;
		}
	}

	public bool IsActive {
		get {
			return isActive;
		}
		set {
			isActive = value;
		}
	}
}
