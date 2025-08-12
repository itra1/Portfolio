#define SPINE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Spine.Unity;

public class LightTween : MonoBehaviour {
	
	private delegate float EasingFunction(float start, float end, float Value);
	private EasingFunction easeFunc;
	public delegate void DelegateOnComplete();
	private DelegateOnComplete OnComplete;
	
	public GameObject objectToSendEvent;
	
	public LightTween.EaseType easeType;
	public LightTween.TweenType tweenType;
	
	public float totalTime;
	public float runningTime=0f;
	public float delay=0f;
	
	// 0 = start, 1 = end, 2 = current;
	public Vector3[] vector3s = new Vector3[3];
	public Color[] color4s = new Color[3];
	
	
	public float percent;
	public ColorType ct;
	private Component component=null;
	private Hashtable tweenArguments;
	private string type,method;
	private bool isLocal=true;
	public bool IsRect=false;
	private bool useRealTime;
	private GameObject onCompleteTarget, onStartTarget, onUpdateTarget;
	private string onCompleteFunc=null;
	private string onStartFunc=null;
	private string onUpdateFunc=null;
	
	private bool targetGenerated=false;
	private bool started=false;
	
	public enum ColorType{
		sprite,
		mesh,
		spine
	}
	
	public enum EaseType{
		linear,
		easeInBack,
		easeOutBack,
		easeInElastic,
		easeOutElastic,
		easeInOutBack,
		easeInSine,
		easeOutSine,
		easeInOutSine,
	}
	
	public enum TweenType {
		move,
		scale,
		rotate,
		colorfade,
		value
	}
	
	/// <summary>
	/// The type of loop (if any) to use.  
	/// </summary>
	public enum LoopType{
		/// <summary>
		/// Do not loop.
		/// </summary>
		none,
		/// <summary>
		/// Rewind and replay.
		/// </summary>
		loop,
		/// <summary>
		/// Ping pong the animation back and forth.
		/// </summary>
		pingPong
	}
	
	/// <summary>
	/// A collection of baseline presets that iTween needs and utilizes if certain parameters are not provided. 
	/// </summary>
	public static class Defaults{
		public static float time = 1f;
		public static float delay = 0f;	
		public static LoopType loopType = LoopType.none;
		public static EaseType easeType = EaseType.linear;
		public static bool isLocal = true;
		public static bool isRect = false;
		public static Color color = Color.white;
		public static float updateTimePercentage = .05f;
		public static float updateTime = 1f*updateTimePercentage;
		public static bool useRealTime = false; // Added by PressPlay
		public static ColorType colorType = ColorType.sprite;
	}
	
	void Start(){
		started=true;
		// set current;
		if (tweenType == TweenType.colorfade){
			color4s[2] = color4s[0];
		} else {
			vector3s[2] = vector3s[0];
		}
		
		if (tweenType == TweenType.rotate){
			vector3s[1]=new Vector3(clerp(vector3s[0].x,vector3s[1].x,1),clerp(vector3s[0].y,vector3s[1].y,1),clerp(vector3s[0].z,vector3s[1].z,1));
		}
		
		if (easeType == EaseType.easeInBack)
			easeFunc = easeInBack;
		else if (easeType == EaseType.easeOutBack)
			easeFunc = easeOutBack;
		else if (easeType == EaseType.linear)
			easeFunc = easeLinear;
		else if (easeType == EaseType.easeInElastic)
			easeFunc = easeInElastic;
		else if (easeType == EaseType.easeOutElastic)
			easeFunc = easeOutElastic;
		else if (easeType == EaseType.easeInOutBack)
			easeFunc = easeInOutBack;
		else if (easeType == EaseType.easeInOutSine)
			easeFunc = easeInOutSine;
		else if (easeType == EaseType.easeOutSine)
			easeFunc = easeOutSine;
		else if (easeType == EaseType.easeInSine)
			easeFunc = easeInSine;
		
	}
	
	public static void Stop(GameObject target, bool includechildren){
		Stop(target);
		if(includechildren){
			foreach(Transform child in target.transform){
				Stop(child.gameObject,true);
			}			
		}
	}	
	
	public static void Stop(GameObject target){
		Component[] tweens = target.GetComponents<LightTween>();
		foreach (LightTween item in tweens){
			Component.Destroy(item);
		}
	}
	
	public static void Finish(GameObject target){
		var tweens = target.GetComponents<LightTween>();
		foreach (LightTween item in tweens){
			item.FinishTween();
			Component.Destroy(item);
		}
	}
	
	void FinishTween(){
		runningTime = totalTime+delay;
		Update();	
	}
	
