using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatAgent : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    public static ChatAgent Instance;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField]private ChatSetting m_ChatSettings;
    /// <summary>
    /// ��ʾ��������
    /// </summary>
    //[Header("�趨AI��ʾ������")]
    //[SerializeField] private string m_AiName = "����";
    #region ui
    /// <summary>
    /// ����UI��
    /// </summary>
    [SerializeField] private GameObject m_ChatPanel;
    /// <summary>
    /// AI������
    /// </summary>
    //[SerializeField] private Text m_AINameText;
    /// <summary>
    /// �������Ϣ
    /// </summary>
    [SerializeField] public InputField m_InputWord;
    /// <summary>
    /// ���ص���Ϣ
    /// </summary>
    [SerializeField] private Text m_TextBack;
    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField]private AudioSource m_AudioSource;
    /// <summary>
    /// ������Ϣ��ť
    /// </summary>
    [SerializeField] private Button m_CommitMsgBtn;

    #endregion

    private void Awake()
    {
        Instance = this;
        m_CommitMsgBtn.onClick.AddListener(delegate { SendData(); });
        RegistButtonEvent();
        //m_AINameText.text = m_AiName + ":";

    }

    #region ��Ϣ����

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public void SendData()
    {
        if (m_InputWord.text.Equals(""))
            return;

        //��Ӽ�¼����
        m_ChatHistory.Add(m_InputWord.text);
        //��ʾ��
        string _msg = m_InputWord.text;

        //��������
        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        m_InputWord.text = "";
        m_TextBack.text = "����˼����...";
    }
    /// <summary>
    /// �����ַ���
    /// </summary>
    /// <param name="_postWord"></param>
    public void SendData(string _postWord)
    {
        if (_postWord.Equals(""))
            return;

        //��Ӽ�¼����
        m_ChatHistory.Add(_postWord);
        //��ʾ��
        string _msg = _postWord;

        //��������
        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        m_InputWord.text = "";
        m_TextBack.text = "����˼����...";
    }

    /// <summary>
    /// AI�ظ�����Ϣ�Ļص�
    /// </summary>
    /// <param name="_response"></param>
    private void CallBack(string _response)
    {
        _response = _response.Trim();
        m_TextBack.text = "";


        //��¼����
        m_ChatHistory.Add(_response);

        m_ChatSettings.m_TextToSpeech.Speak(_response, PlayVoice);
    }

    #endregion

    #region ��������
    /// <summary>
    /// ����ʶ�𷵻ص��ı��Ƿ�ֱ�ӷ�����LLM
    /// </summary>
    [SerializeField] private bool m_AutoSend = true;
    /// <summary>
    /// ��������İ�ť
    /// </summary>
    [SerializeField] private Button m_VoiceInputBotton;
    /// <summary>
    /// ¼������ʾ��Ϣ
    /// </summary>
    [SerializeField] private Text m_RecordTips;
    /// <summary>
    /// �������봦����
    /// </summary>
    [SerializeField] private VoiceInputs m_VoiceInputs;
    /// <summary>
    /// ע�ᰴť�¼�
    /// </summary>
    private void RegistButtonEvent()
    {
        if (m_VoiceInputBotton == null || m_VoiceInputBotton.GetComponent<EventTrigger>())
            return;

        EventTrigger _trigger= m_VoiceInputBotton.gameObject.AddComponent<EventTrigger>();

        //��Ӱ�ť���µ��¼�
        EventTrigger.Entry _pointDown_entry = new EventTrigger.Entry();
        _pointDown_entry.eventID=EventTriggerType.PointerDown;
        _pointDown_entry.callback = new EventTrigger.TriggerEvent();

        //��Ӱ�ť�ɿ��¼�
        EventTrigger.Entry _pointUp_entry = new EventTrigger.Entry();
        _pointUp_entry.eventID = EventTriggerType.PointerUp;
        _pointUp_entry.callback = new EventTrigger.TriggerEvent();

        //���ί���¼�
        _pointDown_entry.callback.AddListener(delegate { StartRecord(); });
        _pointUp_entry.callback.AddListener(delegate { StopRecord(); });

        _trigger.triggers.Add(_pointDown_entry);
        _trigger.triggers.Add(_pointUp_entry);
    }

    /// <summary>
    /// ��ʼ¼��
    /// </summary>
    public void StartRecord()
    {
        m_RecordTips.text = "����¼����...";
        m_VoiceInputs.StartRecordAudio();
    }
    /// <summary>
    /// ����¼��
    /// </summary>
    public void StopRecord()
    {
        m_RecordTips.text = "¼������������ʶ��...";
        m_VoiceInputs.StopRecordAudio(AcceptClip);
    }

    /// <summary>
    /// ����¼�Ƶ���Ƶ����
    /// </summary>
    /// <param name="_data"></param>
    private void AcceptData(byte[] _data)
    {
        m_ChatSettings.m_SpeechToText.SpeechToText(_data, DealingTextCallback);
    }

    /// <summary>
    /// ����¼�Ƶ���Ƶ����
    /// </summary>
    /// <param name="_data"></param>
    private void AcceptClip(AudioClip _audioClip)
    {
        m_ChatSettings.m_SpeechToText.SpeechToText(_audioClip, DealingTextCallback);
    }
    /// <summary>
    /// ����ʶ�𵽵��ı�
    /// </summary>
    /// <param name="_msg"></param>
    private void DealingTextCallback(string _msg)
    {
        m_RecordTips.text = _msg;
        StartCoroutine(SetTextVisible(m_RecordTips));
        //�Զ�����
        if (m_AutoSend)
        {
            SendData(_msg);
            return;
        }

        m_InputWord.text = _msg;
    }

    private IEnumerator SetTextVisible(Text _textbox)
    {
        yield return new WaitForSeconds(3f);
        _textbox.text = "";
    }

    #endregion


    #region �����ϳ�

    private void PlayVoice(AudioClip _clip,string _response)
    {
        m_AudioSource.clip = _clip;
        m_AudioSource.Play();
        Debug.Log("��Ƶʱ����" + _clip.length);
        //��ʼ�����ʾ���ص��ı�
        m_WriteState = true;
        StartCoroutine(SetTextPerWord(_response));
    }

    #endregion

    #region ���������ʾ
    //������ʾ��ʱ����
    [SerializeField] private float m_WordWaitTime = 0.2f;
    //�Ƿ���ʾ���
    [SerializeField] private bool m_WriteState = false;
    private IEnumerator SetTextPerWord(string _msg)
    {
        int currentPos = 0;
        while (m_WriteState)
        {
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //������ʾ������
            m_TextBack.text = _msg.Substring(0, currentPos);

            m_WriteState = currentPos < _msg.Length;

        }
    }

    #endregion

    #region �����¼
    //���������¼
    [SerializeField] private List<string> m_ChatHistory;
    //�����Ѵ�������������
    [SerializeField] private List<GameObject> m_TempChatBox;
    //�����¼��ʾ��
    [SerializeField] private GameObject m_HistoryPanel;
    //�����ı����õĲ�
    [SerializeField] private RectTransform m_rootTrans;
    //������������
    [SerializeField] private ChatPrefab m_PostChatPrefab;
    //�ظ�����������
    [SerializeField] private ChatPrefab m_RobotChatPrefab;
    //������
    [SerializeField] private ScrollRect m_ScroTectObject;
    //��ȡ�����¼
    public void OpenAndGetHistory()
    {
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }
    //����
    public void BackChatMode()
    {
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    //����Ѵ����ĶԻ���
    private void ClearChatBox()
    {
        while (m_TempChatBox.Count != 0)
        {
            if (m_TempChatBox[0])
            {
                Destroy(m_TempChatBox[0].gameObject);
                m_TempChatBox.RemoveAt(0);
            }
        }
        m_TempChatBox.Clear();
    }

    //��ȡ�����¼�б�
    private IEnumerator GetHistoryChatInfo()
    {

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < m_ChatHistory.Count; i++)
        {
            if (i % 2 == 0)
            {
                ChatPrefab _sendChat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
                _sendChat.SetText(m_ChatHistory[i]);
                m_TempChatBox.Add(_sendChat.gameObject);
                continue;
            }

            ChatPrefab _reChat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
            _reChat.SetText(m_ChatHistory[i]);
            m_TempChatBox.Add(_reChat.gameObject);
        }

        //���¼��������ߴ�
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        //�������������Ϣ
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }


    #endregion


}
