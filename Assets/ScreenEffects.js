#pragma strict

public var vignetting: Vignetting;
public var noise: NoiseEffect;
public var wakeUpSpeed = 150.0f;
public var game: GameController;
var moves = 0;

function Update () {
  var delta = Time.deltaTime;

vignetting.intensity = Mathf.Max(0, vignetting.intensity - delta * wakeUpSpeed);
vignetting.chromaticAberration = Mathf.Max(0, vignetting.chromaticAberration - delta * wakeUpSpeed * 1.5f);

if (game.cubeColorController.stats.moves < moves) moves = 0;

if (moves < game.cubeColorController.stats.moves) {
  ++moves;
  vignetting.chromaticAberration += 300.0f;
}

}