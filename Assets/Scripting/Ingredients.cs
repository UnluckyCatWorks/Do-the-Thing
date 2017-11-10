using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Ingredients 
{
	NONE=0,
	Flower=2,
	Soap=4,
	Cactus=8,
	Uranium_Juice=16,
	Quantic_Pears=32,
	Anti_Matter=64,
	Destiled_Water=128,
	// Dispensadores
	Substance_X=256,
	Orange_Juice=512,
	Unicorn_Blood=1024,
	Gnome_Sweat=2048
}