	void Update () {
		runningTime += useRealTime?Time.unscaledDeltaTime:Time.deltaTime;
		if (runningTime < delay)
			return;

		percent = (runningTime-delay)/ totalTime;
		if (percent > 1f)
			percent = 1f;
	
		if (!targetGenerated){
			targetGenerated=true;
			
			if (!started) // needed for FinishTween() routine
				Start (); 
			
			if (onStartTarget != null && onStartFunc != null)
				onStartTarget.SendMessage(onStartFunc);
			GenerateTargets();
		}
		
		if (tweenType == TweenType.colorfade){
			color4s[2].r = easeFunc(color4s[0].r, color4s[1].r, percent);
			color4s[2].g = easeFunc(color4s[0].g, color4s[1].g, percent);
			color4s[2].b = easeFunc(color4s[0].b, color4s[1].b, percent);
			color4s[2].a = easeFunc(color4s[0].a, color4s[1].a, percent);
		} else {
			vector3s[2].x = easeFunc(vector3s[0].x, vector3s[1].x, percent);
			vector3s[2].y = easeFunc(vector3s[0].y, vector3s[1].y, percent);
			vector3s[2].z = easeFunc(vector3s[0].z, vector3s[1].z, percent);
		}
		
		if (percent >= 1){
			vector3s[2] = vector3s[1];
			color4s[2] = color4s[1];
		}
		
		if (tweenType == TweenType.move)
			UpdateMove();
		else if (tweenType == TweenType.scale)
			UpdateScale();
		else if (tweenType == TweenType.rotate){
			UpdateRotate();
		} else if (tweenType == TweenType.colorfade){
			UpdateColor();
		} else if (tweenType == TweenType.value){
			//UpdateValue();
		}
		
		if (onUpdateTarget != null && onUpdateFunc != null){
			if (tweenType == TweenType.move)
				onUpdateTarget.SendMessage(onUpdateFunc, vector3s[2]);
			else if (tweenType == TweenType.scale)
				onUpdateTarget.SendMessage(onUpdateFunc, vector3s[2]);
			else if (tweenType == TweenType.rotate){
				onUpdateTarget.SendMessage(onUpdateFunc, vector3s[2]);
			} else if (tweenType == TweenType.colorfade){
				onUpdateTarget.SendMessage(onUpdateFunc, color4s[2]);
			} else if (tweenType == TweenType.value){
				onUpdateTarget.SendMessage(onUpdateFunc, vector3s[2].x);
			}
		}
		
		if (percent >= 1f){
			if (onCompleteTarget != null && onCompleteFunc != null)
				onCompleteTarget.SendMessage(onCompleteFunc);
			
			if (OnComplete != null)
				OnComplete();
			Destroy(this);
		}
	}
	
	private void UpdateMove(){
		if (IsRect){
			gameObject.GetComponent<RectTransform>().anchoredPosition3D = vector3s[2];
		} else {
			if (isLocal)
				gameObject.transform.localPosition = vector3s[2];
			else 
				gameObject.transform.position = vector3s[2];
		}
	}
	
	private void UpdateScale(){
		if (isLocal)
			transform.localScale = vector3s[2];	
	}
	
	private void UpdateColor(){

    if(component == null 
      || (component.GetType() == typeof(SpriteRenderer) && (SpriteRenderer)component == null) 
      || (component.GetType() == typeof(SkeletonAnimation) && (SkeletonAnimation)component == null)) {
      Destroy(this);
      return;
    }

		switch(ct){
		case ColorType.mesh:
			((MeshRenderer)component).materials[0].color = color4s[2];
			break;
		case ColorType.sprite:
			((SpriteRenderer)component).color = color4s[2];
			break;
			#if SPINE
		case ColorType.spine:
			((SkeletonAnimation)component).skeleton.a = color4s[2].a;
			break;
			#endif
		}
	}
	
	private void UpdateRotate(){
		transform.localRotation = Quaternion.Euler(vector3s[2]);
	}
	

	public static void SpriteColorTo(Component obj, Color to, float time){
		SpriteColorTo(obj,to,time,0f,EaseType.linear,null,null);
	}

	public static void SpriteColorTo(Component obj, Color to, float time, float delay){
		SpriteColorTo(obj,to,time,delay,EaseType.linear,null,null);
	}

	public static void SpriteColorTo(Component obj, Color to, float time, float delay, LightTween.EaseType et){
		SpriteColorTo(obj,to,time,delay,et,null,null);
	}

