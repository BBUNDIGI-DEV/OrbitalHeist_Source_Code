using System;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;

    public static class CSVParser
    {
        public static T[] ParseCSV<T>(string fullCSVString) where T : new()
        {
            string[] lineString = fullCSVString.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] nameString = lineString[0].Split(',');
            int[] nameToFieldIndex = new int[nameString.Length];

            FieldInfo[] fieldInfos = typeof(T).GetFields();
            Debug.Assert(nameString.Length == fieldInfos.Length,
                "Error!! Amount of field is  not equal to amount of name string " +
                $" nameString.Length : {nameString.Length}" +
                $" fieldInfos.Length : {fieldInfos.Length}");

            for (int i = 0; i < nameString.Length; i++)
            {
                bool isSearched = false;
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    if (nameString[i] == fieldInfos[j].Name)
                    {
                        nameToFieldIndex[i] = j;
                        isSearched = true;
                        break;
                    }
                }
                Debug.Assert(isSearched,
                    "Name Is Not Founded!! " +
                    $"Index : {i}, Name : {nameString[i]}");
            }

            T[] resultArr = new T[lineString.Length - 1];
            for (int i = 1; i < lineString.Length; i++)
            {
                resultArr[i - 1] = new T();
                string[] currentDatas = lineString[i].Split(',');

                for (int j = 0; j < currentDatas.Length; j++)
                {
                    int fieldIndex = nameToFieldIndex[j];
                    Type currentFieldType = fieldInfos[fieldIndex].FieldType;
                    if (currentDatas[j] == string.Empty)
                    {
                        continue;
                    }

                    if (currentFieldType.IsArray)
                    {
                        Type elementType = currentFieldType.GetElementType();
                        if (elementType.IsArray)
                        {
                            Type elementType2 = elementType.GetElementType();
                            string[] arrayElementDatas = currentDatas[j].Split('|');
                            Array data2DArray = Array.CreateInstance(elementType, arrayElementDatas.Length);
                            for (int k = 0; k < arrayElementDatas.Length; k++)
                            {
                                string[] arrayDatas = arrayElementDatas[k].Split(' ');

                                //fieldInfos[fieldIndex].SetValue(resultArr[i], )
                                Array dataArray = Array.CreateInstance(elementType2, arrayDatas.Length);
                                for (int l = 0; l < dataArray.Length; l++)
                                {
                                    var elementConverter = TypeDescriptor.GetConverter(elementType2).ConvertFromString(arrayDatas[l]);
                                    dataArray.SetValue(Convert.ChangeType(elementConverter, elementType2), l);
                                }
                                data2DArray.SetValue(dataArray, k);
                            }
                            fieldInfos[fieldIndex].SetValue(resultArr[i - 1], data2DArray);
                        }
                        else
                        {
                            string[] arrayDatas = currentDatas[j].Split(' ');

                            //fieldInfos[fieldIndex].SetValue(resultArr[i], )
                            Array dataArray = Array.CreateInstance(elementType, arrayDatas.Length);
                            for (int k = 0; k < dataArray.Length; k++)
                            {
                                var elementConverter = TypeDescriptor.GetConverter(elementType).ConvertFromString(arrayDatas[k]);
                                dataArray.SetValue(Convert.ChangeType(elementConverter, elementType), k);
                            }
                            fieldInfos[fieldIndex].SetValue(resultArr[i - 1], dataArray);
                        }
                    }
                    else// Value Types
                    {
                        var converter = TypeDescriptor.GetConverter(currentFieldType).ConvertFromString(currentDatas[j]);
                        fieldInfos[fieldIndex].SetValue(resultArr[i - 1], Convert.ChangeType(converter, currentFieldType));
                    }
                }
            }
            return resultArr;
        }
    }

