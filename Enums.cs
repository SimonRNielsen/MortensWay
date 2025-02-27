namespace MortensWay
{
    /// <summary>
    /// Base enum for objects like mouse or background etc.
    /// </summary>
    public enum LogicItems
    {

        MousePointer,
        CollisionPixel,
        SelectionBox,
        Button

    }


    public enum TileTypes
    {

        TowerKey, //Needing keys to open
        TowerPortion, //Need to deliver the portion to
        Portal,
        Key,
        Forest,
        Fence,
        Path,
        Grass,
        FencePath,
        Stone
    }

    public enum MortensEnum
    {

        Bishop = 11 //Set to 11, toavoid confusion when using gameObjects.Find on enums. 

    }

    public enum Monstre
    {

        Goose

    }

    public enum AlgorithmType
    {
        BFS,
        AStat,
        DFS
    }
    public enum SoundEffects
    {

        GooseHonk

    }


    public enum Music
    {



    }
}
