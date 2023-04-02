namespace PrivateKeyShifter.Service.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] RightShift(this byte[] array)
    {
        // the last element (because we're skipping all but one)... then all but the last one.
        // return array.Skip(array.Length - 1).Concat(array.Take(array.Length - 1)).ToArray();

        var tempo = array[0];
        for(var i=0; i<array.Length-1; i++)
        {
            var yolo = array[i + 1];
            array[i + 1] = tempo;
            tempo = yolo;
        }
        array[0] = tempo;
        return array;
    }        

    public static byte[] LeftShift(this byte[] array)
    {
        var tempo = array[array.Length - 1];
        for (var i = array.Length - 1; i >0; i--)
        {
            var yolo = array[i - 1];
            array[i -1] = tempo;
            tempo = yolo;
        }
        array[array.Length - 1] = tempo;
        return array;
    }
    }
}