	public static void SpriteColorTo(Component obj, Color to, float time, float delay, LightTween.EaseType et, GameObject evnt, DelegateOnComplete onComplete){
		LightTween lt = obj.gameObject.AddComponent<LightTween>();
		lt.tweenType = TweenType.colorfade;
		lt.totalTime = time;
		lt.delay = delay;
		lt.color4s[1] = to;
		if (obj.GetType() == typeof(SpriteRenderer)){
			lt.ct = ColorType.sprite;
			lt.color4s[0] = ((SpriteRenderer)obj).color;
		}
		else if (obj.GetType() == typeof(MeshRenderer))
    {
      lt.color4s[0] = ((MeshRenderer)obj).materials[0].color;
			lt.ct = ColorType.mesh;
        } else if (obj.GetType() == typeof(SkeletonAnimation))
    {
      Spine.Skeleton skel = ((SkeletonAnimation)obj).skeleton;
			lt.color4s[0] = new Color(skel.r,skel.g,skel.b, skel.a);
			lt.ct = ColorType.spine;
		}

        lt.objectToSendEvent = evnt;
		if (onComplete != null)
			lt.OnComplete += onComplete;
		lt.easeType = et;
		lt.component = obj;
	}
	
	public static void MoveTo(GameObject obj, Vector3 to, float time, float delay, LightTween.EaseType et, bool rect, GameObject evnt, DelegateOnComplete onComplete){
		LightTween lt = obj.AddComponent<LightTween>();
		lt.tweenType = TweenType.move;
		lt.IsRect = rect;
		lt.totalTime = time;
		lt.delay = delay;
		lt.vector3s[1] = to;
		if (!rect) {
			lt.vector3s[0] = obj.transform.localPosition;
		} else { 
			lt.vector3s[0] = obj.GetComponent<RectTransform>().anchoredPosition3D;
		}
		
		lt.objectToSendEvent = evnt;
		if (onComplete != null)
			lt.OnComplete += onComplete;
		lt.easeType = et;
	}
	
	public static void ScaleTo(GameObject obj, Vector3 to, float time, float delay, LightTween.EaseType et){
		ScaleTo(obj,to,time,delay,et,null,null);
	}
	
	public static void ScaleTo(GameObject obj, Vector3 to, float time, float delay, LightTween.EaseType et, GameObject evnt, DelegateOnComplete onComplete){
		LightTween lt = obj.AddComponent<LightTween>();
		lt.tweenType = TweenType.scale;
		lt.totalTime = time;
		lt.delay = delay;
		lt.vector3s[1] = to;
		lt.vector3s[0] = obj.transform.localScale;
		
		lt.objectToSendEvent = evnt;
		if (onComplete != null)
			lt.OnComplete += onComplete;
		lt.easeType = et;
	}
	
	
	
	private float easeLinear(float start, float end, float value){
		return Mathf.Lerp(start, end, value);
	}
	
	private float easeInBack(float start, float end, float value){
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}
	
