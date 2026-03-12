`목차`

---

참고 자료
https://docs.popekim.com/ko/coding-standards/pocu-csharp

# 주석 규칙

- 주석은  자주 사용하지 않는것을 기본으로 하며 반드시 필요한곳에만 사용하여 주석의 중요성을 높힌다.
- 모든 프로그래머는 주석 없이 코드를 읽어도 리뷰하기 편한 코드를 작성하기 위해 노력해야 한다.
- `복잡`하거나 사용이 `까다로운 함수`라면 summary 등을 사용한다.

```csharp
/// <summary> 플레이어 매니저의 세팅을 다시 설정한다. </summary>
/// <param name="settings"> 플레이어 세팅 </param>
public void SetPlayerManagerSetting(Settings settings) {}
```

---

- 클래스와 구조체의 이름은 파스칼 표기법을 따른다.

```csharp
class PlayerManager;
struct PlayerData;
```

- 지역 변수 및 함수의 매개 변수는 카멜 표기법을 따른다.

```csharp
public void DoSomething(int anythingIntYouWant)
{
    int localSomeNumber;
    int localID;
}
```

- 메서드 이름은 기본적으로 동사(명령형)+명사(목적어)의 형태로 짓는다.

```csharp
public uint GetAge()
{
// 함수 구현부...}
}
```

- Boolean 상태를 반환하는 경우 Is, Can, Has, Should 접미를 적극적으로 활용한다. 다만 반드시 지킬 필요는 없다.

```csharp
public bool IsAlive(Person person);
public bool HasChild(Person person);
public bool CanAccept(Person person);
public bool ShouldDelete(Person person);
public bool Exists(Person person);
```

- private 메서드는 카멜 표기법을 따른다.

```csharp
private bool isAlive(Person person);
private bool hasChild(Person person);
private bool canAccept(Person person);
private bool shouldDelete(Person person);
private bool exists(Person person);
```

- 상수의 이름은 모두 대문자로 하고 띄어쓰기를 밑줄로 표기한다.

```csharp
const int SOME_CONSTANT = 1;
```

- private 맴버변수의 경우 m(변수명)의 형태를 따른다.

```csharp
	private int mHP;
```

- [SerializeField]를 통해 유니티 인스펙터 상에서 초기화되는 변수는 앞에 sf를 붙인 형태의 카멜 케이스를 따른다. SerializeField는 기본적으로 protected, private 에 만 유효한 옵션임으로 7번규칙을 생략한다. 기본적으로 SerializeField 데이터는 런타임에서 직접적으로 수정하지 않도록 하며 이를 컴파일러에서 강제하지 못한다는것을 인지하고 신경쓰도록 한다.

```csharp
[SerializeField] private float sfSpawnWidth;
```

- 인터페이스를 선언할 때는 앞에 i를 붙인다

```csharp
 interface iSomeInterface;
```

- 열거형을 선언할 때는 앞에 e 를 붙인다

```csharp
 public enum eDirection
 {
     North,
     South
 }
```

- 구조체를 선언할 때는 앞에 s 를 붙인다

```csharp
 public struct sTempStruct
 {
 }
```

- 지역 변수 선언후 해당 지역 변수를 사용하는 코드와 최대한 인접하게 하는것을 원칙으로 한다.
switch문에서 default: 케이스가 실행되지 않을경우 Debug.LogError(”Default Switch Detected”)를 추가한다. 이는 의도치않은 Default 호출을 개발 단계에서 방지하기 위함이다.

```csharp
 switch (type)
 {
     case 1:
         ... 
         break;
     default:
         Debug.LogError($”Default Switch Detected {type}”);
         break;
 }
```

- 코드를 작성할때 사용한 예외케이스엔 반드시 Debug.Assert()를 항상 사용한다.
재귀 함수는 이름 뒤에 Recursive를 붙인다.
클래스를 구성할때 다음 순서를 되도록 따르도록 한다. 만일 해당하지 않은 경우엔 ‘비슷한 속성’을 기준으로 적절한 위치에 추가하도록 한다.

<aside>

1. 중첩 클래스
2. 내부 구조체
3. Public 맴버 변수 
→ 상수 및  정적 변수, Readonly, SerializeField 변수, 일반 순서로 나열
4. Private  맴버 변수

    → 상수 및  정적 변수, Readonly, SerializeField 변수, 일반 순서로 나열
    
5. 정적 메서드
6. 생산자 소멸자
7. public abstract 메서드 (추상 클래스일 경우)
8. *Public* 메서드
9. *Protected 메서드*
10. private abstract 메서드 (추상 클래스일 경우)
11. *Private* 메서드
</aside>

MonoBehavior을 상속받는 클래스의 경우 다음 함수들을 5번 뒤에 순서대로 배치한다. 추가로 해당 함수는 가능한 private과 함께 사용한다. 그 외의 함수들은 3~4번 사이에 순서와 상관없이 배치한다.

<aside>

1. Awake
2. OnEnabled
3. Start
4. OnDisable
5. OnDestory

</aside>

- 클래스는 각각 독립된 소스 파일에 있어야 한다. 단, 작은 클래스 몇 개를 한 파일 안에 같이 넣어두는 것이 상식적일 경우 예외를 허용한다.

- 특정 조건이 반드시 충족되어야 한다고 가정(assertion)하고 짠 코드 모든 곳에 `assert`를 사용한다. `assert` 는 빌드전 개발 과정에서 예외없이 잡혀야할 조건들이다.

- `var` 키워드를 사용하지 않으려 노력한다. 단, 대입문의 우항에서 데이터형이 명확하게 드러나는 경우에만 허용한다.  `new` 키워드를 통해 어떤 개체가 생성되는지 알 수 있는 등이 허용되는 경우의 좋은 예이다.

```csharp
 var employee = new Employee();
```

- 외부로부터 들어오는 데이터의 유효성은 외부/내부 경계가 바뀌는 곳에서 검증(validate)하고 문제가 있을 경우 내부 함수로 전달하기 전에 반환해 버린다. 이는 경계를 넘어 내부로 들어온 모든 데이터는 유효하다고 가정한다는 뜻이다.

`null` 값을 허용하는 매개변수를 사용할 경우 변수명 뒤에 `OrNull`를 붙인다

```csharp
public void SetDetectedObject(InteractableBase baseObjectOrNull)
{
}
```

- 함수에서 `null`을 반환할 때는 함수 이름 뒤에 `OrNull`을 붙인다.

```
 public string GetNameOrNull();
```

- 만일 함수가 단 한번이라도 UnityEvent를 통해 엔진상에서 호출되는 함수라면, 함수의 마지막 부분에 _UE를 붙인다. (UE는 UnityEvent의 약자) 예시로는 Button, Animation Event 등이 있다.

```csharp
public void OnPlayerDeadAnimEnd_UE()
```
