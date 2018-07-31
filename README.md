# galaxy-shooter-ml
This is galaxy shooter version that includes machine learning for the game. 

- For the base game check out [the master branch](https://github.com/MaciejWanat/galaxy-shooter-ml/tree/master).
- For version that includes machine learning on single enemy, check out [the machine-learning branch](https://github.com/MaciejWanat/galaxy-shooter-ml/tree/machine-learning).

This is heavily modded version, adjusted for the machine learning purposes. It's based on neural networks. Currently goal realized by program is shooting multiple enemies approaching. Difficulty raises as the the player is playing (enemies are getting spawned quicker). Power-ups are disabled.

If you are interested in older version with different goals, look through past commits:
- e5452ff - avoiding collisions with enemy (respawned enemy)
- 187dd1e - avoiding collisions with enemy (relocated enemy)
- 2545307 - shooting enemies approaching
- dd5a624 - multiple enemies. Enemy represented as a point in 2D grid.
- cc15f78 - multiple enemies. Extended data input.

For more informations about machine learning in Unity, check out [Unity ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents) which this project is based on.
