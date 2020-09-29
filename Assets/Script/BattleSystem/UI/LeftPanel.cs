using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class LeftPanel : BattleUIBase
    {
        public Text nameDisplayer;

        public Image typeBG;
        public Image iconDisplayer;
        public Image career;
        public Image attackPos;
        public Image standPos;

        public InfoPanel infoPanel;

        [Header("Prefabs")]
        public GameObject armyInfoPrefab;
        public GameObject buildingInfoPrefab;
        public GameObject cellInfoPrefab;

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

            SetPanelActive(true);
            LastEntity = SelectingEntity;
            BasicDisplay();
            ShowDisplayer();
        }

        private void Close()
        {
            nameDisplayer.text = "";
            career.enabled = false;
            typeBG.enabled = false;
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
                    career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, (SelectingEntity.Data as ICareerLabled).Career.ToString());
                }
            }
            if (SelectingEntity.Data is BattleArmy)
            {
                typeBG.enabled = true;
                typeBG.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, (SelectingEntity.Data as BattleArmy).Type.ToString());
            }

            iconDisplayer.enabled = true;
            iconDisplayer.sprite = SelectingEntity.Data.Icon;
            nameDisplayer.text = SelectingEntity?.Data.DisplayingName;
        }

        public void ShowDisplayer()
        {
            CloseCurrentDisplayer();

            if (SelectingEntity is ArmyEntity)
            {
                InfoPanelArmy armyPanelInfo = Instantiate(armyInfoPrefab, transform).GetComponent<InfoPanelArmy>();
                infoPanel = armyPanelInfo;
                armyPanelInfo.ArmyEntity = SelectingEntity as ArmyEntity;
                armyPanelInfo.LoadStatus();
            }
            else if (SelectingEntity is BuildingEntity)
            {
                InfoPanelBuilding buildingPanelInfo = Instantiate(buildingInfoPrefab, transform).GetComponent<InfoPanelBuilding>();
                infoPanel = buildingPanelInfo;
                buildingPanelInfo.buildingEntity = SelectingEntity as BuildingEntity;
                buildingPanelInfo.LoadStatus();
            }
            else if (SelectingEntity is CellEntity)
            {
                InfoPanelCell cellPanelInfo = Instantiate(cellInfoPrefab, transform).GetComponent<InfoPanelCell>();
                infoPanel = cellPanelInfo;
                cellPanelInfo.cellEntity = SelectingEntity as CellEntity;
                cellPanelInfo.LoadStatus();
            }
        }

        public override void Hide()
        {
            if (!isShown)
            {
                return;
            }
            LastEntity = null;
            Module.Motion.SetMotion(gameObject, transform.position - new Vector3(3, 0, 0), true);
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

