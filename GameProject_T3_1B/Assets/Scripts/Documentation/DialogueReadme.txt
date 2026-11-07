Tutorial Dialogue System

1. 기능 설명

게임 시작 시 튜토리얼 대화창이 자동으로 활성화되며, 미리 등록된 여러
안내 문장을 순서대로 출력합니다.

-   게임 시작 시 튜토리얼 자동 실행
-   설정된 시간 간격에 따라 다음 문장 출력
-   모든 문장 출력 후 마지막 문장 유지
-   다른 스크립트에서 튜토리얼 시작 및 종료 가능

2. 문장 저장 자료구조

튜토리얼 문장은 string[] 배열(Array) 자료구조를 사용하여 관리합니다.

코드: [SerializeField] private string[] tutorialMessages;

배열은 여러 개의 문자열을 하나의 변수에 저장할 수 있으며, 각 문장은
인덱스 번호로 구분합니다.

예시: - tutorialMessages[0] = “WASD로 움직일 수 있습니다.” -
tutorialMessages[1] = “E를 눌러 인벤토리를 열어보세요.” -
tutorialMessages[2] = “마우스 좌클릭으로 상호작용을 할 수 있습니다.”

3. 여러 문장 등록 방법

Unity Inspector에서 TutorialDialog 컴포넌트의 Tutorial Messages 항목을
확인합니다.

1)  Size에 등록할 문장의 개수를 입력합니다.
2)  생성된 Element 0, Element 1, Element 2 등에 문장을 입력합니다.
3)  문장을 추가하려면 Size 값을 늘립니다.

예를 들어 Size를 3으로 설정하면 총 3개의 문장을 등록할 수 있습니다.

4. 문장 출력 방식

for 반복문을 사용하여 배열에 저장된 문장을 순서대로 출력합니다.

코드: for (int i = 0; i < tutorialMessages.Length; i++) {
dialogueText.text = tutorialMessages[i]; yield return new
WaitForSeconds(tutorialInterval); }

-   tutorialMessages.Length: 배열에 등록된 문장의 전체 개수
-   tutorialMessages[i]: 현재 순서의 문장
-   dialogueText.text: 화면에 표시할 텍스트
-   WaitForSeconds(tutorialInterval): 다음 문장으로 넘어가기 전 대기

기본 출력 간격은 5초이며, Inspector의 Tutorial Interval 값으로 변경할 수
있습니다.

5. 주요 변수 및 함수

-   IsTutorialPlaying: 튜토리얼 진행 여부를 확인합니다.
-   CurrentMessageIndex: 현재 출력 중인 문장의 배열 인덱스를 확인합니다.
-   StartTutorial(): 튜토리얼을 시작합니다.
-   StopTutorial(): 튜토리얼을 중단하고 대화창을 닫습니다.

6. UI 연결 방법

Unity Inspector에서 TutorialDialog 컴포넌트에 다음 UI 오브젝트를
연결합니다.

-   Dialogue Panel: 튜토리얼 전용 대화창 전체 오브젝트
-   Dialogue Text: 안내 문장이 표시될 TextMeshPro UI 오브젝트

기존 NPC 대화창과 별도의 튜토리얼 UI를 연결하면 두 시스템이 서로의
텍스트나 활성화 상태를 덮어쓰는 상황을 줄일 수 있습니다.

7. 참고 사항

현재 스크립트는 마지막 문장까지 출력한 뒤에도 대화창을 유지합니다.
튜토리얼을 중간에 종료하려면 다른 스크립트에서 TutorialDialog 컴포넌트의
StopTutorial() 함수를 호출할 수 있습니다.
