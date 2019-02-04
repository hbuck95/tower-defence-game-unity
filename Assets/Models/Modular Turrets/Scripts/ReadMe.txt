There are a few turrets ready to drop and use from the Prefab/Turrets folder.
All turret parts are under Prefab/TurretParts.
The projectiles are under Prefab/Projectiles
The animations for muzzle flash and explosions are under Prefab/animations.

The turrets use a aim_pan / aim_tilt mechanism to animate, each base has an aim_pan empty which has a child aim_tilt. Do not parent any Items into these empty's
they are purely for Lerping from current to new target position. Each base has a Weapon_pan with a Weapon_tilt empty these are where you can place the children for panning
and tilting. The guns all have muzzle position empties this is where the projectile and muzzle flash will be instantiated. You can set them by dragging the relevant animation 
onto the slot of the relevant turret script.

The turrets have editable values for reload time, turn speed, fire pause time.
Projectiles have editable values for range - before projectile self destructs, speed - of projectile and damage inflicted on the target

The Texture for the turrets is created with a substance. This allows you to create copies and change values i.e. The two main colors, amount of dirt, amount of wear and tear.
This lets you create unique textures from within the unity editor directly.