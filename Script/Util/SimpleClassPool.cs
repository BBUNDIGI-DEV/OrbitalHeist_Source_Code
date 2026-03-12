using System.Collections.Generic;

public class SimpleClassPool<T> where T : IClassPoolBase, new()
{
    private List<T> mObjectList;

    public T GetObject()
    {
        for (int i = 0; i < mObjectList.Count; i++)
        {
            if(!mObjectList[i].GetIsActivate())
            {
                return mObjectList[i];
            }
        }
        T newObject = new T();
        mObjectList.Add(new T());
        return newObject;
    }
}

public interface IClassPoolBase
{
    public abstract bool GetIsActivate();
}