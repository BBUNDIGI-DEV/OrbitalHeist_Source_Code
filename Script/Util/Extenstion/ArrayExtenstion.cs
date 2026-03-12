public static class ArrayExtenstion
{
    public static int FindCloseIndex<T>(this T[] descendingArray, T checkValue) where T : System.IComparable
    {
        int index = 0;
        for (int i = 0; i < descendingArray.Length; i++)
        {
            T value = descendingArray[i];
            int checkResult = value.CompareTo(checkValue);
            if (value.CompareTo(checkValue) == 1 || value.CompareTo(checkValue) == 0)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public static bool IsAscendingSorted<T>(this T[] array) where T : System.IComparable
    {
        if (array == null)
        {
            return false;
        }
        if (array.Length <= 1)
        {
            return true;
        }
        bool isSorted = true;


        for (int i = 1; i < array.Length; i++)
        {
            T prevValue = array[i - 1];
            T value = array[i];
            if (prevValue.CompareTo(value) == -1)
            {
                isSorted = false;
                break;
            }
        }
        return isSorted;
    }

    public static bool IsDescendingSorted<T>(this T[] array) where T : System.IComparable
    {
        if (array == null)
        {
            return false;
        }
        if (array.Length <= 1)
        {
            return true;
        }

        bool isSorted = true;

        for (int i = 1; i < array.Length; i++)
        {
            T prevValue = array[i - 1];
            T value = array[i];
            if (prevValue.CompareTo(value) == 1)
            {
                isSorted = false;
                break;
            }
        }
        return isSorted;
    }
}
