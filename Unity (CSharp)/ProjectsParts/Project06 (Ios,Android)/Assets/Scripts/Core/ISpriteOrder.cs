using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface ISpriteOrder {

	int spriteOrderValue { get; set; }

	void SetSpriteOrder(int spriteOrder);

}
