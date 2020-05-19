using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class LeftPanel : BattleUIBase
    {
        public Text nameDisplayer;
        public Image iconDisplayer;

        public Image career;
        public Image type;

        public InfoPanel infoPanel;

        [Header("Prefabs")]
        public GameObject armyInfoPrefab;
        public GameObject buildingInfoPrefab;

        public OnMapEntity LastEntity { get; set; }
        public OnMapEntity SelectingEntity { get; set; }


        public override void Awake()
        {
            isShown = false;
            originalPos = transform.position;
            SetPanelActive(true);
        }

        public virtual void Start()
        {
            Close();
        }
        // Update is called once per frame

        private void Update()
        {
            if (SelectingEntity != OnMapEntity.SelectingEntity)
            {
                Unselect(null);
                Select(OnMapEntity.SelectingEntity);
            }
        }

        private void Display()
        {
            if (LastEntity == SelectingEntity)
            {
                return;
            }
            else if (!SelectingEntity)
            {
                Close();
                return;
            }
            if (SelectingEntity is CellEntity)
            {
                Close();
                return;
            }

            SetPanelActive(true);
            LastEntity = SelectingEntity;
            BasicDisplay();
            ShowDisplayer();
        }

        private void Close()
        {
            nameDisplayer.text = "";
            career.enabled = false;
            type.enabled = false;
            iconDisplayer.enabled = false;
            CloseCurrentDisplayer();
            SetPanelActive(false);
        }

        public void SetPanelActive(bool value)
        {
            nameDisplayer.gameObject.SetActive(value);
            // GetComponent<Image>().enabled = value;
            if (value is true)
            {
                Show();
            }
            else Hide();
        }

        private void CloseCurrentDisplayer()
        {
            Destroy(infoPanel?.gameObject);
            infoPanel = null;
        }

        private void BasicDisplay()
        {
            if (SelectingEntity.Data is ICareerLabled)
            {
                if ((SelectingEntity.Data as ICareerLabled).Career != Career.none)
                {
                    career.enabled = true;
                    career.sprite = GameData.SpriteLoader.Get((SelectingEntity.Data as ICareerLabled).Career.ToString());
                }
            }
            if (SelectingEntity.Data is BattleArmy)
            {
                type.enabled = true;
                type.sprite = GameData.SpriteLoader.Get((SelectingEntity.Data as BattleArmy).Type.ToString());
            }

            iconDisplayer.enabled = true;
            iconDisplayer.sprite = SelectingEntity.Data.DisplayingIcon;
            nameDisplayer.text = SelectingEntity?.Data.DisplayingName;
        }

        public void ShowDisplayer()
        {
            CloseCurrentDisplayer();

            if (SelectingEntity is ArmyEntity)
            {
                InfoPanelArmy armyPanelInfo = Instantiate(armyInfoPrefab, transform).GetComponent<InfoPanelArmy>();
                infoPanel = armyPanelInfo;
                armyPanelInfo.armyEntity = SelectingEntity as ArmyEntity;
                armyPanelInfo.LoadStatus();
            }
            else if (SelectingEntity is BuildingEntity)
            {
                InfoPanelBuilding buildingPanelInfo = Instantiate(buildingInfoPrefab, transform).GetComponent<InfoPanelBuilding>();
                infoPanel = buildingPanelInfo;
                buildingPanelInfo.buildingEntity = SelectingEntity as BuildingEntity;
                buildingPanelInfo.LoadStatus();


            }
        }

        public override void Hide()
        {
            if (!isShown)
            {
                return;
            }
            LastEntity = null;
            Module.Motion.SetMotion(gameObject, transform.position - new Vector3(3, 0, 0));
            isShown = false;
        }

        public override void Show()
        {
            LastEntity = null;
            base.Show();
        }

        public void Select(Entity entity)
        {
            if (entity is OnMapEntity)
            {
                SelectingEntity = entity as OnMapEntity;
                Display();
            }
        }

        public void Unselect(Entity entity)
        {
            SelectingEntity = null;
            Display();
        }
    }
}

