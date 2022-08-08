# Tree-Grass-Grid-Generator



<img width="267" alt="image" src="https://user-images.githubusercontent.com/33725845/183426182-eeedcad4-13d0-4caa-9542-afb85cfa34f0.png">



A simple grid generator with the following rules:
- Grid must have two objects, in this case tree and grass.
- The width and height of the grid must be an even number size eg. 6x8, 10x10
- In each row and column there must not be more than a run of 2 of the same object. Diagonals are not a concern.
- The amount of each type must be equal in each row and in each column. Thus if the grid is 6 wide, then three trees and three grass must exist in every one.
- If the height is different from the width, that column will have a different amount compared to the width, but it still must have an equal number of grass and trees.