	private float easeOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}
	
	private float clerp(float start, float end, float value){
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) * 0.5f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half){
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}else if ((end - start) > half){
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}else retval = start + (end - start) * value;
		return retval;
	}
	
	
	private float easeInElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
		}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		
	
	private float easeOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p * 0.25f;
		}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}
	
	private float easeInOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1){
			s *= (1.525f);
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}
	
	private float easeInSine(float start, float end, float value){
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}
	
	private float easeOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	}
	
	private float easeInOutSine(float start, float end, float value){
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}
	
	public static void MoveBy(GameObject target, Hashtable args){
		args = LightTween.CleanArgs(args);
		
		args["type"]="move";
		args["method"]="by";
		Launch(target,args);
	}
	
	public static void MoveTo(GameObject target, Hashtable args){
		args = LightTween.CleanArgs(args);
		
		args["type"]="move";
		args["method"]="to";
		Launch(target,args);
	}
	
	public static void ScaleTo(GameObject target, Hashtable args){
		args = LightTween.CleanArgs(args);
		
		args["type"]="scale";
		args["method"]="to";
		Launch(target,args);
	}
	
	public static void ColorTo(GameObject target, Hashtable args){
		args = LightTween.CleanArgs(args);
		
		args["type"]="color";
		args["method"]="to";
		Launch(target,args);
	}

	public static void ValueTo (GameObject target, Hashtable args)
	{
		args = LightTween.CleanArgs(args);
		
		args["type"]="value";
		args["method"]="to";
		Launch(target,args);
	}
	
	public static Hashtable Hash(params object[] args){
		Hashtable hashTable = new Hashtable(args.Length/2);
		if (args.Length %2 != 0) {
			Debug.LogError("Tween Error: Hash requires an even number of arguments!"); 
			return null;
		}else{
			int i = 0;
			while(i < args.Length - 1) {
				hashTable.Add(args[i], args[i+1]);
				i += 2;
			}
			return hashTable;
		}
	}	
	
	//cast any accidentally supplied doubles and ints as floats as iTween only uses floats internally and unify parameter case:
	static Hashtable CleanArgs(Hashtable args){
		Hashtable argsCopy = new Hashtable(args.Count);
		Hashtable argsCaseUnified = new Hashtable(args.Count);
		
		foreach (DictionaryEntry item in args) {
			argsCopy.Add(item.Key, item.Value);
		}
		
		foreach (DictionaryEntry item in argsCopy) {
			if(item.Value.GetType() == typeof(System.Int32)){
				int original = (int)item.Value;
				float casted = (float)original;
				args[item.Key] = casted;
			}
			if(item.Value.GetType() == typeof(System.Double)){
				double original = (double)item.Value;
				float casted = (float)original;
				args[item.Key] = casted;
			}
		}	
		
		//unify parameter case:
		foreach (DictionaryEntry item in args) {
			argsCaseUnified.Add(item.Key.ToString().ToLower(), item.Value);
		}	
		
		//swap back case unification:
		args = argsCaseUnified;
		
		return args;
	}	
	
	static void Launch(GameObject target, Hashtable args){		
		target.AddComponent<LightTween>().ApplyArgs(args);
	}	
	
	void ApplyArgs(Hashtable args){
		tweenArguments = args;
		type = (string)tweenArguments["type"];
		method = (string)tweenArguments["method"];
		SetupBasics();
		//GenerateTargets();
	}
	
	void SetupBasics(){
		
		if(tweenArguments.Contains("onstart")){
			onStartFunc = (string)tweenArguments["onstart"];
		} 
		
		if(tweenArguments.Contains("onstarttarget")){
			onStartTarget = (GameObject)tweenArguments["onstarttarget"];
			if (onStartFunc == null)
				onStartFunc = "OnStart";
		} else {
			if (onStartFunc != null) {
				onStartTarget = gameObject;
			}
		}
		
		if(tweenArguments.Contains("oncomplete")){
			onCompleteFunc = (string)tweenArguments["oncomplete"];
		} 
		
		if(tweenArguments.Contains("onupdate")){
			onUpdateFunc = (string)tweenArguments["onupdate"];
		} 
		
		if(tweenArguments.Contains("onupdatetarget")){
			onUpdateTarget = (GameObject)tweenArguments["onupdatetarget"];
			if (onUpdateFunc == null)
				onUpdateFunc = "OnUpdate";
		} else {
			if (onUpdateFunc != null) {
				onUpdateTarget = gameObject;
			}
		}
		
		if(tweenArguments.Contains("oncompletetarget")){
			onCompleteTarget = (GameObject)tweenArguments["oncompletetarget"];
			if (onCompleteFunc == null)
				onCompleteFunc = "OnComplete";
		} else {
			if (onCompleteFunc != null) {
				onCompleteTarget = gameObject;
			}
		}
		
		if(tweenArguments.Contains("easetype")){
			easeType = (EaseType)tweenArguments["easetype"];
		} else {
			easeType = Defaults.easeType;
		}
		
		if(tweenArguments.Contains("time")){
			totalTime = (float)tweenArguments["time"];
		} else {
			totalTime = Defaults.time;
		}
		
		if(tweenArguments.Contains("delay")){
			delay = (float)tweenArguments["delay"];
		} else {
			delay = Defaults.delay;
		}
		
		if(tweenArguments.Contains("islocal")){
			isLocal = (bool)tweenArguments["islocal"];
		}else{
			isLocal = Defaults.isLocal;
		}
		
		if (tweenArguments.Contains("ignoretimescale"))
			useRealTime = (bool)tweenArguments["ignoretimescale"];
		else
			useRealTime = Defaults.useRealTime;
	}
	
	void GenerateTargets(){
		switch (type) {
		case "scale":
			tweenType = TweenType.scale;
			switch(method){
			case "to":
				GenerateScaleToTargets();
				break;
			}
			break;
		case "color":
			tweenType = TweenType.colorfade;
			switch(method){
			case "to":
				GenerateColorToTargets();
				break;
			}
			break;
		case "value":
			tweenType = TweenType.value;
			switch(method){
			case "to":
				GenerateValueToTargets();
				break;
			}
			break;
		case "move":
			tweenType = TweenType.move;
			switch(method){
			case "to":
				GenerateMoveToTargets();
				break;
			case "by":
				GenerateMoveByTargets();
				break;
			}
			break;
		}
	}
	
	void GenerateColorToTargets(){
		color4s = new Color[3];
		if (tweenArguments.ContainsKey("colortype")){
			ct = (ColorType)tweenArguments["colortype"];
		} else {
			ct = Defaults.colorType;
		}
		
		switch(ct){
		case ColorType.sprite:
			component = GetComponent<SpriteRenderer>();
			color4s[0] = color4s[1] = (component as SpriteRenderer).color;
			break;
		case ColorType.mesh:
			component = GetComponent<MeshRenderer>();
			color4s[0] = color4s[1] = (component as MeshRenderer).materials[0].color;
			break;
			#if SPINE
		case ColorType.spine:
			component = GetComponent<SkeletonAnimation>();
			Spine.Skeleton skel = (component as SkeletonAnimation).skeleton;
			color4s[0] = color4s[1] = new Color(skel.r,skel.g,skel.b, skel.a);
			break;
			#endif
		}	
		
		if (tweenArguments.Contains("color")) {
			color4s[1] = (Color)tweenArguments["color"];
		} else {
			if (tweenArguments.Contains("a")) {
				color4s[1].a=(float)tweenArguments["a"];
			}
			if (tweenArguments.Contains("r")) {
				color4s[1].r=(float)tweenArguments["r"];
			}
			if (tweenArguments.Contains("g")) {
				color4s[1].g=(float)tweenArguments["g"];
			}
			if (tweenArguments.Contains("b")) {
				color4s[1].b=(float)tweenArguments["b"];
			}
		}
	}
	
	void GenerateMoveToTargets(){
		
		if (tweenArguments.Contains("isrect"))
			IsRect = (bool)tweenArguments["isrect"];
		else
			IsRect = Defaults.isRect;
		
		vector3s=new Vector3[3];
		
		if (IsRect){
			RectTransform rt = GetComponent<RectTransform>();
			vector3s[0]=vector3s[1]=rt.anchoredPosition3D; 
		} else if (isLocal)
			vector3s[0]=vector3s[1]=gameObject.transform.localPosition;	
		else 
			vector3s[0]=vector3s[1]=gameObject.transform.position;				
		
		
		if (tweenArguments.Contains("position")) {
			if (tweenArguments["position"].GetType() == typeof(Transform)){
				Transform trans = (Transform)tweenArguments["position"];
				if (IsRect){
					RectTransform rt = trans.GetComponent<RectTransform>();
					vector3s[1]= rt.anchoredPosition3D;
				} if (isLocal)
					vector3s[1]=trans.localPosition;					
				else
					vector3s[1]=trans.position;					
			}else if(tweenArguments["position"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)tweenArguments["position"];
			}
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		} 
	}
	
	
	void GenerateMoveByTargets(){
		
		if (tweenArguments.Contains("isrect"))
			IsRect = (bool)tweenArguments["isrect"];
		else
			IsRect = Defaults.isRect;
		
		vector3s=new Vector3[3];
		
		if (IsRect){
			RectTransform rt = GetComponent<RectTransform>();
			vector3s[0]=vector3s[1]= new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0);	
		} else if (isLocal)
			vector3s[0]=vector3s[1]=gameObject.transform.localPosition;	
		else 
			vector3s[0]=vector3s[1]=gameObject.transform.position;				
		
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=vector3s[0] + (Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=vector3s[0].x + (float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=vector3s[0].y +(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=vector3s[0].z +(float)tweenArguments["z"];
			}
		}
	}
	
	void GenerateScaleToTargets(){
		vector3s=new Vector3[3];
		
		if (isLocal)
			vector3s[0]=vector3s[1]=gameObject.transform.localScale;	
		else 
			vector3s[0]=vector3s[1]=gameObject.transform.lossyScale;					
		
		if (tweenArguments.Contains("scale")) {
			if (tweenArguments["scale"].GetType() == typeof(Transform)){
				Transform trans = (Transform)tweenArguments["scale"];
				if (isLocal)
					vector3s[1]=trans.localScale;					
				else
					vector3s[1]=trans.lossyScale;					
			}else if(tweenArguments["scale"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)tweenArguments["scale"];
			}
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		} 
	}
	
	void GenerateValueToTargets(){
		vector3s=new Vector3[3];
		
		vector3s[0].x=vector3s[1].x=(float)tweenArguments["valuefrom"];
		vector3s[1].x=(float)tweenArguments["valueto"];

	}
	
}
