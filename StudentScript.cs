using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020004E6 RID: 1254
public class StudentScript : MonoBehaviour
{
	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x06002151 RID: 8529 RVA: 0x001C7BD1 File Offset: 0x001C5DD1
	public bool Alive
	{
		get
		{
			return this.DeathType == DeathType.None;
		}
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x001C7BDC File Offset: 0x001C5DDC
	public void Start()
	{
		string idleAnim = this.IdleAnim;
		this.CounterAnim = "f02_teacherCounterB_00";
		if (!this.Started)
		{
			this.CharacterAnimation = this.Character.GetComponent<Animation>();
			this.MyBento = this.Bento.GetComponent<GenericBentoScript>();
			this.Pathfinding.repathRate += (float)this.StudentID * 0.01f;
			this.OriginalIdleAnim = this.IdleAnim;
			this.OriginalLeanAnim = this.LeanAnim;
			this.Friend = PlayerGlobals.GetStudentFriend(this.StudentID);
			if (ClubGlobals.Club == ClubType.Occult)
			{
				this.Perception = 0.5f;
			}
			else
			{
				this.BookRenderer.material.mainTexture = this.RedBookTexture;
			}
			this.Hearts.emission.enabled = false;
			this.Prompt.OwnerType = PromptOwnerType.Person;
			this.Paranoia = 2f - SchoolGlobals.SchoolAtmosphere;
			this.VisionDistance = ((PlayerGlobals.PantiesEquipped == 4) ? 5f : 10f) * this.Paranoia;
			if (this.Yandere != null && this.Yandere.DetectionPanel != null)
			{
				this.SpawnDetectionMarker();
			}
			StudentJson studentJson = this.JSON.Students[this.StudentID];
			this.OriginalScheduleBlocks = studentJson.ScheduleBlocks;
			this.ScheduleBlocks = studentJson.ScheduleBlocks;
			this.Persona = studentJson.Persona;
			this.Class = studentJson.Class;
			this.Crush = studentJson.Crush;
			this.Club = studentJson.Club;
			this.BreastSize = studentJson.BreastSize;
			this.Strength = studentJson.Strength;
			this.Hairstyle = studentJson.Hairstyle;
			this.Accessory = studentJson.Accessory;
			this.Name = studentJson.Name;
			this.OriginalClub = this.Club;
			if (!this.StudentManager.LoveSick && SchoolAtmosphere.Type == SchoolAtmosphereType.Low && this.Club != ClubType.Council && this.Persona != PersonaType.Heroic && this.Persona != PersonaType.Violent && this.StudentID < 90 && (this.Club <= ClubType.Gaming || this.Club == ClubType.Newspaper))
			{
				this.IdleAnim = this.ParanoidAnim;
			}
			if (StudentGlobals.GetStudentBroken(this.StudentID))
			{
				this.Cosmetic.RightEyeRenderer.gameObject.SetActive(false);
				this.Cosmetic.LeftEyeRenderer.gameObject.SetActive(false);
				this.Cosmetic.RightIrisLight.SetActive(false);
				this.Cosmetic.LeftIrisLight.SetActive(false);
				this.RightEmptyEye.SetActive(true);
				this.LeftEmptyEye.SetActive(true);
				this.Traumatized = true;
				this.Shy = true;
				this.Persona = PersonaType.Coward;
			}
			if (this.Name == "Random")
			{
				this.Persona = (PersonaType)UnityEngine.Random.Range(1, 8);
				if (this.Persona == PersonaType.Lovestruck)
				{
					this.Persona = PersonaType.PhoneAddict;
				}
				studentJson.Persona = this.Persona;
				if (this.Persona == PersonaType.Heroic)
				{
					this.Strength = UnityEngine.Random.Range(1, 5);
					studentJson.Strength = this.Strength;
				}
			}
			this.Seat = this.StudentManager.Seats[this.Class].List[studentJson.Seat];
			base.gameObject.name = string.Concat(new string[]
			{
				"Student_",
				this.StudentID.ToString(),
				" (",
				this.Name,
				")"
			});
			this.OriginalPersona = this.Persona;
			if (this.StudentID == 81 && StudentGlobals.GetStudentBroken(81))
			{
				this.Persona = PersonaType.Coward;
			}
			if (this.Persona == PersonaType.Loner || this.Persona == PersonaType.Coward || this.Persona == PersonaType.Fragile)
			{
				this.CameraAnims = this.CowardAnims;
			}
			else if (this.Persona == PersonaType.TeachersPet || this.Persona == PersonaType.Heroic || this.Persona == PersonaType.Strict || this.Persona == PersonaType.LandlineUser || this.Persona == PersonaType.Dangerous)
			{
				this.CameraAnims = this.HeroAnims;
			}
			else if (this.Persona == PersonaType.Evil || this.Persona == PersonaType.Spiteful || this.Persona == PersonaType.Violent)
			{
				this.CameraAnims = this.EvilAnims;
			}
			else if (this.Persona == PersonaType.SocialButterfly || this.Persona == PersonaType.Lovestruck || this.Persona == PersonaType.PhoneAddict || this.Persona == PersonaType.Sleuth)
			{
				this.CameraAnims = this.SocialAnims;
			}
			if (ClubGlobals.GetClubClosed(this.Club))
			{
				if ((this.Club == ClubType.Cooking || this.Club == ClubType.Drama || this.Club == ClubType.Occult || this.Club == ClubType.Gaming || this.Club == ClubType.Art || this.Club == ClubType.MartialArts || this.Club == ClubType.LightMusic || this.Club == ClubType.Photography || this.Club == ClubType.Science || this.Club == ClubType.Sports || this.Club == ClubType.Gardening || this.Club == ClubType.Newspaper) && this.StudentManager.SuitorID != this.StudentID)
				{
					ScheduleBlock scheduleBlock = this.ScheduleBlocks[7];
					scheduleBlock.destination = "Locker";
					scheduleBlock.action = "Shoes";
					if (this.ScheduleBlocks.Length > 8)
					{
						ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[8];
						scheduleBlock2.destination = "Exit";
						scheduleBlock2.action = "Stand";
					}
				}
				this.Club = ClubType.None;
				this.GetDestinations();
			}
			this.Yandere = this.StudentManager.Yandere;
			this.StalkTarget = this.Yandere.transform;
			this.DialogueWheel = this.StudentManager.DialogueWheel;
			this.ClubManager = this.StudentManager.ClubManager;
			this.Reputation = this.StudentManager.Reputation;
			this.Subtitle = this.Yandere.Subtitle;
			this.Police = this.StudentManager.Police;
			this.Clock = this.StudentManager.Clock;
			this.CameraEffects = this.Yandere.MainCamera.GetComponent<CameraEffectsScript>();
			this.RightEyeOrigin = this.RightEye.localPosition;
			this.LeftEyeOrigin = this.LeftEye.localPosition;
			this.Bento.GetComponent<GenericBentoScript>().StudentID = this.StudentID;
			this.DisableProps();
			this.OriginalOriginalSprintAnim = this.SprintAnim;
			this.OriginalOriginalWalkAnim = this.WalkAnim;
			this.OriginalSprintAnim = this.SprintAnim;
			this.OriginalWalkAnim = this.WalkAnim;
			if (this.Persona == PersonaType.Evil)
			{
				this.ScaredAnim = this.EvilWitnessAnim;
			}
			if (this.Persona == PersonaType.PhoneAddict)
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.01f, 0.005f, 0.01f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, 165f);
				this.Countdown.Speed = 0.1f;
				if (!this.StudentManager.Eighties)
				{
					this.SprintAnim = this.PhoneAnims[2];
					this.PatrolAnim = this.PhoneAnims[3];
				}
			}
			if (this.Club == ClubType.Bully)
			{
				this.SkirtCollider.transform.localPosition = new Vector3(0f, 0.055f, 0f);
				if (!StudentGlobals.GetStudentBroken(this.StudentID))
				{
					if (!this.StudentManager.Eighties)
					{
						this.IdleAnim = this.PhoneAnims[0];
					}
					this.BullyID = this.StudentID - 80;
				}
			}
			this.DisableProps();
			if (this.Rival)
			{
				this.MapMarker.gameObject.SetActive(true);
				if (this.StudentManager.MissionMode)
				{
					this.MapMarker.gameObject.GetComponent<Renderer>().material.mainTexture = this.StudentManager.TargetMapMarker;
					this.MapMarker.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 1f);
				}
				else if (this.StudentManager.CustomMode)
				{
					this.MapMarker.gameObject.GetComponent<Renderer>().material.mainTexture = this.StudentManager.RivalMapMarker;
				}
				else if (this.StudentManager.Eighties)
				{
					this.MapMarker.gameObject.GetComponent<Renderer>().material.mainTexture = this.StudentManager.Police.EndOfDay.Counselor.EightiesRivalHeads[this.StudentManager.Week];
				}
				else
				{
					this.MapMarker.gameObject.GetComponent<Renderer>().material.mainTexture = this.StudentManager.Police.EndOfDay.Counselor.RivalHeads[this.StudentManager.Week];
				}
			}
			if (!this.StudentManager.Eighties)
			{
				int i = this.StudentID;
				if (i > 35)
				{
					while (i > 35)
					{
						i -= 35;
					}
				}
				this.SmartPhone.GetComponent<Renderer>().material.mainTexture = this.PhoneTextures[i];
				if (this.StudentID == 1)
				{
					this.MapMarker.gameObject.SetActive(true);
					if (DateGlobals.Week == 2)
					{
						ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[2];
						scheduleBlock3.destination = "Patrol";
						scheduleBlock3.action = "Patrol";
						ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[7];
						scheduleBlock4.destination = "Patrol";
						scheduleBlock4.action = "Patrol";
					}
				}
				else if (this.StudentID == 2)
				{
					if (StudentGlobals.GetStudentDead(3) || StudentGlobals.GetStudentKidnapped(3) || this.StudentManager.Students[3] == null || (this.StudentManager.Students[3] != null && this.StudentManager.Students[3].Slave))
					{
						ScheduleBlock scheduleBlock5 = this.ScheduleBlocks[2];
						scheduleBlock5.destination = "Mourn";
						scheduleBlock5.action = "Mourn";
						ScheduleBlock scheduleBlock6 = this.ScheduleBlocks[7];
						scheduleBlock6.destination = "Mourn";
						scheduleBlock6.action = "Mourn";
						this.IdleAnim = this.BulliedIdleAnim;
						this.WalkAnim = this.BulliedWalkAnim;
					}
					this.PatrolAnim = this.SearchPatrolAnim;
				}
				else if (this.StudentID == 3)
				{
					if (StudentGlobals.GetStudentDead(2) || StudentGlobals.GetStudentKidnapped(2) || this.StudentManager.Students[2] == null || (this.StudentManager.Students[2] != null && this.StudentManager.Students[2].Slave))
					{
						ScheduleBlock scheduleBlock7 = this.ScheduleBlocks[2];
						scheduleBlock7.destination = "Mourn";
						scheduleBlock7.action = "Mourn";
						ScheduleBlock scheduleBlock8 = this.ScheduleBlocks[7];
						scheduleBlock8.destination = "Mourn";
						scheduleBlock8.action = "Mourn";
						this.IdleAnim = this.BulliedIdleAnim;
						this.WalkAnim = this.BulliedWalkAnim;
					}
					this.PatrolAnim = this.SearchPatrolAnim;
				}
				else if (this.StudentID == 4)
				{
					this.IdleAnim = "f02_idleShort_00";
					this.WalkAnim = "f02_newWalk_00";
				}
				else if (this.StudentID == 5)
				{
					if (!this.StudentManager.TakingPortraits)
					{
						if (this.StudentManager.Students[81] == null && this.StudentManager.Students[82] == null && this.StudentManager.Students[83] == null && this.StudentManager.Students[84] == null && this.StudentManager.Students[85] == null)
						{
							this.CharacterAnimation["f02_smile_00"].layer = 1;
							this.CharacterAnimation.Play("f02_smile_00");
							this.CharacterAnimation["f02_smile_00"].weight = 1f;
						}
						else
						{
							this.Shy = true;
						}
					}
					this.WaveAnim = "f02_casualWave_04";
				}
				else if (this.StudentID == 6)
				{
					this.RelaxAnim = "crossarms_00";
					this.CameraAnims = this.HeroAnims;
					this.Curious = true;
					this.Crush = 11;
					if (this.StudentManager.Students[11] == null || this.StudentManager.Students[11].Slave)
					{
						this.Curious = false;
					}
				}
				else if (this.StudentID == 7)
				{
					this.MustTrip = true;
					this.RunAnim = "runFeminine_00";
					this.SprintAnim = "runFeminine_00";
					this.RelaxAnim = "infirmaryRest_00";
					this.OriginalSprintAnim = this.SprintAnim;
					this.Cosmetic.Start();
					if (!GameGlobals.AlphabetMode && !this.StudentManager.MissionMode)
					{
						base.gameObject.SetActive(false);
					}
					if (GameGlobals.AlphabetMode)
					{
						this.Head.gameObject.GetComponent<SphereCollider>().radius = 0.3f;
					}
					this.WaveAnim = "f02_casualWave_04";
				}
				else if (this.StudentID == 8)
				{
					this.IdleAnim = this.BulliedIdleAnim;
					this.WalkAnim = this.BulliedWalkAnim;
					this.WaveAnim = "f02_casualWave_04";
				}
				else if (this.StudentID == 9)
				{
					this.IdleAnim = "idleScholarly_00";
					this.WalkAnim = "walkScholarly_00";
					this.CameraAnims = this.HeroAnims;
				}
				else if (this.StudentID == 10)
				{
					this.Reflexes = true;
					if (GameGlobals.AlphabetMode || this.StudentManager.MissionMode)
					{
						this.OriginalPersona = PersonaType.Heroic;
						this.Persona = PersonaType.Heroic;
						this.Reflexes = false;
						this.Strength = 5;
					}
					else
					{
						this.LovestruckTarget = 11;
					}
					if (this.StudentManager.Students[11] != null && GameGlobals.GetRivalEliminations(1) != 6 && (float)StudentGlobals.GetStudentReputation(10) > -33.33333f && StudentGlobals.StudentSlave != 11 && !GameGlobals.AlphabetMode && !this.StudentManager.MissionMode)
					{
						this.StudentManager.Patrols.List[this.StudentID].parent = this.StudentManager.Students[11].transform;
						this.StudentManager.Patrols.List[this.StudentID].localEulerAngles = new Vector3(0f, 0f, 0f);
						this.StudentManager.Patrols.List[this.StudentID].localPosition = new Vector3(0f, 0f, 0f);
						this.VomitPhase = -1;
						this.Indoors = true;
						this.Spawned = true;
						this.Calm = true;
						if (this.ShoeRemoval.Locker == null)
						{
							this.ShoeRemoval.Start();
						}
						this.ShoeRemoval.PutOnShoes();
						this.FollowTarget = this.StudentManager.Students[11];
						this.FollowTarget.Follower = this;
						this.IdleAnim = "f02_idleGirly_00";
						this.WalkAnim = "f02_walkGirly_00";
						this.PatrolAnim = "f02_stretch_00";
						this.OriginalIdleAnim = this.IdleAnim;
						if (GameGlobals.RivalEliminationID == 4)
						{
							this.StudentManager.LunchSpots.List[this.StudentID].position = this.StudentManager.AltFriendLunchSpot.position;
							this.StudentManager.LunchSpots.List[this.StudentID].eulerAngles = this.StudentManager.AltFriendLunchSpot.eulerAngles;
						}
						if (this.StudentManager.Week > 1)
						{
							this.Persona = PersonaType.SocialButterfly;
							ScheduleBlock scheduleBlock9 = this.ScheduleBlocks[4];
							scheduleBlock9.destination = "LunchSpot";
							scheduleBlock9.action = "Eat";
						}
					}
					else
					{
						Debug.Log("Due to the current circumstances, we're changing Raibaru's destinations.");
						if (this.StudentManager.Students[11] == null || StudentGlobals.StudentSlave == 11 || GameGlobals.AlphabetMode || this.StudentManager.MissionMode)
						{
							this.RaibaruOsanaDeathScheduleChanges();
						}
						else if ((float)StudentGlobals.GetStudentReputation(10) <= -33.33333f)
						{
							this.ShoeRemoval.PutOnShoes();
							ScheduleBlock scheduleBlock10 = this.ScheduleBlocks[2];
							scheduleBlock10.destination = "ShameSpot";
							scheduleBlock10.action = "Shamed";
							scheduleBlock10.time = 8f;
							ScheduleBlock scheduleBlock11 = this.ScheduleBlocks[4];
							scheduleBlock11.destination = "Seat";
							scheduleBlock11.action = "Sit";
							this.IdleAnim = this.BulliedIdleAnim;
							this.WalkAnim = this.BulliedWalkAnim;
							this.OriginalIdleAnim = this.IdleAnim;
						}
						else if (GameGlobals.GetRivalEliminations(1) == 6)
						{
							Debug.Log("Raibaru isn't going to follow Osana around, since she's in a relationship.");
							this.StudentManager.Hangouts.List[10].transform.position = new Vector3(-5f, 0f, -6f);
							this.StudentManager.Hangouts.List[10].transform.eulerAngles = new Vector3(0f, 180f, 0f);
							this.StudentManager.Week2Hangouts.List[10].transform.position = new Vector3(-5f, 0f, -6f);
							this.StudentManager.Week2Hangouts.List[10].transform.eulerAngles = new Vector3(0f, 180f, 0f);
							ScheduleBlock scheduleBlock12 = this.ScheduleBlocks[2];
							scheduleBlock12.destination = "Hangout";
							scheduleBlock12.action = "Relax";
							scheduleBlock12.time = 8f;
							this.RelaxAnim = this.ThinkAnim;
							if (this.StudentManager.Students[46] != null)
							{
								this.Curious = true;
								this.Spawned = true;
								this.Crush = 46;
							}
							ScheduleBlock scheduleBlock13 = this.ScheduleBlocks[4];
							scheduleBlock13.destination = "LunchSpot";
							scheduleBlock13.action = "Eat";
						}
						this.RaibaruStopsFollowingOsana();
						this.TargetDistance = 0.5f;
					}
					this.PhotoPatience = 0f;
					this.OriginalWalkAnim = this.WalkAnim;
					this.CharacterAnimation["f02_nervousLeftRight_00"].speed = 0.5f;
				}
				else if (this.StudentID == 11)
				{
					this.SmartPhone.transform.localPosition = new Vector3(-0.0075f, -0.0025f, -0.0075f);
					this.SmartPhone.transform.localEulerAngles = new Vector3(5f, -150f, 170f);
					this.SmartPhone.GetComponent<Renderer>().material.mainTexture = this.OsanaPhoneTexture;
					this.IdleAnim = "f02_tsunIdle_00";
					this.WalkAnim = "f02_tsunWalk_00";
					this.TaskAnims[0] = "f02_Task33_Line0";
					this.TaskAnims[1] = "f02_Task33_Line1";
					this.TaskAnims[2] = "f02_Task33_Line2";
					this.TaskAnims[3] = "f02_Task33_Line3";
					this.TaskAnims[4] = "f02_Task33_Line4";
					this.TaskAnims[5] = "f02_Task33_Line5";
					this.LovestruckTarget = 1;
					if (GameGlobals.RivalEliminationID == 4)
					{
						this.StudentManager.LunchSpots.List[this.StudentID].position = this.StudentManager.AltRivalLunchSpot.position;
						this.StudentManager.LunchSpots.List[this.StudentID].eulerAngles = this.StudentManager.AltRivalLunchSpot.eulerAngles;
					}
					if (this.StudentManager.Students[10] == null)
					{
						Debug.Log("Raibaru has been killed/arrested/vanished, so Osana's schedule has changed.");
						ScheduleBlock scheduleBlock14 = this.ScheduleBlocks[2];
						scheduleBlock14.destination = "Mourn";
						scheduleBlock14.action = "Mourn";
						ScheduleBlock scheduleBlock15 = this.ScheduleBlocks[7];
						scheduleBlock15.destination = "Mourn";
						scheduleBlock15.action = "Mourn";
						this.IdleAnim = this.BulliedIdleAnim;
						this.WalkAnim = this.BulliedWalkAnim;
					}
					else if (PlayerGlobals.RaibaruLoner || GameGlobals.AlphabetMode || this.StudentManager.MissionMode)
					{
						ScheduleBlock scheduleBlock16 = this.ScheduleBlocks[2];
						scheduleBlock16.destination = "Patrol";
						scheduleBlock16.action = "Patrol";
						ScheduleBlock scheduleBlock17 = this.ScheduleBlocks[7];
						scheduleBlock17.destination = "Patrol";
						scheduleBlock17.action = "Patrol";
						this.PatrolAnim = "f02_pondering_00";
						if (this.StudentManager.MissionMode)
						{
							this.OriginalPersona = PersonaType.Loner;
							this.Persona = PersonaType.Loner;
						}
					}
					this.OriginalWalkAnim = this.WalkAnim;
				}
				else if (this.StudentID == 12)
				{
					this.SmartPhone.GetComponent<Renderer>().material.mainTexture = this.Cosmetic.SmartphoneTextures[this.StudentID];
					if (!ClubGlobals.GetClubClosed(ClubType.Cooking))
					{
						this.Armband.SetActive(true);
						this.ClubLeader = true;
					}
					this.OriginalIdleAnim = "f02_gentleIdle_00";
					this.OriginalWalkAnim = "f02_gentleWalk_00";
					this.IdleAnim = this.OriginalIdleAnim;
					this.WalkAnim = this.OriginalWalkAnim;
					this.Subtitle.EightiesClubDialogue.UpdateDialogue(1);
					this.Subtitle.EightiesClubDialogue.UpdateDialogue(1);
					if (this.StudentManager.MissionMode)
					{
						this.OriginalPersona = PersonaType.SocialButterfly;
						this.Persona = PersonaType.SocialButterfly;
					}
				}
				else if (this.StudentID == 21)
				{
					this.Crush = 12;
				}
				else if (this.StudentID == 24 && this.StudentID == 25)
				{
					this.IdleAnim = "f02_idleGirly_00";
					this.WalkAnim = "f02_walkGirly_00";
				}
				else if (this.StudentID == 26)
				{
					this.IdleAnim = "idleHaughty_00";
					this.WalkAnim = "walkHaughty_00";
				}
				else if (this.StudentID == 27)
				{
					this.IdleAnim = "idleConfident_00";
					this.WalkAnim = "walkConfident_00";
				}
				else if (this.StudentID == 29)
				{
					this.IdleAnim = "f02_idleElegant_00";
					this.WalkAnim = "f02_walkElegant_00";
				}
				else if (this.StudentID == 30)
				{
					this.SmartPhone.GetComponent<Renderer>().material.mainTexture = this.KokonaPhoneTexture;
				}
				else if (this.StudentID == 31)
				{
					this.IdleAnim = this.BulliedIdleAnim;
					this.WalkAnim = this.BulliedWalkAnim;
				}
				else if (this.StudentID == 34 || this.StudentID == 35)
				{
					this.IdleAnim = "f02_idleShort_00";
					this.WalkAnim = "f02_newWalk_00";
					if (this.Paranoia > 1.66666f)
					{
						this.IdleAnim = this.ParanoidAnim;
						this.WalkAnim = this.ParanoidWalkAnim;
					}
				}
				else if (this.StudentID == 36)
				{
					if (TaskGlobals.GetTaskStatus(36) < 3)
					{
						this.IdleAnim = "slouchIdle_00";
						this.WalkAnim = "slouchWalk_00";
						this.ClubAnim = "slouchGaming_00";
					}
					else
					{
						this.IdleAnim = "properIdle_00";
						this.WalkAnim = "properWalk_00";
						this.ClubAnim = "properGaming_00";
					}
					if (this.Paranoia > 1.66666f)
					{
						this.IdleAnim = this.ParanoidAnim;
						this.WalkAnim = this.ParanoidWalkAnim;
					}
					if (this.StudentManager.MissionMode)
					{
						ScheduleBlock scheduleBlock18 = this.ScheduleBlocks[4];
						scheduleBlock18.destination = "LunchSpot";
						scheduleBlock18.action = "Eat";
					}
				}
				else if (this.StudentID == 39)
				{
					this.SmartPhone.GetComponent<Renderer>().material.mainTexture = this.MidoriPhoneTexture;
					this.PatrolAnim = "f02_midoriTexting_00";
				}
				else if (this.StudentID == 51)
				{
					this.IdleAnim = "f02_idleConfident_01";
					this.WalkAnim = "f02_walkConfident_01";
					if (ClubGlobals.GetClubClosed(ClubType.LightMusic))
					{
						this.IdleAnim = this.BulliedIdleAnim;
						this.WalkAnim = this.BulliedWalkAnim;
						this.CameraAnims = this.CowardAnims;
						this.OriginalPersona = PersonaType.Loner;
						this.Persona = PersonaType.Loner;
						if (!SchoolGlobals.RoofFence)
						{
							ScheduleBlock scheduleBlock19 = this.ScheduleBlocks[2];
							scheduleBlock19.destination = "Sulk";
							scheduleBlock19.action = "Sulk";
							ScheduleBlock scheduleBlock20 = this.ScheduleBlocks[4];
							scheduleBlock20.destination = "Sulk";
							scheduleBlock20.action = "Sulk";
							ScheduleBlock scheduleBlock21 = this.ScheduleBlocks[7];
							scheduleBlock21.destination = "Sulk";
							scheduleBlock21.action = "Sulk";
							ScheduleBlock scheduleBlock22 = this.ScheduleBlocks[8];
							scheduleBlock22.destination = "Sulk";
							scheduleBlock22.action = "Sulk";
						}
						else
						{
							ScheduleBlock scheduleBlock23 = this.ScheduleBlocks[2];
							scheduleBlock23.destination = "Seat";
							scheduleBlock23.action = "Sit";
							ScheduleBlock scheduleBlock24 = this.ScheduleBlocks[4];
							scheduleBlock24.destination = "Seat";
							scheduleBlock24.action = "Sit";
							ScheduleBlock scheduleBlock25 = this.ScheduleBlocks[7];
							scheduleBlock25.destination = "Seat";
							scheduleBlock25.action = "Sit";
							ScheduleBlock scheduleBlock26 = this.ScheduleBlocks[8];
							scheduleBlock26.destination = "Seat";
							scheduleBlock26.action = "Sit";
						}
					}
					if (this.StudentManager.MissionMode)
					{
						ScheduleBlock scheduleBlock27 = this.ScheduleBlocks[4];
						scheduleBlock27.destination = "LunchSpot";
						scheduleBlock27.action = "Eat";
					}
				}
				else if (this.StudentID == 56)
				{
					this.IdleAnim = "idleConfident_00";
					this.WalkAnim = "walkConfident_00";
					this.SleuthID = 1;
				}
				else if (this.StudentID == 57)
				{
					this.IdleAnim = "idleChill_01";
					this.WalkAnim = "walkChill_01";
					this.SleuthID = 20;
				}
				else if (this.StudentID == 58)
				{
					this.IdleAnim = "idleChill_00";
					this.WalkAnim = "walkChill_00";
					this.SleuthID = 40;
				}
				else if (this.StudentID == 59)
				{
					this.IdleAnim = "f02_idleGraceful_00";
					this.WalkAnim = "f02_walkGraceful_00";
					this.SleuthID = 60;
				}
				else if (this.StudentID == 60)
				{
					this.IdleAnim = "f02_idleScholarly_00";
					this.WalkAnim = "f02_walkScholarly_00";
					this.CameraAnims = this.HeroAnims;
					this.SleuthID = 80;
				}
				else if (this.StudentID == 61)
				{
					this.IdleAnim = "scienceIdle_00";
					this.WalkAnim = "scienceWalk_00";
					this.OriginalWalkAnim = "scienceWalk_00";
				}
				else if (this.StudentID == 62)
				{
					this.IdleAnim = "idleFrown_00";
					this.WalkAnim = "walkFrown_00";
					if (this.Paranoia > 1.66666f)
					{
						this.IdleAnim = this.ParanoidAnim;
						this.WalkAnim = this.ParanoidWalkAnim;
					}
				}
				else if (this.StudentID == 64 || this.StudentID == 65)
				{
					this.IdleAnim = "f02_idleShort_00";
					this.WalkAnim = "f02_newWalk_00";
					if (this.Paranoia > 1.66666f)
					{
						this.IdleAnim = this.ParanoidAnim;
						this.WalkAnim = this.ParanoidWalkAnim;
					}
					if (this.StudentID == 65)
					{
						if (this.StudentManager.RobotPhase == 1)
						{
							this.StudentManager.Clubs.List[65].eulerAngles = new Vector3(0f, -90f, 0f);
							this.AnimationNames[0] = "f02_carefreeTalk_00";
							this.AnimationNames[1] = "f02_carefreeTalk_01";
							this.AnimationNames[2] = "f02_carefreeTalk_02";
						}
						else if (this.StudentManager.RobotPhase == 2)
						{
							this.StudentManager.Clubs.List[65].position = new Vector3(15.31f, 8f, 26.83333f);
							this.StudentManager.Clubs.List[65].eulerAngles = new Vector3(0f, 180f, 0f);
							this.ClubAnim = "f02_brokenSit_00";
							this.IdleAnim = this.BulliedIdleAnim;
							this.WalkAnim = this.BulliedWalkAnim;
							this.Depressed = true;
						}
					}
				}
				else if (this.StudentID == 66)
				{
					this.IdleAnim = "pose_03";
					this.OriginalWalkAnim = "walkConfident_00";
					this.WalkAnim = "walkConfident_00";
					this.ClubThreshold = 100f;
				}
				else if (this.StudentID == 69)
				{
					this.IdleAnim = "idleFrown_00";
					this.WalkAnim = "walkFrown_00";
					if (this.Paranoia > 1.66666f)
					{
						this.IdleAnim = this.ParanoidAnim;
						this.WalkAnim = this.ParanoidWalkAnim;
					}
				}
				else if (this.StudentID == 71)
				{
					this.IdleAnim = "f02_idleGirly_00";
					this.WalkAnim = "f02_walkGirly_00";
					if (!this.PickPocket.gameObject.transform.parent.gameObject.activeInHierarchy)
					{
						this.PickPocket.gameObject.transform.parent.gameObject.SetActive(true);
					}
				}
				if ((this.StudentID == 6 || this.StudentID == 11) && DatingGlobals.SuitorProgress == 2)
				{
					this.Partner = ((this.StudentID == 11) ? this.StudentManager.Students[6] : this.StudentManager.Students[11]);
					ScheduleBlock scheduleBlock28 = this.ScheduleBlocks[2];
					scheduleBlock28.destination = "Cuddle";
					scheduleBlock28.action = "Cuddle";
					ScheduleBlock scheduleBlock29 = this.ScheduleBlocks[4];
					scheduleBlock29.destination = "Cuddle";
					scheduleBlock29.action = "Eat";
					ScheduleBlock scheduleBlock30 = this.ScheduleBlocks[7];
					scheduleBlock30.destination = "Locker";
					scheduleBlock30.action = "Shoes";
					ScheduleBlock scheduleBlock31 = this.ScheduleBlocks[8];
					scheduleBlock31.destination = "Exit";
					scheduleBlock31.action = "Exit";
				}
				if (this.Rival && !this.Slave && !this.StudentManager.MissionMode)
				{
					if (this.StudentID > 11 && this.StudentID < 21)
					{
						ScheduleBlock scheduleBlock32 = this.ScheduleBlocks[2];
						this.OriginalDestination = scheduleBlock32.destination;
						this.OriginalAction = scheduleBlock32.action;
						scheduleBlock32.destination = "Seat";
						scheduleBlock32.action = "PlaceBag";
						this.BookBag.SetActive(true);
					}
					this.LovestruckTarget = 1;
				}
			}
			else
			{
				if (this.StudentID == 1)
				{
					this.MapMarker.gameObject.SetActive(true);
					this.MapMarker.gameObject.GetComponent<Renderer>().material.mainTexture = this.JokichiHead;
					if (!this.StudentManager.CustomMode)
					{
						this.IdleAnim = this.IdleAnims[3];
						this.WalkAnim = this.WalkAnims[3];
						this.AnimSetID = 3;
					}
				}
				this.BookRenderer.material.mainTexture = this.RedBookTexture;
				this.Phoneless = true;
				if (!this.Male)
				{
					this.PatrolAnim = "f02_thinking_00";
				}
				if (this.Rival)
				{
					if (this.StudentID > 10 && this.StudentID < 21)
					{
						ScheduleBlock scheduleBlock33 = this.ScheduleBlocks[2];
						this.OriginalDestination = scheduleBlock33.destination;
						this.OriginalAction = scheduleBlock33.action;
						scheduleBlock33.destination = "Seat";
						scheduleBlock33.action = "PlaceBag";
						this.BookBag.SetActive(true);
					}
					this.LovestruckTarget = 1;
				}
				if (!this.StudentManager.CustomMode)
				{
					if (this.StudentID == 11)
					{
						this.IdleAnim = "f02_idleGirly_00";
						this.WalkAnim = "f02_walkGirly_00";
						if (!this.Rival)
						{
							this.Persona = PersonaType.LandlineUser;
						}
					}
					else if (this.StudentID == 12)
					{
						this.CharacterAnimation["f02_startFire_00"].speed = 2f;
						this.IdleAnim = "f02_idleChill_00";
						this.WalkAnim = "f02_walkChill_00";
						this.GameAnim = "f02_lookLeftRight_00";
						this.PyroUrge = true;
						this.PyroLimit = 60f;
						if (this.StudentManager.Week > 2)
						{
							this.PyroLimit = 5f;
						}
						if (!this.Rival)
						{
							this.Persona = PersonaType.LandlineUser;
						}
					}
					else if (this.StudentID == 13)
					{
						this.IdleAnim = "f02_idleShy_00";
						this.WalkAnim = "f02_walkShy_00";
						this.WaveAnim = "f02_casualWave_04";
						if (!this.Rival)
						{
							this.Persona = PersonaType.Coward;
						}
					}
					else if (this.StudentID == 14)
					{
						this.IdleAnim = "f02_idleLively_00";
						this.WalkAnim = "f02_walkLively_00";
						this.ClubAnim = "f02_stretch_00";
						if (!this.Rival)
						{
							this.Persona = PersonaType.Heroic;
						}
					}
					else if (this.StudentID == 15)
					{
						this.IdleAnim = "f02_idleHaughty_00";
						this.WalkAnim = "f02_walkHaughty_00";
						if (!this.Rival)
						{
							this.Persona = PersonaType.Loner;
						}
						if (this.StudentManager.MissionMode)
						{
							Debug.Log("Changing Rich Rival's lunch spot for Mission Mode.");
							this.StudentManager.EightiesLunchSpots.List[15].position = new Vector3(-18.73667f, 4f, 0f);
							this.StudentManager.EightiesLunchSpots.List[15].eulerAngles = new Vector3(0f, -90f, 0f);
						}
					}
					else if (this.StudentID == 16)
					{
						this.IdleAnim = "f02_idleConfident_01";
						this.WalkAnim = "f02_walkConfident_01";
						this.ClubAnim = "f02_vocalSingA_00";
						if (DateGlobals.Week != 6)
						{
							ScheduleBlock scheduleBlock34 = this.ScheduleBlocks[2];
							scheduleBlock34.destination = "Lyrics";
							scheduleBlock34.action = "Lyrics";
							ScheduleBlock scheduleBlock35 = this.ScheduleBlocks[7];
							scheduleBlock35.destination = "Lyrics";
							scheduleBlock35.action = "Lyrics";
						}
						if (!this.Rival)
						{
							this.Persona = PersonaType.LandlineUser;
						}
					}
					else if (this.StudentID == 17)
					{
						this.IdleAnim = "f02_idleCouncilGrace_00";
						this.WalkAnim = "f02_walkCouncilGrace_00";
						this.MyRenderer.SetBlendShapeWeight(0, 100f);
						if (!this.Rival)
						{
							this.Persona = PersonaType.TeachersPet;
						}
					}
					else if (this.StudentID == 18)
					{
						this.IdleAnim = "f02_idleGraceful_00";
						this.WalkAnim = "f02_walkGraceful_00";
						this.MyRenderer.SetBlendShapeWeight(0, 100f);
						if (!this.Rival)
						{
							this.Persona = PersonaType.TeachersPet;
						}
					}
					else if (this.StudentID == 19)
					{
						this.IdleAnim = "f02_idleElegant_00";
						this.WalkAnim = "f02_walkElegant_00";
						this.OriginalWalkAnim = "f02_walkElegant_00";
						this.OriginalOriginalWalkAnim = "f02_walkElegant_00";
						this.ClubAnim = this.GravureAnims[0];
						if (!this.Rival)
						{
							this.Persona = PersonaType.LandlineUser;
						}
					}
					else if (this.StudentID == 20)
					{
						this.IdleAnim = "f02_idleConfident_00";
						this.WalkAnim = "f02_walkConfident_00";
						this.Shovey = !this.StudentManager.RivalEliminated;
						if (!this.Rival)
						{
							this.Persona = PersonaType.Heroic;
						}
						this.Suffix = "Strict";
						this.IdleAnim = "f02_idleCouncil" + this.Suffix + "_00";
						this.WalkAnim = "f02_walkCouncil" + this.Suffix + "_00";
						this.ShoveAnim = "f02_pushCouncil" + this.Suffix + "_00";
						this.PatrolAnim = "f02_scanCouncil" + this.Suffix + "_00";
						this.RelaxAnim = "f02_relaxCouncil" + this.Suffix + "_00";
						this.SprayAnim = "f02_sprayCouncil" + this.Suffix + "_00";
						this.BreakUpAnim = "f02_stopCouncil" + this.Suffix + "_00";
						this.PickUpAnim = "f02_teacherPickUp_00";
					}
					else if (this.StudentID == 36)
					{
						this.BecomeSleuth();
						if (this.StudentManager.Atmosphere > 0.8f)
						{
							this.CharacterAnimation["f02_smile_00"].layer = 1;
							this.CharacterAnimation.Play("f02_smile_00");
							this.CharacterAnimation["f02_smile_00"].weight = 1f;
						}
					}
					if (this.StudentID > 35 && this.StudentID < 41 && this.StudentManager.MissionMode)
					{
						ScheduleBlock scheduleBlock36 = this.ScheduleBlocks[4];
						scheduleBlock36.destination = "LunchSpot";
						scheduleBlock36.action = "Eat";
					}
				}
				if (this.StudentID == 71 && this.Club == ClubType.Gardening && !this.PickPocket.gameObject.transform.parent.gameObject.activeInHierarchy)
				{
					this.PickPocket.gameObject.transform.parent.gameObject.SetActive(true);
				}
				if (this.StudentID == 66)
				{
					this.ClubThreshold = 100f;
				}
				if (this.StudentID > 10 && this.StudentID < 21)
				{
					this.OriginalPersona = this.Persona;
				}
				if (this.Club == ClubType.Delinquent)
				{
					if (this.Male)
					{
						this.CharacterAnimation[this.WalkAnim].speed += 0.01f * (float)(this.StudentID - 76);
					}
					else
					{
						this.Jaw.gameObject.name = this.Jaw.gameObject.name + "_RENAMED";
					}
				}
				if (this.StudentID == 20)
				{
					this.GuardID = 1;
				}
				else
				{
					this.GuardID = 20;
				}
			}
			this.OriginalWalkAnim = this.WalkAnim;
			if (StudentGlobals.GetStudentGrudge(this.StudentID))
			{
				if (this.Persona != PersonaType.Coward && this.Persona != PersonaType.Fragile && this.Persona != PersonaType.Evil && this.Club != ClubType.Delinquent)
				{
					this.CameraAnims = this.EvilAnims;
					this.Reputation.PendingRep -= 10f;
					this.PendingRep -= 10f;
					this.ID = 0;
					while (this.ID < this.Outlines.Length)
					{
						if (this.Outlines[this.ID] != null)
						{
							this.Outlines[this.ID].color = new Color(1f, 1f, 0f, 1f);
						}
						this.ID++;
					}
				}
				this.Grudge = true;
				this.CameraAnims = this.EvilAnims;
			}
			if (!this.StudentManager.MissionMode && this.Rival)
			{
				this.ID = 0;
				while (this.ID < this.Outlines.Length)
				{
					if (this.Outlines[this.ID] != null)
					{
						this.Outlines[this.ID].color = new Color(10f, 0f, 0f, 1f);
					}
					this.ID++;
				}
			}
			if (this.Persona == PersonaType.Sleuth)
			{
				if (SchoolGlobals.SchoolAtmosphere <= 0.8f || this.Grudge)
				{
					bool flag = false;
					if (this.StudentManager.Eighties && this.ClubLeader)
					{
						Debug.Log("Student #" + this.StudentID.ToString() + " is a club leader, so they shouldn't become a Sleuth...");
						flag = true;
					}
					if (!flag)
					{
						this.BecomeSleuth();
					}
				}
				else if (SchoolGlobals.SchoolAtmosphere <= 0.9f)
				{
					this.WalkAnim = this.ParanoidWalkAnim;
					this.CameraAnims = this.HeroAnims;
				}
			}
			if (!this.Slave)
			{
				if (this.StudentManager.Bullies > 1)
				{
					if (this.StudentID == 81 || this.StudentID == 83 || this.StudentID == 85)
					{
						if (this.Persona != PersonaType.Coward)
						{
							this.Pathfinding.canSearch = false;
							this.Pathfinding.canMove = false;
							this.Paired = true;
							this.CharacterAnimation["f02_walkTalk_00"].time += (float)(this.StudentID - 81);
							this.WalkAnim = "f02_walkTalk_00";
							this.SpeechLines.Play();
						}
					}
					else if (this.StudentID == 82 || this.StudentID == 84)
					{
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.Paired = true;
						this.CharacterAnimation["f02_walkTalk_01"].time += (float)(this.StudentID - 81);
						this.WalkAnim = "f02_walkTalk_01";
						this.SpeechLines.Play();
					}
				}
				if (this.Club == ClubType.Delinquent)
				{
					if (this.Friend)
					{
						this.RespectEarned = true;
					}
					if (CounselorGlobals.WeaponsBanned == 0)
					{
						this.MyWeapon = this.Yandere.WeaponManager.DelinquentWeapons[this.StudentID - 75];
						this.MyWeapon.transform.parent = this.WeaponBagParent;
						this.MyWeapon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
						this.MyWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
						this.MyWeapon.FingerprintID = this.StudentID;
						this.MyWeapon.MyCollider.enabled = false;
						this.WeaponBag.SetActive(true);
					}
					else
					{
						this.OriginalPersona = PersonaType.Heroic;
						this.Persona = PersonaType.Heroic;
					}
					string str = "";
					if (!this.Male)
					{
						str = "f02_";
					}
					this.ScaredAnim = str + "delinquentCombatIdle_00";
					this.LeanAnim = "delinquentConcern_00";
					this.ShoveAnim = str + "pushTough_00";
					this.WalkAnim = str + "walkTough_00";
					this.IdleAnim = str + "idleTough_00";
					this.SpeechLines = this.DelinquentSpeechLines;
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.Paired = true;
					this.TaskAnims[0] = str + "delinquentTalk_04";
					this.TaskAnims[1] = str + "delinquentTalk_01";
					this.TaskAnims[2] = str + "delinquentTalk_02";
					this.TaskAnims[3] = str + "delinquentTalk_03";
					this.TaskAnims[4] = str + "delinquentTalk_00";
					this.TaskAnims[5] = str + "delinquentTalk_03";
					this.WaveAnim = "f02_casualWave_04";
				}
			}
			else
			{
				this.Club = ClubType.None;
			}
			if (this.Rival)
			{
				this.RivalPrefix = "Rival ";
				if (DateGlobals.Weekday == DayOfWeek.Friday)
				{
					this.ScheduleBlocks[7].time = 17f;
				}
			}
			if (!this.Teacher && this.Name != "Random")
			{
				this.StudentManager.CleaningManager.GetRole(this.StudentID);
				this.CleaningSpot = this.StudentManager.CleaningManager.Spot;
				this.CleaningRole = this.StudentManager.CleaningManager.Role;
			}
			if (this.OriginalClub == ClubType.Cooking)
			{
				if (this.StudentID > 12)
				{
					this.ClubMemberID = this.StudentID - 20;
				}
				this.MyPlate = this.StudentManager.Plates[this.ClubMemberID];
			}
			if (this.Club == ClubType.Cooking)
			{
				this.SleuthID = (this.StudentID - 21) * 20;
				this.ClubAnim = this.PrepareFoodAnim;
				if (this.StudentID > 11)
				{
					if (this.StudentID == 12)
					{
						this.ClubMemberID = 0;
					}
					else
					{
						this.ClubMemberID = this.StudentID - 20;
					}
					this.MyPlate = this.StudentManager.Plates[this.ClubMemberID];
					this.OriginalPlatePosition = this.MyPlate.position;
					this.OriginalPlateRotation = this.MyPlate.rotation;
				}
				if (!this.StudentManager.EmptyDemon && !this.StudentManager.TutorialActive)
				{
					this.ApronAttacher.enabled = true;
				}
			}
			else if (this.Club == ClubType.Drama)
			{
				if (this.StudentID == 26)
				{
					if (this.Male)
					{
						this.ClubAnim = "teaching_00";
					}
					else
					{
						this.ClubAnim = "f02_teaching_00";
					}
				}
				else if (this.Male)
				{
					this.ClubAnim = "sit_00";
				}
				else
				{
					this.ClubAnim = "f02_sit_00";
				}
				this.OriginalClubAnim = this.ClubAnim;
			}
			else if (this.Club == ClubType.Occult)
			{
				if (!this.Male)
				{
					this.WaveAnim = "f02_casualWave_04";
					this.PatrolAnim = "f02_pondering_00";
					this.CharacterAnimation[this.PatrolAnim].speed = 0.7692f;
				}
				else
				{
					this.PatrolAnim = "pondering_00";
					this.CharacterAnimation[this.PatrolAnim].speed = 0.7692f;
				}
			}
			else if (this.Club == ClubType.Gaming)
			{
				this.MiyukiGameScreen.SetActive(true);
				this.ClubMemberID = this.StudentID - 35;
				if (this.StudentID > 36)
				{
					this.ClubAnim = this.GameAnim;
				}
				this.ActivityAnim = this.GameAnim;
			}
			else if (this.Club == ClubType.Art)
			{
				this.ChangingBooth = this.StudentManager.ChangingBooths[4];
				this.ActivityAnim = this.PaintAnim;
				this.Attacher.ArtClub = true;
				this.ClubAnim = this.PaintAnim;
				this.DressCode = true;
				if (this.StudentManager.Week == 1 && DateGlobals.Weekday == DayOfWeek.Friday)
				{
					ScheduleBlock scheduleBlock37 = this.ScheduleBlocks[7];
					scheduleBlock37.destination = "Paint";
					scheduleBlock37.action = "Paint";
					this.VisionDistance += 5f;
				}
				this.ClubMemberID = this.StudentID - 40;
				this.Painting = this.StudentManager.FridayPaintings[this.ClubMemberID];
				this.GetDestinations();
			}
			else if (this.OriginalClub == ClubType.LightMusic)
			{
				this.ClubMemberID = this.StudentID - 50;
				if (!this.Slave)
				{
					this.InstrumentBag[this.ClubMemberID].SetActive(true);
				}
				if (this.StudentID == 51)
				{
					if (!this.Male)
					{
						this.ClubAnim = "f02_practiceGuitar_01";
					}
					else
					{
						this.ClubAnim = "practiceGuitar_01";
					}
					if (ClubGlobals.GetClubClosed(ClubType.LightMusic))
					{
						this.InstrumentBag[this.ClubMemberID].SetActive(false);
					}
				}
				else if (this.StudentID == 52 || this.StudentID == 53)
				{
					if (this.Male)
					{
						this.ClubAnim = "practiceGuitar_00";
					}
					else
					{
						this.ClubAnim = "f02_practiceGuitar_00";
					}
				}
				else if (this.StudentID == 54)
				{
					this.ClubAnim = "f02_practiceDrums_00";
					this.Instruments[4] = this.StudentManager.DrumSet;
					if (this.StudentManager.Eighties)
					{
						this.InstrumentBag[this.ClubMemberID].GetComponent<Renderer>().enabled = false;
						this.Instruments[this.ClubMemberID].GetComponent<Renderer>().enabled = false;
					}
				}
				else if (this.StudentID == 55)
				{
					this.ClubAnim = "f02_practiceKeytar_00";
				}
				if (this.StudentManager.Eighties && this.StudentManager.Students[16] != null && !GameGlobals.CustomMode && DateGlobals.Week == 6)
				{
					this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().enabled = false;
					this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().volume = 0f;
					if (this.StudentID == 52)
					{
						this.ClubAnim = "f02_guitarPlayA_00";
					}
					else if (this.StudentID == 53)
					{
						this.ClubAnim = "f02_guitarPlayB_00";
					}
					else if (this.StudentID == 54)
					{
						this.ClubAnim = "f02_drumsPlay_00";
					}
					else if (this.StudentID == 55)
					{
						this.ClubAnim = "f02_keysPlay_00";
					}
				}
			}
			else if (this.OriginalClub == ClubType.MartialArts)
			{
				this.ChangingBooth = this.StudentManager.ChangingBooths[6];
				this.DressCode = true;
				if (this.StudentID == 46)
				{
					if (this.Male)
					{
						this.IdleAnim = "pose_03";
						this.ClubAnim = "pose_03";
					}
					else
					{
						this.ClubAnim = this.IdleAnim;
					}
					this.ActivityAnim = this.IdleAnim;
				}
				else if (this.StudentID == 47)
				{
					this.GetNewAnimation = true;
					if (this.Male)
					{
						this.ClubAnim = "idle_20";
						this.ActivityAnim = "kick_24";
					}
					else
					{
						this.ClubAnim = "f02_idle_20";
						this.ActivityAnim = "f02_kick_23";
					}
				}
				else if (this.StudentID == 48)
				{
					if (this.Male)
					{
						this.ClubAnim = "sit_04";
						this.ActivityAnim = "kick_24";
					}
					else
					{
						this.ClubAnim = "f02_sit_05";
						this.ActivityAnim = "f02_kick_23";
					}
				}
				else if (this.StudentID == 49)
				{
					this.GetNewAnimation = true;
					if (this.Male)
					{
						this.ClubAnim = "idle_20";
						this.ActivityAnim = "kick_24";
					}
					else
					{
						this.ClubAnim = "f02_idle_20";
						this.ActivityAnim = "f02_kick_23";
					}
				}
				else if (this.StudentID == 50)
				{
					if (this.Male)
					{
						this.ClubAnim = "sit_04";
						this.ActivityAnim = "kick_24";
					}
					else
					{
						this.ClubAnim = "f02_sit_05";
						this.ActivityAnim = "f02_kick_23";
					}
				}
				this.ClubMemberID = this.StudentID - 45;
			}
			else if (this.Club == ClubType.Science)
			{
				if (!this.StudentManager.Eighties)
				{
					this.ChangingBooth = this.StudentManager.ChangingBooths[8];
					this.Attacher.ScienceClub = true;
					this.DressCode = true;
					if (this.StudentID == 61)
					{
						this.ClubAnim = "scienceMad_00";
					}
					else if (this.StudentID == 62)
					{
						this.ClubAnim = "scienceTablet_00";
					}
					else if (this.StudentID == 63)
					{
						this.ClubAnim = "scienceChemistry_00";
					}
					else if (this.StudentID == 64)
					{
						this.ClubAnim = "f02_scienceLeg_00";
					}
					else if (this.StudentID == 65)
					{
						this.ClubAnim = "f02_scienceConsole_00";
						if (this.StudentManager.RobotPhase == 2)
						{
							this.ClubAnim = "f02_brokenSit_00";
						}
					}
				}
				else if (this.Male)
				{
					this.ClubAnim = "scienceChemistry_00";
				}
				else
				{
					this.ClubAnim = "f02_scienceChemistry_00";
				}
				this.ClubMemberID = this.StudentID - 60;
			}
			else if (this.OriginalClub == ClubType.Sports)
			{
				if (this.Male)
				{
					this.ChangingBooth = this.StudentManager.ChangingBooths[9];
					this.ClubAnim = "stretch_00";
				}
				else
				{
					this.ChangingBooth = this.StudentManager.ChangingBooths[10];
					this.ClubAnim = "f02_stretch_00";
				}
				this.DressCode = true;
				this.ClubMemberID = this.StudentID - 65;
			}
			else if (this.OriginalClub == ClubType.Gardening)
			{
				if (!this.StudentManager.Eighties)
				{
					if (this.StudentID == 71)
					{
						this.PatrolAnim = "f02_thinking_00";
						this.ClubAnim = "f02_thinking_00";
						this.CharacterAnimation[this.PatrolAnim].speed = 0.9f;
					}
					else if (ClubGlobals.GetClubClosed(ClubType.Gardening))
					{
						this.ClubAnim = "f02_arrangeFlowers_00";
						this.WateringCan.SetActive(false);
					}
					else
					{
						this.ClubAnim = "f02_waterPlant_00";
						this.WateringCan.SetActive(true);
					}
				}
				else
				{
					if (this.Male)
					{
						this.PatrolAnim = "thinking_00";
						this.ClubAnim = "thinking_00";
					}
					else
					{
						this.PatrolAnim = "f02_thinking_00";
						this.ClubAnim = "f02_thinking_00";
					}
					this.CharacterAnimation[this.PatrolAnim].speed = 0.9f;
				}
			}
			else if (this.Club == ClubType.Newspaper)
			{
				if (this.StudentID == 36)
				{
					if (this.Male)
					{
						this.ClubAnim = "thinking_00";
					}
					else
					{
						this.ClubAnim = "f02_pondering_00";
					}
				}
				else if (this.Male)
				{
					this.PatrolAnim = "thinking_00";
					this.ClubAnim = "onComputer_00";
				}
				else
				{
					this.PatrolAnim = "f02_thinking_00";
					this.ClubAnim = "f02_onComputer_00";
				}
				this.OriginalClubAnim = this.ClubAnim;
			}
			if (this.OriginalClub != ClubType.Gaming)
			{
				this.Miyuki.gameObject.SetActive(false);
			}
			if (this.Cosmetic.Hairstyle == 20)
			{
				this.IdleAnim = "f02_tsunIdle_00";
			}
			this.GetDestinations();
			this.OriginalActions = new StudentActionType[this.Actions.Length];
			Array.Copy(this.Actions, this.OriginalActions, this.Actions.Length);
			if (this.AoT)
			{
				this.AttackOnTitan();
			}
			if (this.Slave)
			{
				this.PoolStairs = GameObject.Find("PoolStairs").GetComponent<Collider>();
				this.NEStairs = GameObject.Find("NEStairs").GetComponent<Collider>();
				this.NWStairs = GameObject.Find("NWStairs").GetComponent<Collider>();
				this.SEStairs = GameObject.Find("SEStairs").GetComponent<Collider>();
				this.SWStairs = GameObject.Find("SWStairs").GetComponent<Collider>();
				this.IdleAnim = this.BrokenAnim;
				this.WalkAnim = this.BrokenWalkAnim;
				this.RightEmptyEye.SetActive(true);
				this.LeftEmptyEye.SetActive(true);
				this.SmartPhone.SetActive(false);
				this.Distracted = true;
				this.Indoors = true;
				this.Safe = false;
				this.Shy = false;
				this.ID = 0;
				while (this.ID < this.ScheduleBlocks.Length)
				{
					this.ScheduleBlocks[this.ID].time = 0f;
					this.ID++;
				}
				if (this.FragileSlave)
				{
					this.HuntTarget = this.StudentManager.Students[StudentGlobals.FragileTarget];
				}
			}
			if (this.Spooky)
			{
				this.Spook();
			}
			this.Prompt.HideButton[0] = true;
			this.Prompt.HideButton[2] = true;
			this.ID = 0;
			while (this.ID < this.Ragdoll.AllRigidbodies.Length)
			{
				this.Ragdoll.AllRigidbodies[this.ID].isKinematic = true;
				this.Ragdoll.AllColliders[this.ID].enabled = false;
				this.ID++;
			}
			this.Ragdoll.AllColliders[10].enabled = true;
			if (this.StudentID == 1)
			{
				this.Yandere.Senpai = base.transform;
				this.Yandere.LookAt.target = this.Head;
				this.ID = 0;
				while (this.ID < this.Outlines.Length)
				{
					if (this.Outlines[this.ID] != null)
					{
						this.Outlines[this.ID].enabled = true;
						this.Outlines[this.ID].color = new Color(1f, 0f, 1f);
					}
					this.ID++;
				}
				this.Prompt.ButtonActive[0] = false;
				this.Prompt.ButtonActive[1] = false;
				this.Prompt.ButtonActive[2] = false;
				this.Prompt.ButtonActive[3] = false;
				if (this.StudentManager.MissionMode || GameGlobals.SenpaiMourning)
				{
					base.transform.position = Vector3.zero;
					base.gameObject.SetActive(false);
				}
			}
			else if ((this.StudentManager.StudentPhotographed[this.StudentID] && !this.Rival) || (this.Friend && !this.Rival))
			{
				this.TurnOutlinesGreen();
			}
			else
			{
				this.ID = 0;
				while (this.ID < this.Outlines.Length)
				{
					if (this.Outlines[this.ID] != null)
					{
						this.Outlines[this.ID].enabled = false;
					}
					this.ID++;
				}
			}
			if (this.StudentManager.MissionMode)
			{
				if (this.StudentID == 11)
				{
					this.ID = 0;
					while (this.ID < this.Outlines.Length)
					{
						if (this.Outlines[this.ID] != null)
						{
							this.Outlines[this.ID].enabled = false;
						}
						this.ID++;
					}
				}
				for (int j = 1; j < 11; j++)
				{
					if (PlayerPrefs.GetInt("MissionModeTarget" + j.ToString()) == this.StudentID)
					{
						this.ID = 0;
						while (this.ID < this.Outlines.Length)
						{
							if (this.Outlines[this.ID] != null)
							{
								this.Outlines[this.ID].color = new Color(10f, 0f, 0f, 1f);
								this.Outlines[this.ID].enabled = true;
							}
							this.ID++;
						}
					}
				}
			}
			this.PickRandomAnim();
			this.PickRandomSleuthAnim();
			Renderer component = this.Armband.GetComponent<Renderer>();
			if (this.Club != ClubType.None && (this.StudentID == 21 || this.StudentID == 26 || this.StudentID == 31 || this.StudentID == 36 || this.StudentID == 41 || this.StudentID == 46 || this.StudentID == 51 || this.StudentID == 56 || this.StudentID == 61 || this.StudentID == 66 || this.StudentID == 71))
			{
				this.Armband.SetActive(true);
				this.ClubLeader = true;
				if (this.StudentID == 21 && this.StudentManager.Students[12] != null)
				{
					this.Armband.SetActive(false);
					this.ClubLeader = false;
				}
				if (this.StudentManager.EmptyDemon)
				{
					this.IdleAnim = this.OriginalIdleAnim;
					this.WalkAnim = this.OriginalOriginalWalkAnim;
					this.OriginalPersona = PersonaType.Evil;
					this.Persona = PersonaType.Evil;
					this.ScaredAnim = this.EvilWitnessAnim;
				}
			}
			if (!this.Teacher)
			{
				this.CurrentDestination = this.Destinations[this.Phase];
				this.Pathfinding.target = this.Destinations[this.Phase];
			}
			else
			{
				this.Indoors = true;
			}
			if (this.StudentID == 1 || this.StudentID == 4 || this.StudentID == 5 || this.StudentID == 11)
			{
				this.BookRenderer.material.mainTexture = this.RedBookTexture;
			}
			this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
			if ((this.StudentManager.MissionMode && this.StudentID == MissionModeGlobals.MissionTarget) || (!this.StudentManager.MissionMode && this.Rival))
			{
				this.ID = 0;
				while (this.ID < this.Outlines.Length)
				{
					if (this.Outlines[this.ID] != null)
					{
						this.Outlines[this.ID].enabled = true;
						this.Outlines[this.ID].color = new Color(1f, 0f, 0f);
					}
					this.ID++;
				}
			}
			UnityEngine.Object.Destroy(this.MyRigidbody);
			this.Started = true;
			if (this.Club == ClubType.Council && !this.Slave)
			{
				if (GameGlobals.AlternateTimeline || GameGlobals.NoCouncilShove)
				{
					this.DoNotShove = true;
				}
				component.material.SetTextureOffset("_MainTex", new Vector2(0.285f, 0f));
				this.Armband.SetActive(true);
				this.Indoors = true;
				this.Spawned = true;
				if (this.ShoeRemoval.Locker == null)
				{
					this.ShoeRemoval.Start();
				}
				this.ShoeRemoval.PutOnShoes();
				if (this.StudentID == 86)
				{
					this.Suffix = "Strict";
				}
				else if (this.StudentID == 87)
				{
					this.Suffix = "Casual";
				}
				else if (this.StudentID == 88)
				{
					this.Suffix = "Grace";
				}
				else if (this.StudentID == 89)
				{
					this.Suffix = "Edgy";
				}
				if (!this.StudentManager.Eighties || !this.Male)
				{
					this.IdleAnim = "f02_idleCouncil" + this.Suffix + "_00";
					this.WalkAnim = "f02_walkCouncil" + this.Suffix + "_00";
					this.ShoveAnim = "f02_pushCouncil" + this.Suffix + "_00";
					this.PatrolAnim = "f02_scanCouncil" + this.Suffix + "_00";
					this.RelaxAnim = "f02_relaxCouncil" + this.Suffix + "_00";
					this.SprayAnim = "f02_sprayCouncil" + this.Suffix + "_00";
					this.BreakUpAnim = "f02_stopCouncil" + this.Suffix + "_00";
					this.PickUpAnim = "f02_teacherPickUp_00";
					if (this.StudentID == 89)
					{
						this.MyController.radius = 0.04f;
					}
				}
				else
				{
					this.IdleAnim = idleAnim;
					this.BreakUpAnim = "delinquentTalk_03";
					this.GuardAnim = this.ReadyToFightAnim;
					this.RelaxAnim = "sit_00";
					if (this.StudentID == 86)
					{
						this.ShoveAnim = "coldPush_00";
					}
					else if (this.StudentID == 87)
					{
						this.ShoveAnim = "foreignPush_00";
					}
					else if (this.StudentID == 88)
					{
						this.ShoveAnim = "humblePush_00";
						this.CharacterAnimation["humblePush_00"].speed = 1.33333f;
					}
					else if (this.StudentID == 89)
					{
						this.RelaxAnim = "crossarms_00";
						this.ShoveAnim = "toughPush_00";
					}
				}
				this.ScaredAnim = this.ReadyToFightAnim;
				this.ParanoidAnim = this.GuardAnim;
				this.CameraAnims[1] = this.IdleAnim;
				this.CameraAnims[2] = this.IdleAnim;
				this.CameraAnims[3] = this.IdleAnim;
				this.ClubMemberID = this.StudentID - 85;
				this.VisionDistance *= 2f;
				if (this.StudentManager.MissionMode)
				{
					Debug.Log("Changing Student Council routine for Mission Mode.");
					ScheduleBlock scheduleBlock38 = this.ScheduleBlocks[4];
					scheduleBlock38.destination = "LunchSpot";
					scheduleBlock38.action = "Eat";
					this.GetDestinations();
				}
			}
			if (this.StudentID == 81 && StudentGlobals.GetStudentBroken(81))
			{
				this.Destinations[2] = this.StudentManager.BrokenSpot;
				this.Destinations[4] = this.StudentManager.BrokenSpot;
				this.Actions[2] = StudentActionType.Shamed;
				this.Actions[4] = StudentActionType.Shamed;
			}
		}
		this.UpdateAnimLayers();
		if (!this.Male)
		{
			if (this.StudentID == 40)
			{
				this.LongHair[0] = this.LongHair[2];
			}
			if (this.StudentID != 55 && this.StudentID != 40)
			{
				this.LongHair[0] = null;
				this.LongHair[1] = null;
				this.LongHair[2] = null;
			}
		}
		if (this.StudentID == 90)
		{
			this.PatrolAnim = "f02_nurseThink_00";
		}
		this.SetOriginalScheduleBlocks();
		if (this.StudentManager.Randomize)
		{
			this.OriginalPersona = PersonaType.Coward;
			this.Persona = PersonaType.Coward;
		}
		if (this.StudentManager.Atmosphere < 0.33333f && this.Teacher)
		{
			this.OriginalIdleAnim = "f02_idleShort_00";
			this.IdleAnim = "f02_idleShort_00";
			if (this.StudentID == 97)
			{
				this.OriginalIdleAnim = "f02_tsunIdle_00";
				this.IdleAnim = "f02_tsunIdle_00";
			}
		}
		if (this.Club != ClubType.Bully)
		{
			this.MiyukiGameScreen.SetActive(true);
			if (this.StudentID <= 35 || this.StudentID >= 41)
			{
				this.MiyukiGameScreen.GetComponent<Renderer>().material.mainTexture = this.SocialMediaTexture;
			}
			this.SmartPhone.transform.localPosition = new Vector3(0.02f, 0.01f, 0.025f);
			this.SmartPhone.transform.localEulerAngles = new Vector3(15f, -145f, 180f);
		}
		if (this.StudentManager.YandereLate || this.StudentManager.MissionMode)
		{
			this.Paired = false;
		}
		if (this.OriginalClub == ClubType.Occult || this.OriginalClub == ClubType.Delinquent || (!this.StudentManager.Eighties && this.StudentID == 4) || (!this.StudentManager.Eighties && this.StudentID == 36) || (!this.StudentManager.Eighties && this.StudentID == 41) || (!this.StudentManager.Eighties && this.StudentID == 42) || (!this.StudentManager.Eighties && this.StudentID == 66))
		{
			this.WaveAnim = "f02_casualWave_04";
		}
		if (this.StudentManager.CustomMode)
		{
			if (this.StudentManager.JSON.Misc.AnimSet[this.StudentID] < this.IdleAnims.Length)
			{
				this.IdleAnim = this.IdleAnims[this.StudentManager.JSON.Misc.AnimSet[this.StudentID]];
				this.WalkAnim = this.WalkAnims[this.StudentManager.JSON.Misc.AnimSet[this.StudentID]];
				this.AnimSetID = this.StudentManager.JSON.Misc.AnimSet[this.StudentID];
			}
			if (this.Slave)
			{
				this.IdleAnim = this.BrokenAnim;
				this.WalkAnim = this.BrokenWalkAnim;
			}
			idleAnim = this.IdleAnim;
			this.OriginalWalkAnim = this.WalkAnim;
			if (this.ClubAnim == "")
			{
				this.ClubAnim = this.ThinkAnim;
			}
			if (this.GuardAnim == "")
			{
				if (this.Male)
				{
					this.GuardAnim = "guardCorpse_00";
				}
				else
				{
					this.GuardAnim = "f02_guardCorpse_00";
				}
			}
			if (this.CustomPatrolAnim == "")
			{
				this.CustomPatrolAnim = this.ThinkAnim;
			}
			if (this.CustomHangoutAnim == "")
			{
				this.CustomHangoutAnim = this.ThinkAnim;
			}
			if (this.RelaxAnim == "")
			{
				this.RelaxAnim = this.ThinkAnim;
			}
		}
		for (int k = 1; k < 11; k++)
		{
			if (this.StudentID == this.StudentManager.SuitorIDs[k])
			{
				this.CuddlePartnerID = 1;
			}
		}
		if (this.Yandere.PauseScreen.MissionMode.YakuzaMode && this.Strength == 7)
		{
			this.Strength = 5;
		}
		if (this.Slave)
		{
			this.Armband.SetActive(false);
		}
		this.CharacterAnimation.Sample();
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x001CBEE8 File Offset: 0x001CA0E8
	private float GetPerceptionPercent(float distance)
	{
		float num = Mathf.Clamp01(distance / this.VisionDistance);
		return 1f - num * num;
	}

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x06002154 RID: 8532 RVA: 0x001CBF0C File Offset: 0x001CA10C
	private SubtitleType LostPhoneSubtitleType
	{
		get
		{
			if (this.RivalPrefix == string.Empty)
			{
				return SubtitleType.LostPhone;
			}
			if (this.RivalPrefix == "Rival ")
			{
				return SubtitleType.RivalLostPhone;
			}
			throw new NotImplementedException("\"" + this.RivalPrefix + "\" case not implemented.");
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x06002155 RID: 8533 RVA: 0x001CBF60 File Offset: 0x001CA160
	private SubtitleType PickpocketSubtitleType
	{
		get
		{
			Debug.Log("This is where the game determines what pickpocket subtitle to use.");
			if (this.Male)
			{
				this.Subtitle.CustomText = "Hey! Are you trying to take my keys? Knock it off!";
				return SubtitleType.Custom;
			}
			if (this.RivalPrefix == string.Empty)
			{
				return SubtitleType.PickpocketReaction;
			}
			if (this.RivalPrefix == "Rival ")
			{
				return SubtitleType.RivalPickpocketReaction;
			}
			throw new NotImplementedException("\"" + this.RivalPrefix + "\" case not implemented.");
		}
	}

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x06002156 RID: 8534 RVA: 0x001CBFDC File Offset: 0x001CA1DC
	private SubtitleType SplashSubtitleType
	{
		get
		{
			if (this.RivalPrefix == string.Empty)
			{
				if (!this.Male)
				{
					return SubtitleType.SplashReaction;
				}
				return SubtitleType.SplashReactionMale;
			}
			else
			{
				if (this.RivalPrefix == "Rival ")
				{
					return SubtitleType.RivalSplashReaction;
				}
				throw new NotImplementedException("\"" + this.RivalPrefix + "\" case not implemented.");
			}
		}
	}

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x06002157 RID: 8535 RVA: 0x001CC044 File Offset: 0x001CA244
	public SubtitleType TaskLineResponseType
	{
		get
		{
			bool flag = false;
			if (this.StudentManager.Eighties)
			{
				flag = true;
				if ((this.StudentID > 10 && this.StudentID < 21) || this.StudentID == 79)
				{
					flag = false;
				}
				if (this.StudentManager.CustomMode && ((this.StudentID > 10 && this.StudentID < 21) || this.StudentID == 79))
				{
					flag = true;
				}
			}
			if (this.StudentManager.Eighties && flag)
			{
				if (this.GenericTaskID == 1)
				{
					return SubtitleType.TaskGenericEightiesLine1;
				}
				if (this.GenericTaskID == 2)
				{
					return SubtitleType.TaskGenericEightiesLine2;
				}
				if (this.GenericTaskID == 3)
				{
					return SubtitleType.TaskGenericEightiesLine3;
				}
				if (this.GenericTaskID == 4)
				{
					return SubtitleType.TaskGenericEightiesLine4;
				}
				if (this.GenericTaskID == 5)
				{
					return SubtitleType.TaskGenericEightiesLine5;
				}
				if (this.GenericTaskID == 6)
				{
					return SubtitleType.TaskGenericEightiesLine6;
				}
				if (this.GenericTaskID == 7)
				{
					return SubtitleType.TaskGenericEightiesLine7;
				}
				if (this.GenericTaskID == 8)
				{
					return SubtitleType.TaskGenericEightiesLine8;
				}
				if (this.GenericTaskID == 9)
				{
					return SubtitleType.TaskGenericEightiesLine9;
				}
				if (this.GenericTaskID == 10)
				{
					return SubtitleType.TaskGenericEightiesLine10;
				}
				Debug.Log("This student doesn't have a Generic Task ID assigned.");
				return SubtitleType.TaskGenericEightiesLine1;
			}
			else
			{
				if (this.StudentID == 4)
				{
					return SubtitleType.Task4Line;
				}
				if (this.StudentID == 6)
				{
					return SubtitleType.Task6Line;
				}
				if (this.StudentID == 8)
				{
					return SubtitleType.Task8Line;
				}
				if (this.StudentID == 11)
				{
					return SubtitleType.Task11Line;
				}
				if (this.StudentID == 12)
				{
					return SubtitleType.Task12Line;
				}
				if (this.StudentID == 13)
				{
					return SubtitleType.Task13Line;
				}
				if (this.StudentID == 14)
				{
					return SubtitleType.Task14Line;
				}
				if (this.StudentID == 15)
				{
					return SubtitleType.Task15Line;
				}
				if (this.StudentID == 16)
				{
					return SubtitleType.Task16Line;
				}
				if (this.StudentID == 17)
				{
					return SubtitleType.Task17Line;
				}
				if (this.StudentID == 18)
				{
					return SubtitleType.Task18Line;
				}
				if (this.StudentID == 19)
				{
					return SubtitleType.Task19Line;
				}
				if (this.StudentID == 20)
				{
					return SubtitleType.Task20Line;
				}
				if (this.StudentID == 21)
				{
					return SubtitleType.Task21Line;
				}
				if (this.StudentID == 22)
				{
					return SubtitleType.Task22Line;
				}
				if (this.StudentID == 23)
				{
					return SubtitleType.Task23Line;
				}
				if (this.StudentID == 24)
				{
					return SubtitleType.Task24Line;
				}
				if (this.StudentID == 25)
				{
					return SubtitleType.Task25Line;
				}
				if (this.StudentID == 28)
				{
					return SubtitleType.Task28Line;
				}
				if (this.StudentID == 30)
				{
					return SubtitleType.Task30Line;
				}
				if (this.StudentID == 36)
				{
					return SubtitleType.Task36Line;
				}
				if (this.StudentID == 37)
				{
					return SubtitleType.Task37Line;
				}
				if (this.StudentID == 38)
				{
					return SubtitleType.Task38Line;
				}
				if (this.StudentID == 41)
				{
					return SubtitleType.Task41Line;
				}
				if (this.StudentID == 46)
				{
					return SubtitleType.Task46Line;
				}
				if (this.StudentID == 47)
				{
					return SubtitleType.Task47Line;
				}
				if (this.StudentID == 48)
				{
					return SubtitleType.Task48Line;
				}
				if (this.StudentID == 49)
				{
					return SubtitleType.Task49Line;
				}
				if (this.StudentID == 50)
				{
					return SubtitleType.Task50Line;
				}
				if (this.StudentID == 52)
				{
					return SubtitleType.Task52Line;
				}
				if (this.StudentID == 65)
				{
					return SubtitleType.Task65Line;
				}
				if (this.StudentID == 76)
				{
					return SubtitleType.Task76Line;
				}
				if (this.StudentID == 77)
				{
					return SubtitleType.Task77Line;
				}
				if (this.StudentID == 78)
				{
					return SubtitleType.Task78Line;
				}
				if (this.StudentID == 79)
				{
					return SubtitleType.Task79Line;
				}
				if (this.StudentID == 80)
				{
					return SubtitleType.Task80Line;
				}
				if (this.StudentID == 81)
				{
					return SubtitleType.Task81Line;
				}
				return SubtitleType.TaskGenericLine;
			}
		}
	}

	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06002158 RID: 8536 RVA: 0x001CC3BC File Offset: 0x001CA5BC
	public SubtitleType ClubInfoResponseType
	{
		get
		{
			if (this.StudentManager.EmptyDemon)
			{
				return SubtitleType.ClubPlaceholderInfo;
			}
			if (this.Club == ClubType.Cooking)
			{
				return SubtitleType.ClubCookingInfo;
			}
			if (this.Club == ClubType.Drama)
			{
				return SubtitleType.ClubDramaInfo;
			}
			if (this.Club == ClubType.Occult)
			{
				return SubtitleType.ClubOccultInfo;
			}
			if (this.Club == ClubType.Art)
			{
				return SubtitleType.ClubArtInfo;
			}
			if (this.Club == ClubType.LightMusic)
			{
				return SubtitleType.ClubLightMusicInfo;
			}
			if (this.Club == ClubType.MartialArts)
			{
				return SubtitleType.ClubMartialArtsInfo;
			}
			if (this.Club == ClubType.Photography)
			{
				if (this.Sleuthing)
				{
					return SubtitleType.ClubPhotoInfoDark;
				}
				return SubtitleType.ClubPhotoInfoLight;
			}
			else
			{
				if (this.Club == ClubType.Science)
				{
					return SubtitleType.ClubScienceInfo;
				}
				if (this.Club == ClubType.Sports)
				{
					return SubtitleType.ClubSportsInfo;
				}
				if (this.Club == ClubType.Gardening)
				{
					return SubtitleType.ClubGardenInfo;
				}
				if (this.Club == ClubType.Gaming)
				{
					return SubtitleType.ClubGamingInfo;
				}
				if (this.Club == ClubType.Delinquent)
				{
					return SubtitleType.ClubDelinquentInfo;
				}
				if (this.Club == ClubType.Newspaper)
				{
					return SubtitleType.ClubNewspaperInfo;
				}
				return SubtitleType.ClubPlaceholderInfo;
			}
		}
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x001CC488 File Offset: 0x001CA688
	private bool PointIsInFOV(Vector3 point)
	{
		Vector3 b = new Vector3(base.transform.position.x, this.Eyes.transform.position.y, base.transform.position.z);
		Vector3 to = point - b;
		float num = 90f;
		bool teacher = this.Teacher;
		return Vector3.Angle(this.Head.transform.forward, to) <= num;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x001CC504 File Offset: 0x001CA704
	public bool SeenByYandere()
	{
		Debug.Log("A ''SeenByYandere'' check is occuring.");
		Debug.DrawLine(this.Yandere.transform.position + new Vector3(0f, 1f, 0f), base.transform.position + new Vector3(0f, 1f, 0f), Color.red);
		RaycastHit raycastHit;
		if (Physics.Linecast(this.Yandere.transform.position + new Vector3(0f, 1f, 0f), base.transform.position + new Vector3(0f, 1f, 0f), out raycastHit, this.YandereCheckMask))
		{
			Debug.Log("Yandere-chan's raycast hit: " + raycastHit.collider.gameObject.name);
			if (raycastHit.collider.gameObject == this.Head.gameObject || raycastHit.collider.gameObject == base.gameObject)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x001CC630 File Offset: 0x001CA830
	public bool CanSeeObject(GameObject obj, Vector3 targetPoint, int[] layers, int mask)
	{
		Vector3 vector = new Vector3(base.transform.position.x, this.Eyes.transform.position.y, base.transform.position.z);
		Vector3 vector2 = targetPoint - vector;
		if (this.PointIsInFOV(targetPoint))
		{
			float num = this.VisionDistance;
			if (obj == this.Yandere.gameObject)
			{
				num += this.VisionBonus;
			}
			float num2 = num * num;
			if (vector2.sqrMagnitude <= num2)
			{
				Debug.DrawLine(vector, targetPoint, Color.green);
				RaycastHit raycastHit;
				if (Physics.Linecast(vector, targetPoint, out raycastHit, mask) && (raycastHit.collider.gameObject == obj || raycastHit.collider.gameObject.transform.root.gameObject == obj || raycastHit.transform.root.gameObject == obj.transform.root.gameObject))
				{
					foreach (int num3 in layers)
					{
						if (raycastHit.collider.gameObject.layer == num3)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x001CC778 File Offset: 0x001CA978
	public bool CanSeeObject(GameObject obj, Vector3 targetPoint)
	{
		if (!this.Blind)
		{
			Vector3 vector = new Vector3(base.transform.position.x, this.Eyes.transform.position.y, base.transform.position.z);
			Vector3 vector2 = targetPoint - vector;
			float num = this.VisionDistance;
			if (obj == this.Yandere.gameObject)
			{
				num += this.VisionBonus;
			}
			float num2 = num * num;
			bool flag = this.PointIsInFOV(targetPoint);
			bool flag2 = vector2.sqrMagnitude <= num2;
			RaycastHit raycastHit;
			if (flag && flag2 && Physics.Linecast(vector, targetPoint, out raycastHit, this.Mask) && raycastHit.collider.gameObject == obj)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x001CC843 File Offset: 0x001CAA43
	public bool CanSeeObject(GameObject obj)
	{
		return this.CanSeeObject(obj, obj.transform.position);
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x001CC858 File Offset: 0x001CAA58
	private void Update()
	{
		if (!this.Stop)
		{
			this.DistanceToPlayer = Vector3.Distance(base.transform.position, this.Yandere.transform.position);
			this.UpdateRoutine();
			this.UpdateDetectionMarker();
			if (this.Dying)
			{
				this.UpdateDying();
				if (this.Burning)
				{
					this.UpdateBurning();
				}
			}
			else
			{
				if (this.DistanceToPlayer <= 2f)
				{
					this.UpdateTalkInput();
				}
				this.UpdateVision();
				if (this.Pushed)
				{
					this.UpdatePushed();
				}
				else if (this.Drowned)
				{
					this.UpdateDrowned();
				}
				else if (this.WitnessedMurder)
				{
					this.UpdateWitnessedMurder();
				}
				else if (this.Alarmed)
				{
					this.UpdateAlarmed();
				}
				else if (this.TurnOffRadio)
				{
					this.UpdateTurningOffRadio();
				}
				else if (this.Confessing)
				{
					this.UpdateConfessing();
				}
				else if (this.Vomiting)
				{
					this.UpdateVomiting();
				}
				else if (this.Splashed)
				{
					this.UpdateSplashed();
				}
			}
			this.UpdateMisc();
			return;
		}
		this.UpdateStop();
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x001CC96C File Offset: 0x001CAB6C
	private void UpdateStop()
	{
		if (this.StudentManager.Pose)
		{
			this.DistanceToPlayer = Vector3.Distance(base.transform.position, this.Yandere.transform.position);
			if (this.Prompt.Circle[0].fillAmount == 0f)
			{
				this.Pose();
			}
		}
		else if (!this.ClubActivity)
		{
			if (!this.Yandere.Talking)
			{
				if (this.Yandere.Sprayed)
				{
					this.CharacterAnimation.CrossFade(this.ScaredAnim);
				}
				if (this.Yandere.Noticed || this.StudentManager.YandereDying)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					if (Vector3.Distance(base.transform.position, this.Yandere.transform.position) < 1f)
					{
						this.MyController.Move(base.transform.forward * (Time.deltaTime * -1f));
					}
					if (this.Yandere.Attacking && this.Yandere.TargetStudent != null)
					{
						if (this.Yandere.TargetStudent.Hips == null)
						{
							Debug.Log("What the hell? Apparently, Hips became null somehow! Investigate quick!");
						}
						else
						{
							this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.TargetStudent.Hips.transform.position.x, base.transform.position.y, this.Yandere.TargetStudent.Hips.transform.position.z) - base.transform.position);
							base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
							if (Vector3.Distance(base.transform.position, this.Yandere.TargetStudent.transform.position) < 2f)
							{
								this.MyController.Move(base.transform.forward * (Time.deltaTime * -1f));
							}
						}
					}
				}
			}
		}
		else if (this.Police.Darkness.color.a < 1f)
		{
			if (this.Club == ClubType.Cooking)
			{
				this.CharacterAnimation[this.SocialSitAnim].layer = 99;
				this.CharacterAnimation.Play(this.SocialSitAnim);
				this.CharacterAnimation[this.SocialSitAnim].weight = 1f;
				this.SmartPhone.SetActive(false);
				this.SpeechLines.Play();
				this.CharacterAnimation.CrossFade(this.RandomAnim);
				if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
				{
					this.PickRandomAnim();
				}
			}
			else if (this.Club == ClubType.MartialArts)
			{
				this.CharacterAnimation.enabled = true;
				this.CharacterAnimation.Play(this.ActivityAnim);
				AudioSource component = base.GetComponent<AudioSource>();
				if (!this.Male)
				{
					if (this.CharacterAnimation["f02_kick_23"].time > 1f)
					{
						if (!this.PlayingAudio)
						{
							component.clip = this.FemaleAttacks[UnityEngine.Random.Range(0, this.FemaleAttacks.Length)];
							component.Play();
							this.PlayingAudio = true;
						}
						if (this.CharacterAnimation["f02_kick_23"].time >= this.CharacterAnimation["f02_kick_23"].length)
						{
							this.CharacterAnimation["f02_kick_23"].time = 0f;
							this.PlayingAudio = false;
						}
					}
				}
				else if (this.CharacterAnimation["kick_24"].time > 1f)
				{
					if (!this.PlayingAudio)
					{
						component.clip = this.MaleAttacks[UnityEngine.Random.Range(0, this.MaleAttacks.Length)];
						component.Play();
						this.PlayingAudio = true;
					}
					if (this.CharacterAnimation["kick_24"].time >= this.CharacterAnimation["kick_24"].length)
					{
						this.CharacterAnimation["kick_24"].time = 0f;
						this.PlayingAudio = false;
					}
				}
			}
			else if (this.Club == ClubType.Drama)
			{
				this.Stop = false;
			}
			else if (this.Club == ClubType.Photography)
			{
				this.CharacterAnimation.CrossFade(this.RandomSleuthAnim);
				if (this.CharacterAnimation[this.RandomSleuthAnim].time >= this.CharacterAnimation[this.RandomSleuthAnim].length)
				{
					this.PickRandomSleuthAnim();
				}
			}
			else if (this.Club == ClubType.Art)
			{
				this.CharacterAnimation.Play(this.ActivityAnim);
				this.Paintbrush.SetActive(true);
				this.Palette.SetActive(true);
			}
			else if (this.Club == ClubType.Science)
			{
				this.CharacterAnimation.Play(this.ClubAnim);
				if (!this.StudentManager.Eighties)
				{
					if (this.StudentID == 62)
					{
						this.ScienceProps[0].SetActive(true);
					}
					else if (this.StudentID == 63)
					{
						this.ScienceProps[1].SetActive(true);
						this.ScienceProps[2].SetActive(true);
					}
					else if (this.StudentID == 64)
					{
						this.ScienceProps[0].SetActive(true);
					}
					else if (this.StudentID == 65)
					{
						Debug.Log("Yo?");
					}
				}
				else
				{
					if (!this.Male && !this.ScienceProps[1].activeInHierarchy)
					{
						Debug.Log("Supposedly straightening skirt.");
						this.CharacterAnimation.Play("f02_straightenSkirt_00");
					}
					this.ScienceProps[1].SetActive(true);
					this.ScienceProps[2].SetActive(true);
				}
			}
			else if (this.Club == ClubType.Sports)
			{
				this.Stop = false;
			}
			else if (this.Club == ClubType.Gardening)
			{
				this.CharacterAnimation.Play(this.ClubAnim);
				this.Stop = false;
			}
			else if (this.Club == ClubType.Gaming)
			{
				this.CharacterAnimation.CrossFade(this.ClubAnim);
			}
			else if (this.Club == ClubType.Newspaper)
			{
				this.CharacterAnimation.enabled = true;
				this.CharacterAnimation.CrossFade(this.ClubAnim);
			}
		}
		this.Alarm = Mathf.MoveTowards(this.Alarm, 0f, Time.deltaTime);
		this.UpdateDetectionMarker();
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x001CD0E0 File Offset: 0x001CB2E0
	private void UpdateRoutine()
	{
		if (this.Routine)
		{
			this.IgnoreFoodTimer -= Time.deltaTime;
			if (this.CurrentDestination != null)
			{
				this.DistanceToDestination = Vector3.Distance(base.transform.position, this.CurrentDestination.position);
			}
			if (this.Phase > this.ScheduleBlocks.Length - 1)
			{
				Debug.Log("For some reason, " + this.Name + "'s Phase was greater than their number of ScheduleBlocks.");
				this.Phase = this.ScheduleBlocks.Length - 1;
				if (this.Bullied || this.StudentID == this.StudentManager.VictimID)
				{
					Debug.Log("The student who reached this code is bullied.");
					ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase];
					scheduleBlock.destination = "Locker";
					scheduleBlock.action = "Shoes";
					this.GetDestinations();
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
				}
				if (this.Leaving)
				{
					Debug.Log("The student who reached this code is supposed to be leaving school.");
					ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[this.Phase];
					scheduleBlock2.destination = "Exit";
					scheduleBlock2.action = "Stand";
					this.GetDestinations();
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
				}
			}
			if (this.StalkerFleeing)
			{
				if (this.Actions[this.Phase] == StudentActionType.Stalk && this.DistanceToPlayer > 10f)
				{
					this.Pathfinding.target = this.Yandere.transform;
					this.CurrentDestination = this.Yandere.transform;
					this.StalkerFleeing = false;
					this.Pathfinding.speed = this.WalkSpeed;
				}
			}
			else if (this.Actions[this.Phase] == StudentActionType.Stalk)
			{
				this.TargetDistance = 10f;
				if (this.DistanceToPlayer > 20f)
				{
					this.Pathfinding.speed = 4f;
				}
				else if (this.DistanceToPlayer < 10f)
				{
					this.Pathfinding.speed = this.WalkSpeed;
				}
			}
			if (!this.Indoors)
			{
				if (this.Hurry && !this.Tripped && this.MustTrip && base.transform.position.z > -50.5f && base.transform.position.x < 6f)
				{
					this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
					this.CharacterAnimation.CrossFade("trip_00");
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.Tripping = true;
					this.Routine = false;
					this.Blind = true;
					if (this.Clock.GameplayDay == 3 && !this.BountyCollider.activeInHierarchy)
					{
						this.BountyCollider.transform.localPosition = new Vector3(0f, 0f, 0.25f);
						this.BountyCollider.GetComponent<BoxCollider>().center = new Vector3(0f, 0.12f, 0f);
						this.BountyCollider.GetComponent<BoxCollider>().size = new Vector3(0.78f, 0.24f, 1.9f);
						this.BountyCollider.SetActive(true);
					}
				}
				if (this.Paired)
				{
					if (base.transform.position.z < -50f)
					{
						if (base.transform.localEulerAngles != Vector3.zero)
						{
							base.transform.localEulerAngles = Vector3.zero;
						}
						this.MyController.Move(base.transform.forward * Time.deltaTime);
						if (this.StudentID == 81 && !this.StudentManager.Eighties)
						{
							this.MusumeTimer += Time.deltaTime;
							if (this.MusumeTimer > 5f)
							{
								this.MusumeTimer = 0f;
								this.MusumeRight = !this.MusumeRight;
								this.WalkAnim = (this.MusumeRight ? "f02_walkTalk_00" : "f02_walkTalk_01");
							}
						}
					}
					else
					{
						if (this.Persona == PersonaType.PhoneAddict)
						{
							this.SpeechLines.Stop();
							this.SmartPhone.SetActive(true);
						}
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.StopPairing();
					}
				}
				if (!this.StudentManager.KokonaTutorial)
				{
					if (this.Phase == 0)
					{
						if (this.DistanceToDestination < 1f)
						{
							this.CurrentDestination = this.MyLocker;
							this.Pathfinding.target = this.MyLocker;
							this.Phase++;
						}
					}
					else if (this.DistanceToDestination < 0.5f && !this.ShoeRemoval.enabled && !this.InEvent)
					{
						if (this.Shy)
						{
							this.CharacterAnimation[this.ShyAnim].weight = 0.5f;
						}
						this.SmartPhone.SetActive(false);
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.ShoeRemoval.StartChangingShoes();
						this.ShoeRemoval.enabled = true;
						this.ChangingShoes = true;
						this.CanTalk = false;
						this.Routine = false;
					}
				}
			}
			else if (this.Phase < this.ScheduleBlocks.Length - 1 && this.Clock.HourTime >= this.ScheduleBlocks[this.Phase].time && !this.InEvent && !this.Meeting && this.ClubActivityPhase < 16 && !this.Ragdoll.Zs.activeInHierarchy && !this.Dying && !this.Posing)
			{
				if (this.Actions[this.Phase] == StudentActionType.Clean && this.Pushable && !this.Meeting)
				{
					this.Pushable = false;
					this.StudentManager.UpdateMe(this.StudentID);
					this.ChalkDust.Stop();
				}
				this.SimpleForgetAboutBloodPool();
				this.MyRenderer.updateWhenOffscreen = false;
				this.SprintAnim = this.OriginalSprintAnim;
				if (this.Headache)
				{
					this.SprintAnim = this.OriginalSprintAnim;
					this.WalkAnim = this.OriginalWalkAnim;
				}
				if (this.Vomiting)
				{
					this.StopVomitting();
				}
				this.SitInInfirmary = false;
				this.Pushable = false;
				this.Headache = false;
				this.Sedated = false;
				this.Hurry = false;
				if (this.Schoolwear == 1)
				{
					this.SunbathePhase = 0;
				}
				this.Phase++;
				this.SciencePhase = 0;
				if (this.WateringCan != null)
				{
					this.WateringCan.transform.parent = this.Hips;
					this.WateringCan.transform.localPosition = new Vector3(0f, 0.0135f, -0.184f);
					this.WateringCan.transform.localEulerAngles = new Vector3(0f, 90f, 30f);
				}
				this.RivalBodyguard = false;
				if (this.ScheduleBlocks[this.Phase].destination == "Guard")
				{
					this.RivalBodyguard = true;
				}
				this.WitnessBonus = 0;
				this.BountyCollider.SetActive(false);
				if (this.Drownable)
				{
					this.Drownable = false;
					this.StudentManager.UpdateMe(this.StudentID);
				}
				if (this.Schoolwear > 1 && this.Destinations[this.Phase] == this.MyLocker)
				{
					this.Phase++;
					if (this.Rival && DateGlobals.Weekday == DayOfWeek.Friday)
					{
						this.Phase--;
					}
				}
				if ((this.Actions[this.Phase] == StudentActionType.SitAndTakeNotes && this.Schoolwear == 2) || (this.Actions[this.Phase] == StudentActionType.ChangeShoes && this.Schoolwear == 2) || (this.Actions[this.Phase] == StudentActionType.AtLocker && this.Schoolwear == 2) || (this.Actions[this.Phase] == StudentActionType.AtLocker && this.BikiniAttacher != null && this.BikiniAttacher.newRenderer != null))
				{
					if (this.Schoolwear == 2 && this.BeenSplashed)
					{
						Debug.Log(this.Name + " doesn't have access to a clean uniform. They will simply have to continue wearing their gym clothing.");
					}
					else
					{
						Debug.Log(this.Name + " needs to change clothing before doing whatever they're supposed to do next.");
						this.MustChangeClothing = true;
						this.ChangeClothingPhase = 0;
					}
				}
				if (this.Actions[this.Phase] == StudentActionType.Graffiti && !this.StudentManager.Bully)
				{
					ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[2];
					scheduleBlock3.destination = "Patrol";
					scheduleBlock3.action = "Patrol";
					this.GetDestinations();
				}
				if (!this.StudentManager.Eighties && this.Actions[this.Phase] == StudentActionType.Bully && this.StudentManager.VictimID == 10)
				{
					Debug.Log("The bullies are scared of Raibaru, and won't bully her.");
					ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[4];
					if (this.StudentManager.Week == 1 && this.Clock.Weekday == 4)
					{
						Debug.Log("The bullies are going to go sunbathe now.");
						scheduleBlock4.destination = "Sunbathe";
						scheduleBlock4.action = "Sunbathe";
					}
					else
					{
						Debug.Log("The bullies are going to patrol now.");
						scheduleBlock4.destination = "Patrol";
						scheduleBlock4.action = "Patrol";
					}
					this.GetDestinations();
				}
				if (!this.StudentManager.ReactedToGameLeader && this.Actions[this.Phase] == StudentActionType.Bully && !this.StudentManager.Bully)
				{
					ScheduleBlock scheduleBlock5 = this.ScheduleBlocks[4];
					scheduleBlock5.destination = "Sunbathe";
					scheduleBlock5.action = "Sunbathe";
					this.GetDestinations();
				}
				if (this.Sedated)
				{
					this.SprintAnim = this.OriginalSprintAnim;
					this.Sedated = false;
				}
				this.CurrentAction = this.Actions[this.Phase];
				this.CurrentDestination = this.Destinations[this.Phase];
				this.Pathfinding.target = this.Destinations[this.Phase];
				if (this.Rival && DateGlobals.Weekday == DayOfWeek.Friday && !this.InCouple)
				{
					if (this.Rival && DateGlobals.Weekday == DayOfWeek.Friday)
					{
						Debug.Log("This is the part where the rival decides whether or not she should put a note in someone's locker.");
						this.CharacterAnimation.CrossFade(this.WalkAnim);
					}
					if ((this.StudentManager.LoveManager.ConfessToSuitor || GameGlobals.RivalEliminationID != 4) && this.Actions[this.Phase] == StudentActionType.ChangeShoes)
					{
						Debug.Log("The rival's current action is ''ChangingShoes''.");
						this.ChooseLocker();
						this.Yandere.PauseScreen.Hint.Show = true;
						this.Yandere.PauseScreen.Hint.QuickID = 10;
						this.IgnoringThingsOnGround = true;
						this.IgnoringPettyActions = true;
						this.Confessing = true;
						this.Routine = false;
						this.CanTalk = false;
					}
				}
				if (this.CurrentDestination != null)
				{
					this.DistanceToDestination = Vector3.Distance(base.transform.position, this.CurrentDestination.position);
				}
				if (this.Bento != null && this.Bento.activeInHierarchy)
				{
					this.Bento.SetActive(false);
					this.Chopsticks[0].SetActive(false);
					this.Chopsticks[1].SetActive(false);
				}
				if (this.Male)
				{
					this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].SetFloat("_BlendAmount", 0f);
					this.PinkSeifuku.SetActive(false);
				}
				if (!this.Paired)
				{
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
				}
				if (this.Persona != PersonaType.PhoneAddict && !this.Sleuthing)
				{
					this.SmartPhone.SetActive(false);
				}
				else if (!this.Slave && !this.Phoneless)
				{
					if (!this.StudentManager.Eighties)
					{
						if ((this.StudentID > 37 && this.StudentID < 41) || (this.StudentID == 84 && this.Club == ClubType.None))
						{
							this.IdleAnim = "f02_friendlyPhoneIdle_00";
						}
						else
						{
							this.IdleAnim = this.PhoneAnims[0];
						}
					}
					this.SmartPhone.SetActive(true);
				}
				this.BountyCollider.SetActive(false);
				this.OccultBook.SetActive(false);
				this.Paintbrush.SetActive(false);
				this.Sketchbook.SetActive(false);
				this.Cigarette.SetActive(false);
				this.Scrubber.SetActive(false);
				this.Drawing.SetActive(false);
				this.Lighter.SetActive(false);
				this.Palette.SetActive(false);
				this.Pencil.SetActive(false);
				this.Eraser.SetActive(false);
				this.Pen.SetActive(false);
				foreach (GameObject gameObject in this.ScienceProps)
				{
					if (gameObject != null)
					{
						gameObject.SetActive(false);
					}
				}
				foreach (GameObject gameObject2 in this.Fingerfood)
				{
					if (gameObject2 != null)
					{
						gameObject2.SetActive(false);
					}
				}
				this.SpeechLines.Stop();
				this.GoAway = false;
				this.ReadPhase = 0;
				if (!this.ReturningFromSave)
				{
					this.PatrolID = 0;
				}
				if (this.Phase == this.PhaseFromSave)
				{
					if (this.StudentManager.Patrols.List[this.StudentID] != null && this.Destinations[this.Phase] == this.StudentManager.Patrols.List[this.StudentID].GetChild(0))
					{
						this.Destinations[this.Phase] = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
						this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
						this.Pathfinding.target = this.CurrentDestination;
					}
					this.ReturningFromSave = false;
				}
				if (this.Actions[this.Phase] == StudentActionType.Clean)
				{
					this.TargetDistance = 0.61f;
					if (!this.StudentManager.CustomMode && this.StudentID == 11)
					{
						this.FollowTargetDestination.localPosition = new Vector3(0f, 0f, -1f);
					}
					this.EquipCleaningItems();
				}
				else
				{
					if (!this.StudentManager.Eighties && this.StudentID == 11)
					{
						this.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 0f);
					}
					if (!this.Slave && !this.Phoneless)
					{
						if (this.Persona == PersonaType.PhoneAddict)
						{
							this.SmartPhone.transform.localPosition = new Vector3(0.01f, 0.005f, 0.01f);
							this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, 165f);
							if (!this.StudentManager.Eighties)
							{
								this.WalkAnim = this.PhoneAnims[1];
							}
						}
						else if (this.Sleuthing)
						{
							this.WalkAnim = this.SleuthWalkAnim;
						}
					}
				}
				if (this.Actions[this.Phase] == StudentActionType.Sleuth && this.StudentManager.SleuthPhase == 3)
				{
					this.GetSleuthTarget();
				}
				if (this.Actions[this.Phase] == StudentActionType.Stalk)
				{
					this.TargetDistance = 10f;
				}
				else if (this.Actions[this.Phase] == StudentActionType.Follow)
				{
					this.TargetDistance = 0.5f;
					if (this.FollowTarget != null && !this.FollowTarget.Alive && !this.WitnessedCorpse)
					{
						if (this.FollowTarget.CurrentAction == StudentActionType.Clean)
						{
							this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(-1f, 0f, -1f);
						}
						else if (this.FollowTarget.CurrentAction == StudentActionType.SitAndEatBento)
						{
							this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 1f);
						}
						else if (this.FollowTarget.Meeting)
						{
							this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(-1f, 0f, 0f);
						}
						else
						{
							this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 0f);
						}
						this.TargetDistance = 1f;
					}
				}
				else if (this.Actions[this.Phase] != StudentActionType.SitAndEatBento)
				{
					this.TargetDistance = 0.5f;
				}
				if (this.Club == ClubType.Gardening && this.StudentID != 71 && this.Actions[this.Phase] == StudentActionType.ClubAction)
				{
					if (!this.StudentManager.Eighties || this.WaterLow)
					{
						if (this.WateringCan != null && this.WateringCan.activeInHierarchy && this.WateringCan.transform.parent != this.Hips)
						{
							this.WateringCan.transform.parent = this.Hips;
							this.WateringCan.transform.localPosition = new Vector3(0f, 0.0135f, -0.184f);
							this.WateringCan.transform.localEulerAngles = new Vector3(0f, 90f, 30f);
						}
						if (this.Clock.Period == 6 && !this.WaterLow && this.StudentManager.Patrols.List[this.StudentID] != this.StudentManager.GardeningPatrols[this.StudentID - 71])
						{
							this.StudentManager.Patrols.List[this.StudentID] = this.StudentManager.GardeningPatrols[this.StudentID - 71];
							this.ClubAnim = "f02_waterPlantLow_00";
							this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
							this.Pathfinding.target = this.CurrentDestination;
						}
					}
					else if (this.Clock.Period == 6 && this.StudentManager.Patrols.List[this.StudentID] != this.StudentManager.GardeningPatrols[this.StudentID - 71])
					{
						this.StudentManager.Patrols.List[this.StudentID] = this.StudentManager.GardeningPatrols[this.StudentID - 71];
						this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
						this.Pathfinding.target = this.CurrentDestination;
					}
				}
				if (this.OriginalClub == ClubType.LightMusic)
				{
					if (this.StudentID == 51)
					{
						if (this.InstrumentBag[this.ClubMemberID].transform.parent == null)
						{
							this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().playOnAwake = false;
							this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Stop();
							this.Instruments[this.ClubMemberID].transform.parent = null;
							if (!this.StudentManager.Eighties)
							{
								this.Instruments[this.ClubMemberID].transform.position = new Vector3(-0.5f, 4.5f, 22.45666f);
								this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, 0f, 0f);
							}
							else
							{
								this.Instruments[this.ClubMemberID].transform.position = new Vector3(2.105f, 4.5f, 25.5f);
								this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
							}
						}
						else
						{
							this.Instruments[this.ClubMemberID].SetActive(false);
						}
					}
					else
					{
						this.Instruments[this.ClubMemberID].SetActive(false);
					}
					this.Drumsticks[0].SetActive(false);
					this.Drumsticks[1].SetActive(false);
					this.AirGuitar.Stop();
				}
				if (!this.StudentManager.Eighties && this.Phase == 8 && this.StudentID == 36)
				{
					this.StudentManager.Clubs.List[this.StudentID].position = this.StudentManager.Clubs.List[71].position;
					this.StudentManager.Clubs.List[this.StudentID].rotation = this.StudentManager.Clubs.List[71].rotation;
					this.ClubAnim = this.GameAnim;
				}
				if (this.MyPlate != null && this.MyPlate.parent == this.RightHand)
				{
					this.CurrentDestination = this.StudentManager.Clubs.List[this.StudentID];
					this.Pathfinding.target = this.StudentManager.Clubs.List[this.StudentID];
				}
				if (this.Persona == PersonaType.Sleuth)
				{
					if (this.Male)
					{
						this.SmartPhone.transform.localPosition = new Vector3(0.06f, -0.02f, -0.02f);
						if (!this.StudentManager.Eighties)
						{
							this.SmartPhone.transform.localEulerAngles = new Vector3(12.5f, 120f, 180f);
						}
						else
						{
							this.SmartPhone.transform.localEulerAngles = new Vector3(22.5f, 22.5f, 150f);
						}
					}
					else if (this.StudentManager.Eighties)
					{
						this.SmartPhone.transform.localPosition = new Vector3(0.033333f, -0.066666f, -0.01f);
						this.SmartPhone.transform.localEulerAngles = new Vector3(15f, 15f, 105f);
					}
					else if (this.Sleuthing)
					{
						this.SmartPhone.transform.localPosition = new Vector3(0.033333f, -0.015f, -0.015f);
						this.SmartPhone.transform.localEulerAngles = new Vector3(12.5f, 120f, 180f);
					}
					else
					{
						this.SmartPhone.transform.localPosition = new Vector3(0.025f, 0.02f, 0.04f);
						this.SmartPhone.transform.localEulerAngles = new Vector3(22.5f, -157.5f, 180f);
					}
				}
				if (this.Character.transform.localPosition.y == -0.25f)
				{
					Debug.Log("Swimming club special case was reached.");
					this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.ID].GetChild(this.ClubActivityPhase - 2);
					this.CurrentDestination = this.StudentManager.Clubs.List[this.ID].GetChild(this.ClubActivityPhase - 2);
					this.Pathfinding.target = this.StudentManager.Clubs.List[this.ID].GetChild(this.ClubActivityPhase - 2);
				}
				if (this.Actions[this.Phase] == StudentActionType.Sunbathe && this.SunbathePhase > 1)
				{
					this.CurrentDestination = this.StudentManager.SunbatheSpots[this.StudentID];
					this.Pathfinding.target = this.StudentManager.SunbatheSpots[this.StudentID];
				}
				if (this.StudentID == 10 && this.FollowTarget != null && !this.FollowTarget.Alive && this.StudentManager.LastKnownOsana.position != Vector3.zero)
				{
					this.Pathfinding.target = this.StudentManager.LastKnownOsana;
					this.CurrentDestination = this.StudentManager.LastKnownOsana;
				}
				if (this.Phoneless)
				{
					this.SmartPhone.SetActive(false);
				}
				if (this.Rival)
				{
					if (this.CurrentAction == StudentActionType.Clean)
					{
						this.StudentManager.RivalBookBag.gameObject.SetActive(false);
						this.StudentManager.RivalBookBag.Prompt.Hide();
						this.StudentManager.RivalBookBag.Prompt.enabled = false;
						if (this.StudentManager.Eighties)
						{
							this.BookBag.SetActive(true);
						}
					}
					else if (this.Clock.Period == 4 && this.CurrentAction == StudentActionType.Read && !this.StudentManager.RivalBookBag.BorrowedBook)
					{
						ScheduleBlock scheduleBlock6 = this.ScheduleBlocks[this.Phase];
						scheduleBlock6.destination = "Search Patrol";
						scheduleBlock6.action = "Search Patrol";
						this.GetDestinations();
						this.CurrentDestination = this.Destinations[this.Phase];
						this.Pathfinding.target = this.Destinations[this.Phase];
					}
				}
				if (!this.Teacher && this.Club != ClubType.Delinquent && this.Club != ClubType.Sports)
				{
					if (this.Clock.Period == 2 || this.Clock.Period == 4)
					{
						if (this.ClubActivityPhase < 16)
						{
							this.Pathfinding.speed = 4f;
						}
					}
					else
					{
						this.Pathfinding.speed = this.WalkSpeed;
					}
					if (!this.StudentManager.Eighties)
					{
						if (this.StudentManager.ConvoManager.Week != 0)
						{
						}
					}
					else if (this.StudentID > 10 && this.StudentID < 21 && (this.Schoolwear != 1 || (this.BikiniAttacher != null && this.BikiniAttacher.newRenderer != null)))
					{
						Debug.Log(this.Name + " is advancing her phase now.");
						Debug.Log(this.Name + " wasn't dressed normally when she reached this part of the code.");
						if (this.Schoolwear == 3)
						{
							Debug.Log(this.Name + " was in gym clothing. Should be running.");
							this.Hurry = true;
							this.Pathfinding.speed = 4f;
						}
						if (this.CurrentAction == StudentActionType.AtLocker || this.CurrentAction == StudentActionType.ChangeShoes || this.CurrentAction == StudentActionType.SitAndTakeNotes)
						{
							Debug.Log(this.Name + "'s next destination is should be their locker.");
							if (this.SchoolwearUnavailable)
							{
								Debug.Log(this.Name + " can't change clothing! Her schoolwear is unavailable!");
							}
							else
							{
								this.ConfessAfterwards = this.Confessing;
								if (this.ConfessAfterwards)
								{
									Debug.Log(this.Name + " needs to confess love to someone.");
									this.Routine = true;
								}
								this.Confessing = false;
								this.ChangeClothingPhase = 0;
								Debug.Log(this.Name + " is going to change clothing first. Now firing the GoChange() function.");
								this.GoChange();
							}
						}
					}
				}
				if (this.Infatuated && this.Actions[this.Phase] == StudentActionType.Admire)
				{
					if (this.DistanceToDestination > this.TargetDistance + 5f)
					{
						this.Pathfinding.speed = 5f;
					}
					else
					{
						this.Pathfinding.speed = this.WalkSpeed;
					}
					if (this.InvestigatingMysteriousDisappearance)
					{
						this.TargetDistance = 1f;
					}
					else if (base.transform.position.y > this.CurrentDestination.position.y + 1f || base.transform.position.y < this.CurrentDestination.position.y - 1f)
					{
						this.TargetDistance = 2f;
					}
					else if (this.StudentManager.Students[this.StudentManager.RivalID] != null && this.StudentManager.Students[this.StudentManager.RivalID].Meeting)
					{
						this.TargetDistance = 10f;
					}
					else if (this.StudentManager.Week > 8)
					{
						this.TargetDistance = 0.5f;
					}
					else
					{
						this.TargetDistance = 3.9f;
					}
				}
				if (this.Club == ClubType.Sports && this.Clock.Period == 6 && !this.StudentManager.PoolClosed && this.Schoolwear == 3)
				{
					this.ClubAnim = this.GenderPrefix + "poolDive_00";
					this.ClubActivityPhase = 15;
					this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
				}
				if (this.CurrentAction == StudentActionType.GravurePose && this.WearingBikini)
				{
					Debug.Log("She's already in her bikini. Telling her to go pose!");
					this.GoPose();
				}
				if (this.WitnessID > 0)
				{
					this.Hurry = true;
				}
				if (this.ClubLeader && this.Club == ClubType.Art && this.CurrentAction == StudentActionType.Paint && DateGlobals.Weekday == DayOfWeek.Friday)
				{
					this.StudentManager.Clubs.List[41].position = this.CurrentDestination.position;
				}
				this.CharacterAnimation[this.SitAnim].weight = 0f;
				this.CharacterAnimation[this.SocialSitAnim].weight = 0f;
				if (this.MustChangeClothing && this.Schoolwear == 2)
				{
					Debug.Log(this.Name + " really ought to change their clothing before they proceed with their routine.");
					this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
					this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
				}
				if (this.Bullied && this.Clock.HourTime > 16f)
				{
					Debug.Log("A bullied student should be heading for his locker now.");
					ScheduleBlock scheduleBlock7 = this.ScheduleBlocks[this.Phase];
					scheduleBlock7.destination = "Locker";
					scheduleBlock7.action = "Shoes";
					this.GetDestinations();
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
				}
			}
			if (this.MeetTime > 0f)
			{
				bool flag = false;
				if (this.MyPlate != null && this.MyPlate.parent == this.RightHand)
				{
					flag = true;
				}
				if (!this.InEvent && this.Clock.HourTime > this.MeetTime && !flag && this.Schoolwear != 0 && this.Schoolwear != 2)
				{
					if (this.Follower != null)
					{
						ScheduleBlock scheduleBlock8 = this.Follower.ScheduleBlocks[this.Follower.Phase];
						scheduleBlock8.destination = "Follow";
						scheduleBlock8.action = "Follow";
						this.Follower.Actions[this.Follower.Phase] = StudentActionType.Follow;
						this.CurrentAction = StudentActionType.Follow;
						this.Follower.GetDestinations();
						this.Follower.CurrentDestination = this.Follower.Destinations[this.Follower.Phase];
						this.Follower.Pathfinding.target = this.Follower.Destinations[this.Follower.Phase];
					}
					this.CurrentDestination = this.MeetSpot;
					this.Pathfinding.target = this.MeetSpot;
					this.DistanceToDestination = Vector3.Distance(base.transform.position, this.CurrentDestination.position);
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.Pathfinding.speed = 4f;
					this.TargetDistance = 1f;
					this.SpeechLines.Stop();
					this.EmptyHands();
					this.Meeting = true;
					this.MeetTime = 0f;
					if (this.Rival)
					{
						this.StudentManager.UpdateInfatuatedTargetDistances();
					}
				}
			}
			if (this.DistanceToDestination > this.TargetDistance)
			{
				this.ReadPhase = 0;
				if (this.Actions[this.Phase] == StudentActionType.Sleuth)
				{
					if (!this.SmartPhone.activeInHierarchy)
					{
						this.SmartPhone.SetActive(true);
						this.SpeechLines.Stop();
					}
					if (this.CurrentDestination == null)
					{
						this.GetSleuthTarget();
					}
					else
					{
						this.LockerRoomCheckTimer += Time.deltaTime;
						if (this.LockerRoomCheckTimer > 5f)
						{
							this.LockerRoomCheckTimer = 0f;
							if (this.StudentManager.LockerRoomArea.bounds.Contains(this.CurrentDestination.position) || this.StudentManager.MaleLockerRoomArea.bounds.Contains(this.CurrentDestination.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.CurrentDestination.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.CurrentDestination.position) || this.CurrentDestination.position.z < -100f)
							{
								this.GetSleuthTarget();
							}
						}
					}
				}
				else if (this.Actions[this.Phase] == StudentActionType.Relax && this.Curious)
				{
					if (this.CurrentDestination == this.StudentManager.Students[this.Crush].transform)
					{
						if (!this.StudentManager.Students[this.Crush].Drowned)
						{
							this.TargetDistance = 3.9f;
						}
						else
						{
							this.TargetDistance = 4.1f;
						}
					}
				}
				else if (this.Actions[this.Phase] == StudentActionType.Stalk && this.StudentManager.LockerRoomArea.bounds.Contains(this.Yandere.transform.position))
				{
					if (Vector3.Distance(base.transform.position, this.StudentManager.FleeSpots[0].position) > 10f)
					{
						this.Pathfinding.target = this.StudentManager.FleeSpots[0];
						this.CurrentDestination = this.StudentManager.FleeSpots[0];
					}
					else
					{
						this.Pathfinding.target = this.StudentManager.FleeSpots[1];
						this.CurrentDestination = this.StudentManager.FleeSpots[1];
					}
					this.Pathfinding.speed = 4f;
					this.StalkerFleeing = true;
				}
				if (this.StudentID == 10)
				{
					if (this.Actions[this.Phase] == StudentActionType.Follow && !this.Alarmed)
					{
						this.Obstacle.enabled = false;
						if (this.FollowTarget != null)
						{
							if (this.FollowTarget.Wet && this.FollowTarget.DistanceToDestination < 5f)
							{
								this.TargetDistance = 4f;
							}
							else if (this.FollowTarget.CurrentAction == StudentActionType.SearchPatrol)
							{
								this.TargetDistance = 1f;
							}
							else
							{
								this.TargetDistance = 0.5f;
								if (this.FollowTarget != null && !this.FollowTarget.Alive && !this.WitnessedCorpse)
								{
									this.TargetDistance = 1f;
								}
							}
						}
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						if (this.DistanceToDestination > 2f)
						{
							this.Pathfinding.speed = 5f;
							this.SpeechLines.Stop();
						}
						else
						{
							this.Pathfinding.speed = this.WalkSpeed;
							this.SpeechLines.Stop();
						}
					}
					else if (base.transform.position.z > 121f && this.Actions[this.Phase] == StudentActionType.Sketch && this.Yandere.Attacking && this.Yandere.TargetStudent == this.FollowTarget)
					{
						this.AwareOfMurder = true;
						this.Alarm = 200f;
					}
				}
				if (this.StudentID == 12 && this.Actions[this.Phase] == StudentActionType.LightFire && Vector3.Distance(this.Yandere.transform.position, this.CurrentDestination.position) < 5f && this.CanSeeObject(this.Yandere.gameObject, this.Yandere.HeadPosition))
				{
					this.Subtitle.CustomText = "...oh...I didn't realize someone was here...I'll just...be going, now...";
					this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					this.FinishPyro();
				}
				if (this.CuriosityPhase == 1 && this.Actions[this.Phase] == StudentActionType.Relax && this.StudentManager.Students[this.Crush].Private)
				{
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.CurrentDestination = this.Destinations[this.Phase];
					this.TargetDistance = 0.5f;
					this.CuriosityTimer = 0f;
					this.CuriosityPhase--;
				}
				if (this.Actions[this.Phase] != StudentActionType.Follow || (this.Actions[this.Phase] == StudentActionType.Follow && this.DistanceToDestination > this.TargetDistance + 0.1f))
				{
					if (this.Actions[this.Phase] == StudentActionType.Follow && ((this.Clock.Period == 1 && this.Clock.HourTime > 8.483334f) || (this.Clock.Period == 3 && this.Clock.HourTime > 13.483334f)))
					{
						this.Pathfinding.speed = 4f;
					}
					if (!this.InEvent && !this.Meeting && !this.GoAway)
					{
						if (this.DressCode)
						{
							if (this.Actions[this.Phase] == StudentActionType.ClubAction)
							{
								if (!this.ClubAttire)
								{
									if (!this.ChangingBooth.Occupied)
									{
										this.CurrentDestination = this.ChangingBooth.transform;
										this.Pathfinding.target = this.ChangingBooth.transform;
									}
									else
									{
										this.CurrentDestination = this.ChangingBooth.WaitSpots[this.ClubMemberID];
										this.Pathfinding.target = this.ChangingBooth.WaitSpots[this.ClubMemberID];
									}
								}
								else if (this.Indoors && !this.GoAway)
								{
									this.CurrentDestination = this.Destinations[this.Phase];
									this.Pathfinding.target = this.Destinations[this.Phase];
									this.DistanceToDestination = 100f;
								}
							}
							else if (this.ClubAttire)
							{
								this.TargetDistance = 1f;
								if (!this.ChangingBooth.Occupied)
								{
									this.CurrentDestination = this.ChangingBooth.transform;
									this.Pathfinding.target = this.ChangingBooth.transform;
								}
								else
								{
									this.CurrentDestination = this.ChangingBooth.WaitSpots[this.ClubMemberID];
									this.Pathfinding.target = this.ChangingBooth.WaitSpots[this.ClubMemberID];
								}
							}
							else if (this.Indoors && this.Actions[this.Phase] != StudentActionType.Clean && this.Actions[this.Phase] != StudentActionType.Sketch && this.Actions[this.Phase] != StudentActionType.Relax)
							{
								this.CurrentDestination = this.Destinations[this.Phase];
								this.Pathfinding.target = this.Destinations[this.Phase];
							}
						}
						else if (this.Actions[this.Phase] == StudentActionType.SitAndTakeNotes && this.Schoolwear > 1 && !this.SchoolwearUnavailable)
						{
							this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
							this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
						}
					}
					if (!this.Pathfinding.canMove)
					{
						if (!this.Spawned)
						{
							base.transform.position = this.StudentManager.SpawnPositions[this.StudentID].position;
							this.Spawned = true;
							if (!this.StudentManager.Eighties && this.StudentID == 10 && this.StudentManager.Students[11] == null)
							{
								base.transform.position = new Vector3(-4f, 0f, -96f);
								Physics.SyncTransforms();
							}
						}
						if (!this.Paired && !this.Alarmed)
						{
							this.Pathfinding.canSearch = true;
							this.Pathfinding.canMove = true;
							this.Obstacle.enabled = false;
						}
					}
					if (!this.InEvent)
					{
						if (this.Pathfinding.speed > 0f)
						{
							if (!this.Hurry && this.Pathfinding.speed == this.WalkSpeed)
							{
								if (!this.CharacterAnimation.IsPlaying(this.WalkAnim))
								{
									if (this.Persona == PersonaType.PhoneAddict && this.Actions[this.Phase] == StudentActionType.Clean)
									{
										this.CharacterAnimation.CrossFade(this.OriginalWalkAnim);
									}
									else
									{
										if (this.WalkAnim == "")
										{
											this.WalkAnim = this.OriginalWalkAnim;
										}
										this.CharacterAnimation.CrossFade(this.WalkAnim);
									}
								}
							}
							else if (!this.Dying)
							{
								if (this.Sleepy)
								{
									this.Pathfinding.speed = 2f;
								}
								else
								{
									this.Pathfinding.speed = 4f;
								}
								this.CharacterAnimation.CrossFade(this.SprintAnim);
							}
						}
					}
					else if (this.Mentoring && this.StudentID == 10 && this.InEvent && this.CurrentAction == StudentActionType.Socializing)
					{
						if (!this.Hurry)
						{
							this.Pathfinding.speed = this.WalkSpeed;
						}
						else
						{
							this.Pathfinding.speed = 4f;
						}
						if (this.Pathfinding.speed == this.WalkSpeed)
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.SprintAnim);
						}
						this.CheckForEndRaibaruEvent();
					}
					if (this.Club == ClubType.Occult && this.Actions[this.Phase] != StudentActionType.ClubAction)
					{
						this.OccultBook.SetActive(false);
					}
					if (!this.Meeting && !this.GoAway && !this.InEvent)
					{
						if (this.Actions[this.Phase] == StudentActionType.Clean)
						{
							if (this.SmartPhone.activeInHierarchy)
							{
								this.SmartPhone.SetActive(false);
							}
							if (this.CurrentDestination != this.CleaningSpot.GetChild(this.CleanID))
							{
								this.CurrentDestination = this.CleaningSpot.GetChild(this.CleanID);
								this.Pathfinding.target = this.CurrentDestination;
							}
						}
						if ((this.Actions[this.Phase] == StudentActionType.Patrol || (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Gardening)) && this.CurrentDestination != this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID))
						{
							this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
							this.Pathfinding.target = this.CurrentDestination;
						}
					}
				}
				if (this.Meeting)
				{
					if (this.BakeSale)
					{
						this.CharacterAnimation.CrossFade(this.WalkAnim);
						this.Pathfinding.speed = this.WalkSpeed;
					}
					else
					{
						this.CharacterAnimation.CrossFade(this.SprintAnim);
						this.Pathfinding.speed = 4f;
					}
				}
				if (this.CuriosityPhase == 1 && this.CurrentAction == StudentActionType.Relax && (this.StudentManager.LockerRoomArea.bounds.Contains(this.StudentManager.Students[this.Crush].transform.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.StudentManager.Students[this.Crush].transform.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.StudentManager.Students[this.Crush].transform.position)))
				{
					Debug.Log("This code is called when a student is stalking their crush and their crush is in the locker room or shower room.");
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.CurrentDestination = this.Destinations[this.Phase];
					this.TargetDistance = 0.5f;
					this.CuriosityTimer = 0f;
					this.CuriosityPhase--;
				}
				if (this.Infatuated && this.Actions[this.Phase] == StudentActionType.Admire)
				{
					if (this.DistanceToDestination > this.TargetDistance + 5f)
					{
						this.Pathfinding.speed = 5f;
					}
					else
					{
						this.Pathfinding.speed = this.WalkSpeed;
					}
					if (this.InvestigatingMysteriousDisappearance)
					{
						this.TargetDistance = 1f;
					}
					else if (base.transform.position.y > this.CurrentDestination.position.y + 0.1f || base.transform.position.y < this.CurrentDestination.position.y - 0.1f)
					{
						this.TargetDistance = 1f;
					}
					else if (this.StudentManager.Students[this.StudentManager.RivalID].Meeting || this.StudentManager.Students[this.StudentManager.RivalID].InEvent)
					{
						this.TargetDistance = 10f;
					}
					else if (this.StudentManager.Week > 8)
					{
						this.TargetDistance = 0.5f;
					}
					else
					{
						this.TargetDistance = 3.9f;
					}
					if (this.StudentManager.Students[this.StudentManager.RivalID].Talking || this.StudentManager.LockerRoomArea.bounds.Contains(this.CurrentDestination.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.CurrentDestination.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.CurrentDestination.position) || this.StudentManager.LockerRoomArea.bounds.Contains(this.StudentManager.Students[this.StudentManager.RivalID].transform.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.StudentManager.Students[this.StudentManager.RivalID].transform.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.StudentManager.Students[this.StudentManager.RivalID].transform.position))
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("impatientWait_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.WaitAnim);
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
					}
					else
					{
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
					}
					if (this.StudentID < 6)
					{
						this.ScootAway();
					}
				}
				if (this.RivalBodyguard)
				{
					if (this.StudentManager.LockerRoomArea.bounds.Contains(this.StudentManager.Students[this.StudentManager.RivalID].transform.position))
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("impatientWait_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.WaitAnim);
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
					}
					else
					{
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
					}
				}
				if (this.Actions[this.Phase] == StudentActionType.Clean)
				{
					this.TargetDistance = 0.61f;
				}
			}
			else
			{
				if (this.StudentID == 10 && this.InEvent && this.CurrentAction != StudentActionType.Follow && this.Mentoring)
				{
					this.SpeechLines.Play();
					this.CharacterAnimation.CrossFade(this.RandomAnim);
					if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
					{
						this.PickRandomAnim();
					}
					if (this.Mentoring)
					{
						this.CheckForEndRaibaruEvent();
					}
				}
				if (this.CurrentDestination != null)
				{
					bool flag2 = false;
					if ((this.Actions[this.Phase] == StudentActionType.Sleuth && this.StudentManager.SleuthPhase == 3 && !this.Meeting) || (this.Actions[this.Phase] == StudentActionType.Stalk || (this.Actions[this.Phase] == StudentActionType.Relax && this.CuriosityPhase == 1)) || (this.Actions[this.Phase] == StudentActionType.Guard && !this.Meeting) || (this.CurrentAction == StudentActionType.Follow && this.FollowTarget != null && this.FollowTarget.CurrentAction == StudentActionType.SearchPatrol))
					{
						if (this.StudentID == 10)
						{
							Debug.Log("For some reason, Raibaru believes that she should ''StopEarly''.");
						}
						if (this.StudentID == 11)
						{
							Debug.Log("Rival #1 believes that she should ''StopEarly''.");
						}
						this.TargetDistance = 2f;
						flag2 = true;
					}
					else if (this.Infatuated && this.Actions[this.Phase] == StudentActionType.Admire)
					{
						if (this.DistanceToDestination > this.TargetDistance + 5f)
						{
							this.Pathfinding.speed = 5f;
						}
						else
						{
							this.Pathfinding.speed = this.WalkSpeed;
						}
						if (this.InvestigatingMysteriousDisappearance)
						{
							this.TargetDistance = 1f;
						}
						else if (base.transform.position.y > this.CurrentDestination.position.y + 0.1f || base.transform.position.y < this.CurrentDestination.position.y - 0.1f)
						{
							this.TargetDistance = 1f;
						}
						else if (!this.StudentManager.Students[this.StudentManager.RivalID].Meeting && !this.StudentManager.Students[this.StudentManager.RivalID].InEvent)
						{
							if (this.StudentManager.Week > 8)
							{
								this.TargetDistance = 0.5f;
							}
							else
							{
								this.TargetDistance = 3.9f;
							}
						}
						flag2 = true;
					}
					if (this.Actions[this.Phase] == StudentActionType.Follow)
					{
						if (this.FollowTarget != null)
						{
							if (!this.ManualRotation)
							{
								this.targetRotation = Quaternion.LookRotation(this.FollowTarget.transform.position - base.transform.position);
								base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
							}
							if (this.FollowTarget.Attacked && this.FollowTarget.Alive && !this.FollowTarget.Tranquil && !this.Blind)
							{
								Debug.Log("Raibaru should be aware that Osana is being attacked.");
								this.AwareOfMurder = true;
								this.Alarm = 200f;
							}
						}
					}
					else if (!flag2)
					{
						this.MoveTowardsTarget(this.CurrentDestination.position);
						if (Quaternion.Angle(base.transform.rotation, this.CurrentDestination.rotation) > 1f && !this.StopRotating)
						{
							base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, 10f * Time.deltaTime);
						}
					}
					else
					{
						if (this.Infatuated)
						{
							StudentScript studentScript = null;
							if (this.Stalker)
							{
								this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
							}
							else
							{
								studentScript = this.StudentManager.Students[this.InfatuationID];
								this.targetRotation = Quaternion.LookRotation(new Vector3(this.StudentManager.Students[this.InfatuationID].transform.position.x, base.transform.position.y, this.StudentManager.Students[this.InfatuationID].transform.position.z) - base.transform.position);
							}
							if (studentScript != null && (!studentScript.gameObject.activeInHierarchy || !studentScript.enabled) && !this.StudentManager.TranqArea.bounds.Contains(studentScript.transform.position))
							{
								this.InvestigatingMysteriousDisappearance = true;
								this.TargetDistance = 1f;
								this.CannotFindInfatuationTarget();
							}
						}
						else if (this.Actions[this.Phase] == StudentActionType.Sleuth || this.Actions[this.Phase] == StudentActionType.Stalk)
						{
							if (this.Stalker)
							{
								this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
							}
							else
							{
								this.targetRotation = Quaternion.LookRotation(new Vector3(this.SleuthTarget.transform.position.x, base.transform.position.y, this.SleuthTarget.transform.position.z) - base.transform.position);
							}
						}
						else if (this.Actions[this.Phase] == StudentActionType.Guard)
						{
							this.targetRotation = Quaternion.LookRotation(base.transform.position - new Vector3(this.CurrentDestination.position.x, base.transform.position.y, this.CurrentDestination.position.z));
						}
						else if (this.Crush > 0)
						{
							this.targetRotation = Quaternion.LookRotation(new Vector3(this.StudentManager.Students[this.Crush].transform.position.x, base.transform.position.y, this.StudentManager.Students[this.Crush].transform.position.z) - base.transform.position);
						}
						float num = Quaternion.Angle(base.transform.rotation, this.targetRotation);
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
						if (num > 1f)
						{
							base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
						}
					}
					if (!this.Hurry)
					{
						this.Pathfinding.speed = this.WalkSpeed;
					}
					else
					{
						this.Pathfinding.speed = 4f;
					}
				}
				if (this.Pathfinding.canMove)
				{
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					if (this.Actions[this.Phase] != StudentActionType.Clean && !this.Infatuated)
					{
						this.Obstacle.enabled = true;
					}
				}
				if (!this.InEvent && !this.Meeting && this.DressCode)
				{
					if (this.Actions[this.Phase] == StudentActionType.ClubAction)
					{
						if (!this.ClubAttire)
						{
							if (!this.ChangingBooth.Occupied)
							{
								if (this.CurrentDestination == this.ChangingBooth.transform)
								{
									this.ChangingBooth.Occupied = true;
									this.ChangingBooth.Student = this;
									this.ChangingBooth.CheckYandereClub();
									this.Obstacle.enabled = false;
								}
								this.CurrentDestination = this.ChangingBooth.transform;
								this.Pathfinding.target = this.ChangingBooth.transform;
							}
							else
							{
								this.CharacterAnimation.CrossFade(this.IdleAnim);
							}
						}
						else if (!this.GoAway)
						{
							this.CurrentDestination = this.Destinations[this.Phase];
							this.Pathfinding.target = this.Destinations[this.Phase];
						}
					}
					else if (this.ClubAttire)
					{
						if (!this.ChangingBooth.Occupied)
						{
							if (this.CurrentDestination == this.ChangingBooth.transform)
							{
								this.ChangingBooth.Occupied = true;
								this.ChangingBooth.Student = this;
								this.ChangingBooth.CheckYandereClub();
							}
							this.CurrentDestination = this.ChangingBooth.transform;
							this.Pathfinding.target = this.ChangingBooth.transform;
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.IdleAnim);
						}
					}
					else if (this.Actions[this.Phase] != StudentActionType.Clean)
					{
						this.CurrentDestination = this.Destinations[this.Phase];
						this.Pathfinding.target = this.Destinations[this.Phase];
					}
				}
				if (!this.InEvent)
				{
					if (!this.Meeting)
					{
						if (!this.GoAway)
						{
							if (this.Actions[this.Phase] == StudentActionType.AtLocker)
							{
								if (this.MustChangeClothing)
								{
									Debug.Log(this.Name + " is calling ChangeClothing() from here, specifically.");
									this.ChangeClothing();
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
									this.Pathfinding.canSearch = false;
									this.Pathfinding.canMove = false;
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Socializing || (this.Actions[this.Phase] == StudentActionType.Follow && this.FollowTarget != null && this.FollowTarget.Actions[this.Phase] != StudentActionType.Clean && this.FollowTargetDistance < 1f && !this.FollowTarget.Alone && !this.FollowTarget.InEvent && !this.FollowTarget.Talking && !this.FollowTarget.Meeting && !this.FollowTarget.Confessing && this.FollowTarget.DistanceToDestination < 1f))
							{
								bool flag3 = false;
								if (this.FollowTarget != null)
								{
									if ((!this.FollowTarget.Alive && this.FollowTarget.Ragdoll.Concealed) || (!this.FollowTarget.Alive && !this.FollowTarget.gameObject.activeInHierarchy))
									{
										if (base.transform.position.y > -1f)
										{
											this.RaibaruCannotFindOsana();
											flag3 = true;
										}
									}
									else if (this.FollowTarget.Indoors && this.FollowTarget.CurrentAction != StudentActionType.SearchPatrol)
									{
										this.CurrentDestination = this.FollowTarget.FollowTargetDestination;
										this.Pathfinding.target = this.FollowTarget.FollowTargetDestination;
										this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 1f);
										this.MoveTowardsTarget(this.CurrentDestination.position);
									}
									else if (this.FollowTarget.CurrentAction == StudentActionType.Clean)
									{
										this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(-1f, 0f, -1f);
									}
									else
									{
										this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 0f);
									}
								}
								if (this.MyPlate != null && this.MyPlate.parent == this.RightHand)
								{
									this.ClubActivityPhase = 0;
									this.MyPlate.parent = null;
									this.MyPlate.position = this.OriginalPlatePosition;
									this.MyPlate.rotation = this.OriginalPlateRotation;
									this.IdleAnim = this.OriginalIdleAnim;
									this.WalkAnim = this.OriginalWalkAnim;
									this.LeanAnim = this.OriginalLeanAnim;
									this.ResumeDistracting = false;
									this.Distracting = false;
									this.Distracted = false;
									this.CanTalk = true;
								}
								if (this.Paranoia > 1.66666f && !this.StudentManager.LoveSick && this.Club != ClubType.Delinquent)
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
								else
								{
									this.StudentManager.ConvoManager.CheckMe(this.StudentID);
									if (this.Club == ClubType.Delinquent)
									{
										if (this.Alone)
										{
											if (!this.Phoneless && this.TrueAlone)
											{
												if (!this.SmartPhone.activeInHierarchy)
												{
													this.CharacterAnimation.CrossFade("delinquentTexting_00");
													this.SmartPhone.SetActive(true);
													this.SpeechLines.Stop();
												}
											}
											else
											{
												this.CharacterAnimation.CrossFade(this.IdleAnim);
												this.SpeechLines.Stop();
											}
										}
										else
										{
											if (!this.InEvent)
											{
												if (!this.Grudge)
												{
													if (!this.SpeechLines.isPlaying)
													{
														this.SmartPhone.SetActive(false);
														this.SpeechLines.Play();
													}
												}
												else
												{
													this.SmartPhone.SetActive(false);
												}
											}
											this.CharacterAnimation.CrossFade(this.RandomAnim);
											if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
											{
												this.PickRandomAnim();
											}
										}
									}
									else if (this.Alone)
									{
										if (!this.Male)
										{
											if (!this.Phoneless)
											{
												if (!flag3)
												{
													this.CharacterAnimation.CrossFade("f02_standTexting_00");
													this.SmartPhone.SetActive(true);
													this.SpeechLines.Stop();
												}
											}
											else
											{
												this.CharacterAnimation.CrossFade(this.PatrolAnim);
												this.SpeechLines.Stop();
											}
										}
										else if (!this.Phoneless)
										{
											if (this.StudentID == 36)
											{
												this.CharacterAnimation.CrossFade(this.ClubAnim);
											}
											else if (this.StudentID == 66)
											{
												this.CharacterAnimation.CrossFade("delinquentTexting_00");
											}
											else
											{
												this.CharacterAnimation.CrossFade("standTexting_00");
											}
											if (!this.SmartPhone.activeInHierarchy && !this.Shy)
											{
												if (this.StudentID == 36)
												{
													this.SmartPhone.transform.localPosition = new Vector3(0.0566666f, -0.02f, 0f);
													this.SmartPhone.transform.localEulerAngles = new Vector3(10f, 115f, 180f);
												}
												else
												{
													this.SmartPhone.transform.localPosition = new Vector3(0.015f, 0.01f, 0.025f);
													this.SmartPhone.transform.localEulerAngles = new Vector3(10f, -160f, 165f);
												}
												this.SmartPhone.SetActive(true);
												this.SpeechLines.Stop();
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.PatrolAnim);
											this.SpeechLines.Stop();
										}
									}
									else if ((this.FollowTarget != null && !this.FollowTarget.gameObject.activeInHierarchy) || (this.FollowTarget != null && !this.FollowTarget.enabled))
									{
										Debug.Log("Raibaru can't find Osana.");
										this.RaibaruCannotFindOsana();
									}
									else
									{
										if (!this.InEvent)
										{
											if (!this.Grudge)
											{
												if (!this.SpeechLines.isPlaying)
												{
													this.SmartPhone.SetActive(false);
													this.SpeechLines.Play();
												}
											}
											else
											{
												this.SmartPhone.SetActive(false);
											}
										}
										if (this.Club != ClubType.Photography)
										{
											this.CharacterAnimation.CrossFade(this.RandomAnim);
											if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
											{
												this.PickRandomAnim();
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.RandomSleuthAnim);
											if (this.CharacterAnimation[this.RandomSleuthAnim].time >= this.CharacterAnimation[this.RandomSleuthAnim].length)
											{
												this.PickRandomSleuthAnim();
											}
										}
									}
								}
								if (this.PyroUrge)
								{
									this.UpdatePyroUrge();
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Gossip)
							{
								if (this.Paranoia > 1.66666f && !this.StudentManager.LoveSick)
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
								else
								{
									this.StudentManager.ConvoManager.CheckMe(this.StudentID);
									if (this.Alone)
									{
										if (!this.Shy)
										{
											if (!this.Male)
											{
												this.CharacterAnimation.CrossFade("f02_standTexting_00");
											}
											else
											{
												this.CharacterAnimation.CrossFade("standTexting_00");
											}
											if (!this.SmartPhone.activeInHierarchy)
											{
												this.SmartPhone.SetActive(true);
												this.SpeechLines.Stop();
											}
										}
									}
									else
									{
										if (!this.SpeechLines.isPlaying)
										{
											this.SmartPhone.SetActive(false);
											this.SpeechLines.Play();
										}
										this.CharacterAnimation.CrossFade(this.RandomGossipAnim);
										if (this.CharacterAnimation[this.RandomGossipAnim].time >= this.CharacterAnimation[this.RandomGossipAnim].length)
										{
											this.PickRandomGossipAnim();
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Gaming)
							{
								this.CharacterAnimation.CrossFade(this.GameAnim);
								if (this.PyroUrge)
								{
									this.UpdatePyroUrge();
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Shamed)
							{
								this.CharacterAnimation.CrossFade(this.SadSitAnim);
							}
							else if (this.Actions[this.Phase] == StudentActionType.Slave)
							{
								this.CharacterAnimation.CrossFade(this.BrokenSitAnim);
								if (this.FragileSlave)
								{
									if (this.HuntTarget == null)
									{
										this.HuntTarget = this;
										this.GoCommitMurder();
									}
									else if (this.HuntTarget.Indoors)
									{
										this.GoCommitMurder();
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Relax)
							{
								if (this.CuriosityPhase == 0)
								{
									this.CharacterAnimation.CrossFade(this.RelaxAnim);
									if (this.Curious)
									{
										this.CuriosityTimer += Time.deltaTime;
										if (this.CuriosityTimer > 30f)
										{
											if (!this.StudentManager.Students[this.Crush].Private && !this.StudentManager.Students[this.Crush].Wet && !this.StudentManager.LockerRoomArea.bounds.Contains(this.StudentManager.Students[this.Crush].transform.position) && !this.StudentManager.EastBathroomArea.bounds.Contains(this.StudentManager.Students[this.Crush].transform.position) && !this.StudentManager.WestBathroomArea.bounds.Contains(this.StudentManager.Students[this.Crush].transform.position))
											{
												this.Pathfinding.target = this.StudentManager.Students[this.Crush].transform;
												this.CurrentDestination = this.StudentManager.Students[this.Crush].transform;
												this.DistanceToDestination = 100f;
												this.TargetDistance = 3.9f;
												this.CuriosityTimer = 0f;
												this.CuriosityPhase++;
											}
											else
											{
												this.CuriosityTimer = 0f;
											}
										}
									}
								}
								else
								{
									if (this.Pathfinding.target != this.StudentManager.Students[this.Crush].transform)
									{
										this.Pathfinding.target = this.StudentManager.Students[this.Crush].transform;
										this.CurrentDestination = this.StudentManager.Students[this.Crush].transform;
									}
									this.TargetDistance = 6f;
									if ((!this.StudentManager.Students[this.Crush].Alive && this.StudentManager.Students[this.Crush].Ragdoll.Concealed) || (!this.StudentManager.Students[this.Crush].Alive && this.StudentManager.Students[this.Crush].Ragdoll.Disposed) || this.StudentManager.Students[this.Crush].Ragdoll.Zs.activeInHierarchy || !this.StudentManager.Students[this.Crush].gameObject.activeInHierarchy)
									{
										this.CharacterAnimation.CrossFade("lookLeftRightConfused_00");
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.LeanAnim);
										if (this.StudentID == 6 && this.Clock.GameplayDay == 2 && !this.BountyCollider.activeInHierarchy)
										{
											this.BountyCollider.SetActive(true);
										}
									}
									this.CuriosityTimer += Time.deltaTime;
									if (this.CuriosityTimer > 10f || this.StudentManager.Students[this.Crush].Private || this.StudentManager.Students[this.Crush].Wet)
									{
										this.Pathfinding.target = this.Destinations[this.Phase];
										this.CurrentDestination = this.Destinations[this.Phase];
										this.BountyCollider.SetActive(false);
										this.TargetDistance = 0.5f;
										this.CuriosityTimer = 0f;
										this.CuriosityPhase--;
										if ((!this.StudentManager.Students[this.Crush].Alive && this.StudentManager.Students[this.Crush].Ragdoll.Concealed) || (!this.StudentManager.Students[this.Crush].Alive && this.StudentManager.Students[this.Crush].Ragdoll.Disposed) || !this.StudentManager.Students[this.Crush].gameObject.activeInHierarchy)
										{
											this.Curious = false;
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.SitAndTakeNotes)
							{
								if (this.Follower != null && this.Follower.Actions[this.Follower.Phase] != StudentActionType.SitAndTakeNotes && this.Clock.HourTime < 15.5f)
								{
									this.Follower.GoToClass();
								}
								if (this.MyPlate != null && this.MyPlate.parent == this.RightHand)
								{
									this.ClubActivityPhase = 0;
									this.MyPlate.parent = null;
									this.MyPlate.position = this.OriginalPlatePosition;
									this.MyPlate.rotation = this.OriginalPlateRotation;
									this.CurrentDestination = this.Destinations[this.Phase];
									this.Pathfinding.target = this.Destinations[this.Phase];
									this.IdleAnim = this.OriginalIdleAnim;
									this.WalkAnim = this.OriginalWalkAnim;
									this.LeanAnim = this.OriginalLeanAnim;
									this.ResumeDistracting = false;
									this.Distracting = false;
									this.Distracted = false;
									this.CanTalk = true;
								}
								if (this.MustChangeClothing)
								{
									Debug.Log("Calling ChangeClothing() from here, specifically.");
									this.ChangeClothing();
								}
								else if (this.Bullied)
								{
									if (this.SmartPhone.activeInHierarchy)
									{
										this.SmartPhone.SetActive(false);
									}
									this.CharacterAnimation.CrossFade(this.SadDeskSitAnim, 1f);
								}
								else
								{
									if (this.Phoneless && this.StudentManager.CommunalLocker.RivalPhone.gameObject.activeInHierarchy && !this.EndSearch && this.Yandere.CanMove)
									{
										if (this.Rival)
										{
											this.LewdPhotos = this.StudentManager.CommunalLocker.RivalPhone.LewdPhotos;
											if (DateGlobals.Weekday == DayOfWeek.Monday)
											{
												SchemeGlobals.SetSchemeStage(1, 8);
												this.Yandere.PauseScreen.Schemes.UpdateInstructions();
											}
										}
										Debug.Log(this.Name + " found her lost phone from this spot in the code. 1");
										this.CharacterAnimation.CrossFade(this.DiscoverPhoneAnim);
										this.Subtitle.UpdateLabel(this.LostPhoneSubtitleType, 2, 5f);
										this.SearchingForPhone = false;
										this.Phoneless = false;
										this.EndSearch = true;
										this.Routine = false;
										if (this.EventToSabotage != null)
										{
											this.EventToSabotage.gameObject.SetActive(true);
										}
									}
									if (!this.EndSearch)
									{
										if (this.Clock.Period != 2 && this.Clock.Period != 4)
										{
											if (this.DressCode && this.ClubAttire)
											{
												this.CharacterAnimation.CrossFade(this.IdleAnim);
											}
											else if ((double)Vector3.Distance(base.transform.position, this.Seat.position) < 0.5)
											{
												if ((this.StudentID == 1 && this.StudentManager.Gift.activeInHierarchy) || (this.StudentID == 1 && this.StudentManager.Note.activeInHierarchy))
												{
													this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
													this.CharacterAnimation.CrossFade(this.InspectBloodAnim);
													if (this.CharacterAnimation[this.InspectBloodAnim].time >= this.CharacterAnimation[this.InspectBloodAnim].length)
													{
														this.StudentManager.SenpaiLoveWindow.UpdateSenpaiLove();
														this.StudentManager.Gift.SetActive(false);
														this.StudentManager.Note.SetActive(false);
													}
												}
												else if (!this.Phoneless)
												{
													if (this.Club != ClubType.Delinquent)
													{
														if (!this.StudentManager.Eighties)
														{
															if (!this.SmartPhone.activeInHierarchy)
															{
																if (this.Male)
																{
																	this.SmartPhone.transform.localPosition = new Vector3(0.025f, 0.0025f, 0.025f);
																	this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, 180f);
																}
																else
																{
																	this.SmartPhone.transform.localPosition = new Vector3(0.01f, 0.01f, 0.01f);
																	this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, 165f);
																}
																this.SmartPhone.SetActive(true);
															}
															this.CharacterAnimation.CrossFade(this.DeskTextAnim);
														}
														else
														{
															if (this.SmartPhone.activeInHierarchy)
															{
																this.SmartPhone.SetActive(false);
															}
															this.CharacterAnimation.CrossFade(this.ConfusedSitAnim);
														}
													}
													else
													{
														this.CharacterAnimation.CrossFade("delinquentSit_00");
													}
												}
												else
												{
													this.CharacterAnimation.CrossFade(this.ConfusedSitAnim);
												}
											}
										}
										else if (this.StudentManager.Teachers[this.Class].SpeechLines.isPlaying && !this.StudentManager.Teachers[this.Class].Alarmed)
										{
											if (this.DressCode && this.ClubAttire)
											{
												this.CharacterAnimation.CrossFade(this.IdleAnim);
											}
											else
											{
												if (!this.Depressed && !this.Pen.activeInHierarchy)
												{
													this.Pen.SetActive(true);
												}
												if (this.SmartPhone.activeInHierarchy)
												{
													this.SmartPhone.SetActive(false);
												}
												if (this.MyPaper == null)
												{
													if (base.transform.position.x < 0f)
													{
														this.MyPaper = UnityEngine.Object.Instantiate<GameObject>(this.Paper, this.Seat.position + new Vector3(-0.4f, 0.772f, 0f), Quaternion.identity);
													}
													else
													{
														this.MyPaper = UnityEngine.Object.Instantiate<GameObject>(this.Paper, this.Seat.position + new Vector3(0.4f, 0.772f, 0f), Quaternion.identity);
													}
													this.MyPaper.transform.eulerAngles = new Vector3(0f, 0f, -90f);
													this.MyPaper.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
													this.MyPaper.transform.parent = this.StudentManager.Papers;
												}
												this.CharacterAnimation.CrossFade(this.SitAnim);
											}
										}
										else if (this.Club != ClubType.Delinquent)
										{
											this.CharacterAnimation.CrossFade(this.ConfusedSitAnim);
										}
										else
										{
											this.CharacterAnimation.CrossFade("delinquentSit_00");
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Peek)
							{
								this.CharacterAnimation.CrossFade(this.PeekAnim);
								if (this.Male)
								{
									this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].SetFloat("_BlendAmount", 1f);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.ClubAction)
							{
								if (this.DressCode && !this.ClubAttire)
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
								else
								{
									if (this.StudentID == 47 || this.StudentID == 49)
									{
										if (this.GetNewAnimation)
										{
											this.StudentManager.ConvoManager.MartialArtsCheck();
										}
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											this.GetNewAnimation = true;
										}
									}
									if (this.Club != ClubType.Occult)
									{
										this.CharacterAnimation.CrossFade(this.ClubAnim);
									}
								}
								if (this.Club == ClubType.Cooking)
								{
									if (this.ClubActivityPhase == 0)
									{
										if (this.ClubTimer == 0f)
										{
											this.ClubActivityPhase = 0;
											this.MyPlate.parent = null;
											this.MyPlate.gameObject.SetActive(true);
											this.MyPlate.position = this.OriginalPlatePosition;
											this.MyPlate.rotation = this.OriginalPlateRotation;
										}
										this.ClubTimer += Time.deltaTime;
										if (this.ClubTimer > 60f)
										{
											this.MyPlate.parent = this.RightHand;
											this.MyPlate.localPosition = new Vector3(0.02f, -0.02f, -0.15f);
											this.MyPlate.localEulerAngles = new Vector3(-5f, -90f, 172.5f);
											this.IdleAnim = this.PlateIdleAnim;
											this.WalkAnim = this.PlateWalkAnim;
											this.LeanAnim = this.PlateIdleAnim;
											this.GetFoodTarget();
											this.ClubTimer = 0f;
											this.ClubActivityPhase++;
										}
									}
									else
									{
										this.GetFoodTarget();
									}
								}
								else if (this.OriginalClub == ClubType.Drama)
								{
									this.StudentManager.DramaTimer += Time.deltaTime;
									if (this.StudentManager.DramaPhase == 1)
									{
										this.StudentManager.ConvoManager.CheckMe(this.StudentID);
										if (this.Alone)
										{
											if (this.Phoneless)
											{
												if (this.ClubLeader)
												{
													this.CharacterAnimation.CrossFade(this.IdleAnim);
												}
												else if (!this.Male)
												{
													this.CharacterAnimation.CrossFade("f02_sit_01");
												}
												else
												{
													this.CharacterAnimation.CrossFade("sit_00");
												}
											}
											else
											{
												if (this.Male)
												{
													this.CharacterAnimation.CrossFade("standTexting_00");
												}
												else
												{
													this.CharacterAnimation.CrossFade("f02_standTexting_00");
												}
												if (!this.SmartPhone.activeInHierarchy)
												{
													this.SmartPhone.transform.localPosition = new Vector3(0.02f, 0.01f, 0.03f);
													this.SmartPhone.transform.localEulerAngles = new Vector3(5f, -160f, 180f);
													this.SmartPhone.SetActive(true);
													this.SpeechLines.Stop();
												}
											}
										}
										else if (this.StudentID == 26 && !this.SpeechLines.isPlaying)
										{
											this.SmartPhone.SetActive(false);
											this.SpeechLines.Play();
										}
										if (this.StudentManager.DramaTimer > 100f)
										{
											this.StudentManager.DramaTimer = 0f;
											this.StudentManager.UpdateDrama();
										}
									}
									else if (this.StudentManager.DramaPhase == 2)
									{
										if (this.StudentManager.DramaTimer > 300f)
										{
											this.StudentManager.DramaTimer = 0f;
											this.StudentManager.UpdateDrama();
										}
									}
									else if (this.StudentManager.DramaPhase == 3)
									{
										this.SpeechLines.Play();
										this.CharacterAnimation.CrossFade(this.RandomAnim);
										if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
										{
											this.PickRandomAnim();
										}
										if (this.StudentManager.DramaTimer > 100f)
										{
											this.StudentManager.DramaTimer = 0f;
											this.StudentManager.UpdateDrama();
										}
									}
								}
								else if (this.Club == ClubType.Occult)
								{
									if (this.ReadPhase == 0)
									{
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										this.CharacterAnimation.CrossFade(this.BookSitAnim);
										if (this.CharacterAnimation[this.BookSitAnim].time > this.CharacterAnimation[this.BookSitAnim].length)
										{
											this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
											this.CharacterAnimation.CrossFade(this.BookReadAnim);
											this.ReadPhase++;
										}
										else if (this.CharacterAnimation[this.BookSitAnim].time > 1f)
										{
											this.OccultBook.SetActive(true);
										}
									}
								}
								else if (this.Club == ClubType.Art)
								{
									if (this.ClubAttire && !this.Paintbrush.activeInHierarchy && (double)Vector3.Distance(base.transform.position, this.CurrentDestination.position) < 0.5)
									{
										this.Paintbrush.SetActive(true);
										this.Palette.SetActive(true);
									}
								}
								else if (this.OriginalClub == ClubType.LightMusic)
								{
									if ((double)this.Clock.HourTime < 16.9)
									{
										this.Instruments[this.ClubMemberID].SetActive(true);
										this.CharacterAnimation.CrossFade(this.ClubAnim);
										if (this.StudentID == 51)
										{
											if (this.InstrumentBag[this.ClubMemberID].transform.parent != null)
											{
												this.InstrumentBag[this.ClubMemberID].transform.parent = null;
												if (!this.StudentManager.Eighties)
												{
													this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(0.5f, 4.5f, 22.45666f);
													this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, 0f, 0f);
												}
												else
												{
													this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(2.06f, 4.5f, 26.5f);
													this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
												}
											}
											if (this.Instruments[this.ClubMemberID].transform.parent == null)
											{
												this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Play();
												this.Instruments[this.ClubMemberID].transform.parent = base.transform;
												this.Instruments[this.ClubMemberID].transform.localPosition = new Vector3(0.340493f, 0.653502f, -0.286104f);
												this.Instruments[this.ClubMemberID].transform.localEulerAngles = new Vector3(-13.6139f, 16.16775f, 72.5293f);
											}
										}
										else if (this.StudentID == 54 && !this.Drumsticks[0].activeInHierarchy)
										{
											this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Play();
											this.Drumsticks[0].SetActive(true);
											this.Drumsticks[1].SetActive(true);
										}
									}
									else if (this.StudentID == 51)
									{
										this.InstrumentBag[this.ClubMemberID].transform.parent;
										if (!this.StudentManager.Eighties)
										{
											this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(0.5f, 4.5f, 22.45666f);
											this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, 0f, 0f);
										}
										else
										{
											this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(2.06f, 4.5f, 26.5f);
											this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
										}
										this.InstrumentBag[this.ClubMemberID].transform.parent = null;
										if (!this.StudentManager.PracticeMusic.isPlaying)
										{
											this.CharacterAnimation.CrossFade("f02_vocalIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 114.5f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalCelebrate_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 104f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalWait_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 32f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalSingB_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 24f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalSingB_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 17f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalSingB_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 14f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalWait_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 8f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalSingA_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 0f)
										{
											this.CharacterAnimation.CrossFade("f02_vocalWait_00");
										}
									}
									else if (this.StudentID == 52)
									{
										if (!this.Instruments[this.ClubMemberID].activeInHierarchy)
										{
											this.Instruments[this.ClubMemberID].SetActive(true);
											this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Stop();
											this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().playOnAwake = false;
											this.Instruments[this.ClubMemberID].transform.parent = this.Spine;
											this.Instruments[this.ClubMemberID].transform.localPosition = new Vector3(0.275f, -0.16f, 0.095f);
											this.Instruments[this.ClubMemberID].transform.localEulerAngles = new Vector3(-22.5f, 30f, 60f);
											this.InstrumentBag[this.ClubMemberID].transform.parent = null;
											this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(5.5825f, 4.01f, 25f);
											this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
										}
										if (!this.StudentManager.PracticeMusic.isPlaying)
										{
											this.CharacterAnimation.CrossFade("f02_guitarIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 114.5f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarCelebrate_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 112f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarWait_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 64f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarPlayA_01");
										}
										else if (this.StudentManager.PracticeMusic.time > 8f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarPlayA_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 0f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarWait_00");
										}
									}
									else if (this.StudentID == 53)
									{
										if (!this.Instruments[this.ClubMemberID].activeInHierarchy)
										{
											this.Instruments[this.ClubMemberID].SetActive(true);
											this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Stop();
											this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().playOnAwake = false;
											this.Instruments[this.ClubMemberID].transform.parent = this.Spine;
											this.Instruments[this.ClubMemberID].transform.localPosition = new Vector3(0.275f, -0.16f, 0.095f);
											this.Instruments[this.ClubMemberID].transform.localEulerAngles = new Vector3(-22.5f, 30f, 60f);
											this.InstrumentBag[this.ClubMemberID].transform.parent = null;
											this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(5.5825f, 4.01f, 26f);
											this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
										}
										if (!this.StudentManager.PracticeMusic.isPlaying)
										{
											this.CharacterAnimation.CrossFade("f02_guitarIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 114.5f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarCelebrate_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 112f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarWait_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 88f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarPlayA_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 80f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarWait_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 64f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarPlayB_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 0f)
										{
											this.CharacterAnimation.CrossFade("f02_guitarPlaySlowA_01");
										}
									}
									else if (this.StudentID == 54)
									{
										if (this.InstrumentBag[this.ClubMemberID].transform.parent != null)
										{
											this.InstrumentBag[this.ClubMemberID].transform.parent = null;
											this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(5.5825f, 4.01f, 23f);
											this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
										}
										this.Drumsticks[0].SetActive(true);
										this.Drumsticks[1].SetActive(true);
										if (!this.StudentManager.PracticeMusic.isPlaying)
										{
											this.CharacterAnimation.CrossFade("f02_drumsIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 114.5f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsCelebrate_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 108f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 96f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsPlaySlow_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 80f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 38f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsPlay_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 46f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 16f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsPlay_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 0f)
										{
											this.CharacterAnimation.CrossFade("f02_drumsIdle_00");
										}
									}
									else if (this.StudentID == 55)
									{
										if (this.InstrumentBag[this.ClubMemberID].transform.parent != null)
										{
											this.InstrumentBag[this.ClubMemberID].transform.parent = null;
											this.InstrumentBag[this.ClubMemberID].transform.position = new Vector3(5.5825f, 4.01f, 24f);
											this.InstrumentBag[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
										}
										if (!this.StudentManager.PracticeMusic.isPlaying)
										{
											this.CharacterAnimation.CrossFade("f02_keysIdle_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 114.5f)
										{
											this.CharacterAnimation.CrossFade("f02_keysCelebrate_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 80f)
										{
											this.CharacterAnimation.CrossFade("f02_keysWait_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 24f)
										{
											this.CharacterAnimation.CrossFade("f02_keysPlay_00");
										}
										else if (this.StudentManager.PracticeMusic.time > 0f)
										{
											this.CharacterAnimation.CrossFade("f02_keysWait_00");
										}
									}
								}
								else if (this.Club == ClubType.Science)
								{
									if ((this.ClubAttire && !this.GoAway) || (this.StudentManager.Eighties && !this.GoAway))
									{
										if (this.SciencePhase == 0)
										{
											this.CharacterAnimation.CrossFade(this.ClubAnim);
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.RummageAnim);
										}
										if ((double)Vector3.Distance(base.transform.position, this.CurrentDestination.position) < 0.5)
										{
											if (this.SciencePhase == 0)
											{
												if (!this.StudentManager.Eighties)
												{
													if (this.StudentID == 62)
													{
														this.ScienceProps[0].SetActive(true);
													}
													else if (this.StudentID == 63)
													{
														this.ScienceProps[1].SetActive(true);
														this.ScienceProps[2].SetActive(true);
													}
													else if (this.StudentID == 64)
													{
														this.ScienceProps[0].SetActive(true);
													}
													else if (this.StudentID == 65 && this.StudentManager.RobotPhase == 1)
													{
														if (!this.StudentManager.RobotChan.Hunting)
														{
															this.CharacterAnimation.CrossFade(this.RandomAnim);
															if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
															{
																this.PickRandomAnim();
															}
														}
														else
														{
															this.CharacterAnimation.CrossFade("f02_lookLeftRight_00_loop");
														}
													}
												}
												else
												{
													if (!this.Male && !this.ScienceProps[1].activeInHierarchy)
													{
														this.CharacterAnimation.Play("f02_straightenSkirt_00");
													}
													this.ScienceProps[1].SetActive(true);
													this.ScienceProps[2].SetActive(true);
												}
											}
											if (this.StudentID > 61)
											{
												this.ClubTimer += Time.deltaTime;
												if (this.ClubTimer > 60f)
												{
													this.ClubTimer = 0f;
													this.SciencePhase++;
													if (this.SciencePhase == 1)
													{
														this.ClubTimer = 50f;
														this.Destinations[this.Phase] = this.StudentManager.SupplySpots[this.StudentID - 61];
														this.CurrentDestination = this.StudentManager.SupplySpots[this.StudentID - 61];
														this.Pathfinding.target = this.StudentManager.SupplySpots[this.StudentID - 61];
														foreach (GameObject gameObject3 in this.ScienceProps)
														{
															if (gameObject3 != null)
															{
																gameObject3.SetActive(false);
															}
														}
													}
													else
													{
														this.SciencePhase = 0;
														this.ClubTimer = 0f;
														this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID];
														this.CurrentDestination = this.StudentManager.Clubs.List[this.StudentID];
														this.Pathfinding.target = this.StudentManager.Clubs.List[this.StudentID];
													}
												}
											}
										}
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.IdleAnim);
									}
								}
								else if (this.OriginalClub == ClubType.Sports)
								{
									this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
									if (this.ClubActivityPhase == 0)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											string str = "";
											if (!this.Male)
											{
												str = "f02_";
											}
											this.ClubActivityPhase++;
											this.ClubAnim = str + "stretch_01";
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase == 1)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											string str2 = "";
											if (!this.Male)
											{
												str2 = "f02_";
											}
											this.ClubActivityPhase++;
											this.ClubAnim = str2 + "stretch_02";
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase == 2)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											bool male = this.Male;
											this.Hurry = true;
											this.ClubActivityPhase++;
											this.CharacterAnimation[this.ClubAnim].time = 0f;
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase < 14)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											this.ClubActivityPhase++;
											this.CharacterAnimation[this.ClubAnim].time = 0f;
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase == 14)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											this.WalkAnim = this.OriginalWalkAnim;
											string str3 = "";
											if (!this.Male)
											{
												str3 = "f02_";
											}
											this.Hurry = false;
											this.ClubActivityPhase = 0;
											this.ClubAnim = str3 + "stretch_00";
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase == 15)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= 1f && this.MyController.radius > 0f)
										{
											this.MyRenderer.updateWhenOffscreen = true;
											this.Obstacle.enabled = false;
											this.MyController.radius = 0f;
											this.Distracted = true;
										}
										if (!this.StudentManager.Eighties && this.CharacterAnimation[this.ClubAnim].time >= 2f)
										{
											float value = this.Cosmetic.Goggles[this.StudentID].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) + Time.deltaTime * 200f;
											this.Cosmetic.Goggles[this.StudentID].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, value);
										}
										if (this.CharacterAnimation[this.ClubAnim].time >= 5f)
										{
											this.ClubActivityPhase++;
										}
									}
									else if (this.ClubActivityPhase == 16)
									{
										if ((double)this.CharacterAnimation[this.ClubAnim].time >= 6.1)
										{
											if (!this.StudentManager.Eighties)
											{
												this.Cosmetic.Goggles[this.StudentID].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100f);
												this.Cosmetic.MaleHair[this.Cosmetic.Hairstyle].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100f);
											}
											GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.BigWaterSplash, this.RightHand.transform.position, Quaternion.identity);
											gameObject4.transform.eulerAngles = new Vector3(-90f, gameObject4.transform.eulerAngles.y, gameObject4.transform.eulerAngles.z);
											this.SetSplashes(true);
											this.ClubActivityPhase++;
										}
									}
									else if (this.ClubActivityPhase == 17)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											this.WalkAnim = this.GenderPrefix + "poolSwim_00";
											this.ClubAnim = this.GenderPrefix + "poolSwim_00";
											this.ClubActivityPhase++;
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase - 2);
											base.transform.position = this.Hips.transform.position;
											this.Character.transform.localPosition = new Vector3(0f, -0.25f, 0f);
											Physics.SyncTransforms();
											this.CharacterAnimation.Play(this.WalkAnim);
										}
									}
									else if (this.ClubActivityPhase == 18)
									{
										this.ClubActivityPhase++;
										this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase - 2);
										this.DistanceToDestination = 100f;
									}
									else if (this.ClubActivityPhase == 19)
									{
										this.ClubAnim = this.GenderPrefix + "poolExit_00";
										if (this.CharacterAnimation[this.ClubAnim].time >= 0.1f)
										{
											this.SetSplashes(false);
										}
										if (!this.StudentManager.Eighties && this.CharacterAnimation[this.ClubAnim].time >= 4.66666f)
										{
											float value2 = this.Cosmetic.Goggles[this.StudentID].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) - Time.deltaTime * 200f;
											this.Cosmetic.Goggles[this.StudentID].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, value2);
										}
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											this.ClubActivityPhase = 15;
											this.ClubAnim = this.GenderPrefix + "poolDive_00";
											this.MyController.radius = 0.1f;
											this.WalkAnim = this.OriginalWalkAnim;
											this.MyRenderer.updateWhenOffscreen = false;
											this.Character.transform.localPosition = new Vector3(0f, 0f, 0f);
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
											if (!this.StudentManager.Eighties)
											{
												this.Cosmetic.Goggles[this.StudentID].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0f);
											}
											base.transform.position = new Vector3(this.Hips.position.x, 4f, this.Hips.position.z);
											Physics.SyncTransforms();
											this.CharacterAnimation.Play(this.IdleAnim);
											this.Distracted = false;
											if (this.Clock.Period == 2 || this.Clock.Period == 4)
											{
												this.Pathfinding.speed = 4f;
											}
										}
									}
								}
								else if (this.OriginalClub == ClubType.Gardening)
								{
									if ((!this.StudentManager.Eighties || this.WaterLow) && this.WateringCan.transform.parent != this.RightHand)
									{
										this.WateringCan.transform.parent = this.RightHand;
										this.WateringCan.transform.localPosition = new Vector3(0.14f, -0.15f, -0.05f);
										this.WateringCan.transform.localEulerAngles = new Vector3(10f, 15f, 45f);
									}
									this.PatrolTimer += Time.deltaTime * this.CharacterAnimation[this.PatrolAnim].speed;
									if (this.PatrolTimer >= this.CharacterAnimation[this.ClubAnim].length)
									{
										this.PatrolID++;
										if (this.PatrolID == this.StudentManager.Patrols.List[this.StudentID].childCount)
										{
											this.PatrolID = 0;
										}
										this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
										this.Pathfinding.target = this.CurrentDestination;
										this.PatrolTimer = 0f;
										if (!this.StudentManager.Eighties || this.WaterLow)
										{
											this.WateringCan.transform.parent = this.Hips;
											this.WateringCan.transform.localPosition = new Vector3(0f, 0.0135f, -0.184f);
											this.WateringCan.transform.localEulerAngles = new Vector3(0f, 90f, 30f);
										}
									}
								}
								else if (this.OriginalClub == ClubType.Gaming)
								{
									if (this.Phase < 8)
									{
										if (this.StudentID == 36 && !this.SmartPhone.activeInHierarchy)
										{
											this.SmartPhone.SetActive(true);
											this.SmartPhone.transform.localPosition = new Vector3(0.0566666f, -0.02f, 0f);
											this.SmartPhone.transform.localEulerAngles = new Vector3(10f, 115f, 180f);
										}
									}
									else
									{
										if (!this.ClubManager.GameScreens[this.ClubMemberID].activeInHierarchy)
										{
											this.ClubManager.GameScreens[this.ClubMemberID].SetActive(true);
											this.MyController.radius = 0.2f;
										}
										if (this.SmartPhone.activeInHierarchy)
										{
											this.SmartPhone.SetActive(false);
										}
									}
								}
								else if (this.Club == ClubType.Newspaper && this.StudentID > 36)
								{
									this.ClubTimer += Time.deltaTime;
									if (this.ClubTimer > 30f)
									{
										if (this.CurrentDestination.position.y > 0f)
										{
											this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(0);
											this.Pathfinding.target = this.CurrentDestination;
											this.ClubAnim = this.PatrolAnim;
										}
										else
										{
											this.CurrentDestination = this.StudentManager.Clubs.List[this.StudentID];
											this.Pathfinding.target = this.CurrentDestination;
											this.ClubAnim = this.OriginalClubAnim;
										}
										this.ClubTimer = 0f;
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.SitAndSocialize)
							{
								if (this.Paranoia > 1.66666f)
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
								else
								{
									this.StudentManager.ConvoManager.CheckMe(this.StudentID);
									if (this.Alone)
									{
										if (!this.Male)
										{
											this.CharacterAnimation.CrossFade("f02_standTexting_00");
										}
										else
										{
											this.CharacterAnimation.CrossFade("standTexting_00");
										}
										if (!this.SmartPhone.activeInHierarchy)
										{
											this.SmartPhone.SetActive(true);
											this.SpeechLines.Stop();
										}
									}
									else
									{
										if (!this.InEvent && !this.SpeechLines.isPlaying)
										{
											this.SmartPhone.SetActive(false);
											this.SpeechLines.Play();
										}
										if (this.Club != ClubType.Photography)
										{
											this.CharacterAnimation.CrossFade(this.RandomAnim);
											if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
											{
												this.PickRandomAnim();
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.RandomSleuthAnim);
											if (this.CharacterAnimation[this.RandomSleuthAnim].time >= this.CharacterAnimation[this.RandomSleuthAnim].length)
											{
												this.PickRandomSleuthAnim();
											}
										}
									}
									if (this.StudentID == 56 && this.Clock.GameplayDay == 7)
									{
										this.BountyCollider.SetActive(true);
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.SitAndEatBento)
							{
								if (this.MustChangeClothing)
								{
									if (this.Schoolwear == 1 || this.Schoolwear == 3)
									{
										this.WalkAnim = this.OriginalWalkAnim;
										this.MustChangeClothing = false;
									}
									else
									{
										Debug.Log("Calling ChangeClothing() from here, specifically.");
										this.ChangeClothing();
									}
								}
								else if (!this.DiscCheck && this.Alarm < 100f)
								{
									if (!this.Ragdoll.Poisoned && (!this.Bento.activeInHierarchy || this.Bento.transform.parent == null))
									{
										this.SmartPhone.SetActive(false);
										if (!this.Male)
										{
											this.Bento.transform.parent = this.LeftItemParent;
											this.Bento.transform.localPosition = new Vector3(-0.025f, -0.105f, 0f);
											this.Bento.transform.localEulerAngles = new Vector3(0f, 165f, 82.5f);
										}
										else
										{
											this.Bento.transform.parent = this.LeftItemParent;
											this.Bento.transform.localPosition = new Vector3(-0.05f, -0.085f, 0f);
											this.Bento.transform.localEulerAngles = new Vector3(-3.2f, -24.4f, -94f);
										}
										this.Chopsticks[0].SetActive(true);
										this.Chopsticks[1].SetActive(true);
										this.Bento.SetActive(true);
										this.Lid.SetActive(false);
										this.MyBento.Prompt.Hide();
										this.MyBento.Prompt.enabled = false;
										if (this.MyBento.Tampered)
										{
											if (this.MyBento.Emetic)
											{
												Debug.Log(this.Name + "'s bento contains emetic medicine.");
												this.Emetic = true;
											}
											else if (this.MyBento.Lethal)
											{
												this.Lethal = true;
											}
											else if (this.MyBento.Tranquil)
											{
												this.Sedated = true;
											}
											else if (this.MyBento.Headache)
											{
												Debug.Log(this.Name + "'s bento contains headache medicine.");
												this.Headache = true;
											}
											this.Distracted = true;
											this.Alarm = 0f;
											this.UpdateDetectionMarker();
										}
									}
									if (!this.Emetic && !this.Lethal && !this.Sedated && !this.Headache)
									{
										this.CharacterAnimation.CrossFade(this.EatAnim);
										if (this.FollowTarget != null && ((this.FollowTarget.CurrentAction != StudentActionType.SitAndEatBento && !this.FollowTarget.Dying && !this.FollowTarget.Emetic) || this.Clock.HourTime > 13.375f))
										{
											if (this.FollowTarget.Alive)
											{
												Debug.Log("Osana is no longer eating, so Raibaru is now following Osana.");
												this.CharacterAnimation.CrossFade(this.WalkAnim);
												this.EmptyHands();
												this.Pathfinding.canSearch = true;
												this.Pathfinding.canMove = true;
												ScheduleBlock scheduleBlock9 = this.ScheduleBlocks[4];
												scheduleBlock9.destination = "Follow";
												scheduleBlock9.action = "Follow";
												ScheduleBlock scheduleBlock10 = this.ScheduleBlocks[5];
												scheduleBlock10.destination = "Follow";
												scheduleBlock10.action = "Follow";
												this.GetDestinations();
												this.Pathfinding.target = this.FollowTarget.transform;
												this.CurrentDestination = this.FollowTarget.transform;
											}
											else
											{
												this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 5, 10f);
												this.RaibaruOsanaDeathScheduleChanges();
												this.RaibaruStopsFollowingOsana();
												this.GetDestinations();
												this.CurrentDestination = this.Destinations[this.Phase];
												this.Pathfinding.target = this.Destinations[this.Phase];
											}
										}
									}
									else if (this.Emetic)
									{
										if (!this.Private)
										{
											this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.CharacterAnimation.CrossFade(this.EmeticAnim);
											this.Distracted = true;
											this.Private = true;
											this.CanTalk = false;
										}
										if (this.CharacterAnimation[this.EmeticAnim].time >= 16f)
										{
											if (this.StudentID == 10)
											{
												if (this.VomitPhase < 0)
												{
													this.Subtitle.UpdateLabel(SubtitleType.ObstaclePoisonReaction, 0, 9f);
													this.VomitPhase++;
												}
											}
											else if (this.StudentID == 11 && this.Follower != null)
											{
												this.StudentManager.LastKnownOsana.position = base.transform.position;
											}
										}
										if (this.CharacterAnimation[this.EmeticAnim].time >= this.CharacterAnimation[this.EmeticAnim].length)
										{
											this.GoPuke();
										}
									}
									else if (this.Lethal)
									{
										if (!this.Private)
										{
											this.SpawnTimeRespectingAudioSource(this.PoisonDeathClip);
											if (this.Male)
											{
												this.CharacterAnimation.CrossFade("poisonDeath_00");
												this.PoisonDeathAnim = "poisonDeath_00";
											}
											else
											{
												this.CharacterAnimation.CrossFade("f02_poisonDeath_00");
												this.PoisonDeathAnim = "f02_poisonDeath_00";
												this.Distracted = true;
											}
											this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.MyRenderer.updateWhenOffscreen = true;
											this.Ragdoll.Poisoned = true;
											this.Private = true;
											this.Prompt.Hide();
											this.Prompt.enabled = false;
										}
										else if (this.StudentID == 11 && this.StudentManager.Students[1] != null && !this.StudentManager.Students[1].SenpaiWitnessingRivalDie && Vector3.Distance(base.transform.position, this.StudentManager.Students[1].transform.position) < 2f)
										{
											Debug.Log("Setting ''SenpaiWitnessingRivalDie'' to true.");
											this.StudentManager.Students[1].CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.StudentManager.Students[1].CharacterAnimation.CrossFade("witnessPoisoning_00");
											this.StudentManager.Students[1].CurrentDestination = this.StudentManager.LunchSpots.List[1];
											this.StudentManager.Students[1].Pathfinding.target = this.StudentManager.LunchSpots.List[1];
											this.StudentManager.Students[1].MyRenderer.updateWhenOffscreen = true;
											this.StudentManager.Students[1].SenpaiWitnessingRivalDie = true;
											this.StudentManager.Students[1].IgnoringPettyActions = true;
											this.StudentManager.Students[1].Distracted = true;
											this.StudentManager.Students[1].Routine = false;
										}
										if (!this.Distracted && this.CharacterAnimation[this.PoisonDeathAnim].time >= 2.5f)
										{
											this.Distracted = true;
										}
										if (this.CharacterAnimation[this.PoisonDeathAnim].time >= 17.5f && this.Bento.activeInHierarchy)
										{
											this.Police.CorpseList[this.Police.Corpses] = this.Ragdoll;
											this.Police.Corpses++;
											GameObjectUtils.SetLayerRecursively(base.gameObject, 11);
											this.MapMarker.gameObject.layer = 10;
											base.tag = "Blood";
											this.Ragdoll.ChokingAnimation = true;
											this.Ragdoll.Disturbing = true;
											this.Ragdoll.Choking = true;
											this.Dying = true;
											this.MurderSuicidePhase = 100;
											this.SpawnAlarmDisc();
											Debug.Log(this.Name + " just spawned an 'AlarmDisc'.");
											this.Chopsticks[0].SetActive(false);
											this.Chopsticks[1].SetActive(false);
											this.Bento.SetActive(false);
										}
										if (this.CharacterAnimation[this.PoisonDeathAnim].time >= this.CharacterAnimation[this.PoisonDeathAnim].length)
										{
											this.BecomeRagdoll();
											this.DeathType = DeathType.Poison;
											this.Ragdoll.Choking = false;
											if (this.StudentManager.Students[1].SenpaiWitnessingRivalDie)
											{
												this.Ragdoll.Prompt.Hide();
												this.Ragdoll.Prompt.enabled = false;
											}
										}
									}
									else if (this.Sedated)
									{
										if (!this.Private)
										{
											Debug.Log(this.Name + " is beginning to eat food that has been poisoned with a sedative.");
											this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.CharacterAnimation.CrossFade(this.HeadacheAnim);
											this.Distracted = true;
											this.CanTalk = false;
											this.Private = true;
										}
										if (this.CharacterAnimation[this.HeadacheAnim].time >= this.CharacterAnimation[this.HeadacheAnim].length)
										{
											this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
											if (this.Male)
											{
												this.SprintAnim = "headacheWalk_00";
												this.RelaxAnim = "infirmaryRest_00";
											}
											else
											{
												this.SprintAnim = "f02_headacheWalk_00";
												this.RelaxAnim = "f02_infirmaryRest_00";
											}
											this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
											this.CharacterAnimation.CrossFade(this.SprintAnim);
											this.DistanceToDestination = 100f;
											this.Pathfinding.canSearch = true;
											this.Pathfinding.canMove = true;
											this.Pathfinding.speed = 2f;
											this.MyBento.Tampered = false;
											this.Private = true;
											this.Sleepy = true;
											ScheduleBlock scheduleBlock11 = this.ScheduleBlocks[4];
											scheduleBlock11.destination = "InfirmaryBed";
											scheduleBlock11.action = "Relax";
											this.Oversleep();
											this.GetDestinations();
											this.CurrentDestination = this.Destinations[this.Phase];
											this.Pathfinding.target = this.Destinations[this.Phase];
											this.Chopsticks[0].SetActive(false);
											this.Chopsticks[1].SetActive(false);
											this.Bento.SetActive(false);
										}
									}
									else if (this.Headache)
									{
										if (!this.Private)
										{
											this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.CharacterAnimation.CrossFade(this.HeadacheAnim);
											this.CanTalk = false;
											this.Private = true;
										}
										if (this.CharacterAnimation[this.HeadacheAnim].time >= this.CharacterAnimation[this.HeadacheAnim].length)
										{
											this.GetHeadache();
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.ChangeShoes)
							{
								if (this.MustChangeClothing)
								{
									Debug.Log("Calling ChangeClothing() from here, specifically.");
									this.ChangeClothing();
								}
								else if (this.MeetTime == 0f)
								{
									if ((this.StudentID == 1 && !this.StudentManager.LoveManager.ConfessToSuitor && this.StudentManager.LoveManager.LeftNote) || (this.StudentID == this.StudentManager.LoveManager.SuitorID && this.StudentManager.LoveManager.ConfessToSuitor && this.StudentManager.LoveManager.LeftNote))
									{
										if (this.StudentID == 1)
										{
											Debug.Log("Senpai is now checking their locker...");
										}
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										if (this.Male)
										{
											this.CharacterAnimation.CrossFade("keepNote_00");
										}
										else
										{
											this.CharacterAnimation.CrossFade("f02_keepNote_00");
										}
										this.Pathfinding.canSearch = false;
										this.Pathfinding.canMove = false;
										this.Confessing = true;
										this.CanTalk = false;
										this.Routine = false;
									}
									else
									{
										this.SmartPhone.SetActive(false);
										this.Pathfinding.canSearch = false;
										this.Pathfinding.canMove = false;
										this.CanTalk = false;
										this.Routine = false;
										if (!this.Confessing)
										{
											this.ShoeRemoval.enabled = true;
											this.ShoeRemoval.LeavingSchool();
										}
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.GradePapers)
							{
								this.CharacterAnimation.CrossFade("f02_deskWrite");
								this.GradingPaper.Writing = true;
								this.Obstacle.enabled = true;
								this.Pen.SetActive(true);
							}
							else if (this.Actions[this.Phase] == StudentActionType.Patrol)
							{
								if (this.PatrolAnim == "")
								{
									this.PatrolAnim = this.IdleAnim;
								}
								if (this.StudentID == 1 && this.ExtraBento && this.CurrentDestination == this.StudentManager.Patrols.List[this.StudentID].GetChild(0))
								{
									this.StudentManager.MondayBento.SetActive(true);
									this.ExtraBento = false;
									if (this.Infatuated)
									{
										Debug.Log("Senpai is now changing his routine to go stalk the gravure idol.");
										this.StudentManager.StalkerID = 0;
										this.StudentManager.FollowGravureIdol(1);
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
									}
									else if (this.StudentManager.CustomMode)
									{
										Debug.Log("Attempting to update Senpai's routine to whatever his Custom Mode routine is supposed to be.");
										ScheduleBlock scheduleBlock12 = this.ScheduleBlocks[this.Phase];
										ScheduleBlock scheduleBlock13 = this.OriginalScheduleBlocks[this.Phase];
										scheduleBlock12.destination = scheduleBlock13.destination;
										scheduleBlock12.action = scheduleBlock13.action;
										scheduleBlock12.destination = this.ReturnDestination;
										scheduleBlock12.action = this.ReturnAction;
										this.GetDestinations();
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
									}
								}
								this.PatrolTimer += Time.deltaTime * this.CharacterAnimation[this.PatrolAnim].speed;
								if (this.StudentManager.Eighties && this.StudentID == 13)
								{
									if (this.PatrolID == 0)
									{
										this.PatrolAnim = this.BookReadAnim;
										this.OccultBook.SetActive(true);
									}
									else
									{
										this.PatrolAnim = this.ThinkAnim;
									}
								}
								if (this.StudentID == 1)
								{
									if (this.PatrolID == 0 && Vector3.Distance(base.transform.position, this.StudentManager.Gift.transform.position) < 1f)
									{
										if (this.StudentManager.Gift.activeInHierarchy || this.StudentManager.Note.activeInHierarchy)
										{
											this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.CharacterAnimation.CrossFade(this.InspectBloodAnim);
											if (this.CharacterAnimation[this.InspectBloodAnim].time >= this.CharacterAnimation[this.InspectBloodAnim].length)
											{
												this.StudentManager.SenpaiLoveWindow.UpdateSenpaiLove();
												this.StudentManager.Gift.SetActive(false);
												this.StudentManager.Note.SetActive(false);
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.PatrolAnim);
										}
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.PatrolAnim);
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.PatrolAnim);
								}
								if (this.PatrolTimer >= this.CharacterAnimation[this.PatrolAnim].length || this.Pathfinding.target == null)
								{
									this.PatrolID++;
									if (this.PatrolID == this.StudentManager.Patrols.List[this.StudentID].childCount || this.Pathfinding.target == null)
									{
										this.PatrolID = 0;
									}
									this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
									this.Pathfinding.target = this.CurrentDestination;
									this.BountyCollider.SetActive(false);
									this.OccultBook.SetActive(false);
									this.PatrolTimer = 0f;
								}
								if (this.Restless)
								{
									this.SewTimer += Time.deltaTime;
									if (this.SewTimer > 20f)
									{
										this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
										ScheduleBlock scheduleBlock14 = this.ScheduleBlocks[this.Phase];
										scheduleBlock14.destination = "Sketch";
										scheduleBlock14.action = "Sketch";
										this.GetDestinations();
										this.CurrentDestination = this.SketchPosition;
										this.Pathfinding.target = this.SketchPosition;
										this.SewTimer = 0f;
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Read)
							{
								if (this.ReadPhase == 0)
								{
									this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
									this.CharacterAnimation.CrossFade(this.BookSitAnim);
									if (this.CharacterAnimation[this.BookSitAnim].time > this.CharacterAnimation[this.BookSitAnim].length)
									{
										this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
										this.CharacterAnimation.CrossFade(this.BookReadAnim);
										this.ReadPhase++;
									}
									else if (this.CharacterAnimation[this.BookSitAnim].time > 1f)
									{
										this.SmartPhone.SetActive(false);
										this.OccultBook.SetActive(true);
									}
								}
								if (this.Rival)
								{
									this.ReadTimer += Time.deltaTime;
									if (this.ReadTimer > 300f)
									{
										this.OccultBook.SetActive(false);
										ScheduleBlock scheduleBlock15 = this.ScheduleBlocks[this.Phase];
										scheduleBlock15.destination = "LunchSpot";
										scheduleBlock15.action = "SitAndEatBento";
										this.Actions[this.Phase] = StudentActionType.SitAndEatBento;
										this.CurrentAction = StudentActionType.SitAndEatBento;
										this.GetDestinations();
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Texting)
							{
								this.CharacterAnimation.CrossFade("f02_midoriTexting_00");
								if (this.SmartPhone.transform.localPosition.x != 0.02f)
								{
									this.SmartPhone.transform.localPosition = new Vector3(0.02f, -0.0075f, 0f);
									this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, -164f);
								}
								if (!this.SmartPhone.activeInHierarchy && base.transform.position.y > 11f)
								{
									this.SmartPhone.SetActive(true);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Mourn)
							{
								this.CharacterAnimation.CrossFade("f02_brokenSit_00");
							}
							else if (this.Actions[this.Phase] == StudentActionType.Cuddle)
							{
								if (Vector3.Distance(base.transform.position, this.Partner.transform.position) < 1f && this.Partner.Routine)
								{
									ParticleSystem.EmissionModule emission = this.Hearts.emission;
									if (!emission.enabled)
									{
										this.Hearts.Play();
										emission.enabled = true;
										if (!this.Male)
										{
											this.Cosmetic.MyRenderer.materials[2].SetFloat("_BlendAmount", 1f);
										}
										else
										{
											this.Cosmetic.DetermineMaleFaceID();
											this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].SetFloat("_BlendAmount", 1f);
											this.SnackTimer += Time.deltaTime;
											if (this.SnackTimer > 1f)
											{
												this.SnackTimer = 0f;
												if (this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].mainTexture == this.Cosmetic.FaceTexture)
												{
													this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].mainTexture = this.BloodTexture;
												}
												else
												{
													this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].mainTexture = this.Cosmetic.FaceTexture;
												}
											}
										}
									}
									if (this.CuddlePartnerID == 1)
									{
										if (this.Male)
										{
											this.CharacterAnimation.CrossFade(this.CuddleAnim);
										}
										else
										{
											this.CharacterAnimation.CrossFade("f02_cuddle_00");
										}
									}
									else if (!this.Male)
									{
										this.CharacterAnimation.CrossFade(this.CuddleAnim);
									}
									else
									{
										this.CharacterAnimation.CrossFade("cuddle_00");
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Teaching)
							{
								if (this.Clock.Period != 2 && this.Clock.Period != 4)
								{
									this.CharacterAnimation.CrossFade("f02_teacherPodium_00");
								}
								else
								{
									if (!this.SpeechLines.isPlaying)
									{
										this.SpeechLines.Play();
									}
									this.CharacterAnimation.CrossFade(this.TeachAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.SearchPatrol)
							{
								if (this.PatrolID == 0 && this.StudentManager.CommunalLocker.RivalPhone.gameObject.activeInHierarchy && !this.EndSearch)
								{
									if (this.Rival)
									{
										this.LewdPhotos = this.StudentManager.CommunalLocker.RivalPhone.LewdPhotos;
										if (DateGlobals.Weekday == DayOfWeek.Monday)
										{
											SchemeGlobals.SetSchemeStage(1, 8);
											this.Yandere.PauseScreen.Schemes.UpdateInstructions();
										}
									}
									this.CharacterAnimation.CrossFade(this.DiscoverPhoneAnim);
									this.Subtitle.UpdateLabel(this.LostPhoneSubtitleType, 2, 5f);
									Debug.Log(this.Name + " found her lost phone from this spot in the code. 2");
									this.SearchingForPhone = false;
									this.Phoneless = false;
									this.EndSearch = true;
									this.Routine = false;
									this.PatrolTimer = 0f;
									if (this.EventToSabotage != null)
									{
										this.EventToSabotage.gameObject.SetActive(true);
									}
								}
								if (!this.EndSearch)
								{
									this.PatrolTimer += Time.deltaTime * this.CharacterAnimation[this.SearchPatrolAnim].speed;
									this.CharacterAnimation.CrossFade(this.SearchPatrolAnim);
									if (this.PatrolTimer >= this.CharacterAnimation[this.SearchPatrolAnim].length)
									{
										this.PatrolID++;
										if (this.PatrolID == this.StudentManager.SearchPatrols.List[this.Class].childCount)
										{
											this.PatrolID = 0;
										}
										this.CurrentDestination = this.StudentManager.SearchPatrols.List[this.Class].GetChild(this.PatrolID);
										this.Pathfinding.target = this.CurrentDestination;
										this.DistanceToDestination = 100f;
										this.PatrolTimer = 0f;
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Wait)
							{
								this.CharacterAnimation.CrossFade(this.IdleAnim);
							}
							else if (this.Actions[this.Phase] == StudentActionType.LightCig)
							{
								if (!this.Cigarette.active)
								{
									if (!this.Male)
									{
										this.WaitAnim = "f02_smokeAttempt_00";
									}
									else
									{
										this.WaitAnim = "smokeAttempt_00";
									}
									this.SmartPhone.SetActive(false);
									this.Cigarette.SetActive(true);
									this.Lighter.SetActive(true);
								}
								this.CharacterAnimation.CrossFade(this.WaitAnim);
							}
							else if (this.Actions[this.Phase] == StudentActionType.Random)
							{
								this.CurrentDestination.transform.position = this.StudentManager.PossibleRandomSpots[UnityEngine.Random.Range(1, 11)].position;
							}
							else if (this.Actions[this.Phase] == StudentActionType.Clean)
							{
								if (this.CleanTimer == 0f)
								{
									this.EquipCleaningItems();
								}
								this.CleanTimer += Time.deltaTime;
								if (this.CleaningRole == 5)
								{
									if (this.CleanID == 0)
									{
										this.CharacterAnimation.CrossFade(this.CleanAnims[1]);
									}
									else
									{
										if (!this.StudentManager.RoofFenceUp)
										{
											this.Pushable = true;
											this.StudentManager.UpdateMe(this.StudentID);
										}
										this.CharacterAnimation.CrossFade(this.CleanAnims[this.CleaningRole]);
										if ((double)this.CleanTimer >= 1.166666 && (double)this.CleanTimer <= 6.166666 && !this.ChalkDust.isPlaying)
										{
											this.ChalkDust.Play();
										}
									}
								}
								else if (this.CleaningRole == 4)
								{
									this.CharacterAnimation.CrossFade(this.CleanAnims[this.CleaningRole]);
									if (!this.Drownable)
									{
										this.Drownable = true;
										this.StudentManager.UpdateMe(this.StudentID);
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.CleanAnims[this.CleaningRole]);
								}
								if (this.CleanTimer >= this.CharacterAnimation[this.CleanAnims[this.CleaningRole]].length)
								{
									this.CleanID++;
									if (this.CleanID == this.CleaningSpot.childCount)
									{
										this.CleanID = 0;
									}
									this.CurrentDestination = this.CleaningSpot.GetChild(this.CleanID);
									this.Pathfinding.target = this.CurrentDestination;
									this.DistanceToDestination = 100f;
									this.CleanTimer = 0f;
									if (this.Pushable)
									{
										this.Prompt.Label[0].text = "     Talk";
										this.Pushable = false;
										this.StudentManager.UpdateMe(this.StudentID);
									}
									if (this.Drownable)
									{
										this.Drownable = false;
										this.StudentManager.UpdateMe(this.StudentID);
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Graffiti)
							{
								if (this.KilledMood)
								{
									this.Subtitle.UpdateLabel(SubtitleType.KilledMood, 0, 5f);
									this.GraffitiPhase = 4;
									this.KilledMood = false;
								}
								if (this.GraffitiPhase == 0)
								{
									AudioSource.PlayClipAtPoint(this.BullyGiggles[UnityEngine.Random.Range(0, this.BullyGiggles.Length)], this.Head.position);
									this.CharacterAnimation.CrossFade("f02_bullyDesk_00");
									this.SmartPhone.SetActive(false);
									this.GraffitiPhase++;
								}
								else if (this.GraffitiPhase == 1)
								{
									if (this.CharacterAnimation["f02_bullyDesk_00"].time >= 8f)
									{
										this.StudentManager.Graffiti[this.BullyID].SetActive(true);
										this.GraffitiPhase++;
									}
								}
								else if (this.GraffitiPhase == 2)
								{
									if (this.CharacterAnimation["f02_bullyDesk_00"].time >= 9.66666f)
									{
										AudioSource.PlayClipAtPoint(this.BullyGiggles[UnityEngine.Random.Range(0, this.BullyGiggles.Length)], this.Head.position);
										this.GraffitiPhase++;
									}
								}
								else if (this.GraffitiPhase == 3)
								{
									if (this.CharacterAnimation["f02_bullyDesk_00"].time >= this.CharacterAnimation["f02_bullyDesk_00"].length)
									{
										this.GraffitiPhase++;
									}
								}
								else if (this.GraffitiPhase == 4)
								{
									this.DistanceToDestination = 100f;
									ScheduleBlock scheduleBlock16 = this.ScheduleBlocks[2];
									scheduleBlock16.destination = "Patrol";
									scheduleBlock16.action = "Patrol";
									this.GetDestinations();
									this.CurrentDestination = this.Destinations[this.Phase];
									this.Pathfinding.target = this.Destinations[this.Phase];
									this.SmartPhone.SetActive(true);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Bully)
							{
								this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
								if (this.StudentManager.Students[this.StudentManager.VictimID] != null && this.StudentManager.Students[this.StudentManager.VictimID].Alive && !this.StudentManager.Students[this.StudentManager.VictimID].Tranquil)
								{
									if (this.StudentManager.Students[this.StudentManager.VictimID].Distracted)
									{
										this.StudentManager.NoBully[this.BullyID] = true;
										this.KilledMood = true;
									}
									if (this.KilledMood)
									{
										this.Subtitle.UpdateLabel(SubtitleType.KilledMood, 0, 5f);
										this.BullyPhase = 4;
										this.KilledMood = false;
										this.BullyDust.Stop();
									}
									if (this.StudentManager.Students[81] == null)
									{
										ScheduleBlock scheduleBlock17 = this.ScheduleBlocks[4];
										scheduleBlock17.destination = "Patrol";
										scheduleBlock17.action = "Patrol";
										this.GetDestinations();
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
									}
									else
									{
										this.SmartPhone.SetActive(false);
										if (this.BullyID == 1)
										{
											if (this.BullyPhase == 0)
											{
												this.BullyTimer += Time.deltaTime;
												if (this.BullyTimer >= 10f && Vector3.Distance(base.transform.position, this.StudentManager.Students[1].transform.position) > 10f)
												{
													this.Scrubber.GetComponent<Renderer>().material.mainTexture = this.Eraser.GetComponent<Renderer>().material.mainTexture;
													this.Scrubber.SetActive(true);
													this.Eraser.SetActive(true);
													this.StudentManager.Students[this.StudentManager.VictimID].CharacterAnimation.CrossFade(this.StudentManager.Students[this.StudentManager.VictimID].BullyVictimAnim);
													this.StudentManager.Students[this.StudentManager.VictimID].Routine = false;
													this.CharacterAnimation.CrossFade("f02_bullyEraser_00");
													this.BullyPhase++;
												}
												else
												{
													if (this.GiggleTimer == 0f)
													{
														AudioSource.PlayClipAtPoint(this.BullyGiggles[UnityEngine.Random.Range(0, this.BullyGiggles.Length)], this.Head.position);
														this.GiggleTimer = 5f;
													}
													this.GiggleTimer = Mathf.MoveTowards(this.GiggleTimer, 0f, Time.deltaTime);
													this.CharacterAnimation.CrossFade("f02_bullyGiggle_00");
												}
											}
											else if (this.BullyPhase == 1)
											{
												if (this.CharacterAnimation["f02_bullyEraser_00"].time >= 0.833333f)
												{
													this.BullyDust.Play();
													this.BullyPhase++;
												}
											}
											else if (this.BullyPhase == 2)
											{
												if (this.CharacterAnimation["f02_bullyEraser_00"].time >= this.CharacterAnimation["f02_bullyEraser_00"].length)
												{
													AudioSource.PlayClipAtPoint(this.BullyLaughs[this.BullyID], this.Head.position);
													this.CharacterAnimation.CrossFade("f02_bullyLaugh_00");
													this.Scrubber.SetActive(false);
													this.Eraser.SetActive(false);
													this.BullyPhase++;
												}
											}
											else if (this.BullyPhase == 3)
											{
												if (this.CharacterAnimation["f02_bullyLaugh_00"].time >= this.CharacterAnimation["f02_bullyLaugh_00"].length)
												{
													this.BullyPhase++;
												}
											}
											else if (this.BullyPhase == 4)
											{
												this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
												this.StudentManager.Students[this.StudentManager.VictimID].Routine = true;
												ScheduleBlock scheduleBlock18 = this.ScheduleBlocks[4];
												scheduleBlock18.destination = "Patrol";
												scheduleBlock18.action = "Patrol";
												this.GetDestinations();
												if (!this.StudentManager.Eighties)
												{
													if (this.StudentID == 82)
													{
														this.StudentManager.UpdateLunchtimeHangout(82);
													}
													else if (this.StudentID == 83)
													{
														this.StudentManager.UpdateLunchtimeHangout(83);
													}
												}
												this.CurrentDestination = this.Destinations[this.Phase];
												this.Pathfinding.target = this.Destinations[this.Phase];
												this.SmartPhone.SetActive(true);
												this.Scrubber.SetActive(false);
												this.Eraser.SetActive(false);
											}
										}
										else
										{
											if (this.StudentManager.Students[81].BullyPhase < 2)
											{
												if (this.GiggleTimer == 0f)
												{
													AudioSource.PlayClipAtPoint(this.BullyGiggles[UnityEngine.Random.Range(0, this.BullyGiggles.Length)], this.Head.position);
													this.GiggleTimer = 5f;
												}
												this.GiggleTimer = Mathf.MoveTowards(this.GiggleTimer, 0f, Time.deltaTime);
												this.CharacterAnimation.CrossFade("f02_bullyGiggle_00");
											}
											else if (this.StudentManager.Students[81].BullyPhase < 4)
											{
												if (this.LaughTimer == 0f)
												{
													AudioSource.PlayClipAtPoint(this.BullyLaughs[this.BullyID], this.Head.position);
													this.LaughTimer = 5f;
												}
												this.LaughTimer = Mathf.MoveTowards(this.LaughTimer, 0f, Time.deltaTime);
												this.CharacterAnimation.CrossFade("f02_bullyLaugh_00");
											}
											if (this.CharacterAnimation["f02_bullyLaugh_00"].time >= this.CharacterAnimation["f02_bullyLaugh_00"].length || this.StudentManager.Students[81].BullyPhase == 4 || this.BullyPhase == 4)
											{
												Debug.Log("The bullying event has ended.");
												this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
												this.DistanceToDestination = 100f;
												ScheduleBlock scheduleBlock19 = this.ScheduleBlocks[4];
												scheduleBlock19.destination = "Patrol";
												scheduleBlock19.action = "Patrol";
												this.GetDestinations();
												this.CurrentDestination = this.Destinations[this.Phase];
												this.Pathfinding.target = this.Destinations[this.Phase];
												this.SmartPhone.SetActive(true);
											}
										}
									}
								}
								else
								{
									Debug.Log("This code is called when the bullies' victim is missing or dead.");
									this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
									this.DistanceToDestination = 100f;
									ScheduleBlock scheduleBlock20 = this.ScheduleBlocks[this.Phase];
									scheduleBlock20.destination = "Patrol";
									scheduleBlock20.action = "Patrol";
									this.GetDestinations();
									this.CurrentDestination = this.Destinations[this.Phase];
									this.Pathfinding.target = this.Destinations[this.Phase];
									this.SmartPhone.SetActive(true);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Follow)
							{
								Debug.Log("Raibaru is currently following Osana, and believes that she has reached her destination.");
								if (this.FollowTarget != null)
								{
									if (this.StudentManager.LoveManager.RivalWaiting && this.FollowTarget.transform.position.x > 40f && this.FollowTarget.DistanceToDestination < 1f)
									{
										this.CurrentDestination = this.StudentManager.LoveManager.FriendWaitSpot;
										this.Pathfinding.target = this.StudentManager.LoveManager.FriendWaitSpot;
										this.CharacterAnimation.CrossFade(this.IdleAnim);
									}
									else if (this.FollowTarget.Routine && !this.FollowTarget.InEvent && this.FollowTarget.CurrentAction == StudentActionType.Clean && this.FollowTarget.DistanceToDestination < 1f)
									{
										this.FollowTarget.FollowTargetDestination.localPosition = new Vector3(-1f, 0f, -1f);
										this.CurrentDestination = this.FollowTarget.FollowTargetDestination;
										this.Pathfinding.target = this.FollowTarget.FollowTargetDestination;
										this.CharacterAnimation.CrossFade(this.CleanAnims[this.CleaningRole]);
										this.Scrubber.SetActive(true);
										if (this.CharacterAnimation[this.CleanAnims[this.CleaningRole]].time > this.CharacterAnimation[this.CleanAnims[this.CleaningRole]].length)
										{
											this.CharacterAnimation[this.CleanAnims[this.CleaningRole]].time = 1f;
										}
									}
									else if (this.FollowTarget.Routine && !this.FollowTarget.InEvent && !this.FollowTarget.Meeting && this.FollowTarget.gameObject.activeInHierarchy && this.FollowTarget.CurrentAction == StudentActionType.Socializing && this.FollowTarget.DistanceToDestination < 1f)
									{
										if (this.FollowTarget.Alone || this.FollowTarget.Meeting)
										{
											this.CharacterAnimation.CrossFade(this.IdleAnim);
											this.SpeechLines.Stop();
										}
										else
										{
											this.Scrubber.SetActive(false);
											this.SpeechLines.Play();
											this.CharacterAnimation.CrossFade(this.RandomAnim);
											if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
											{
												this.PickRandomAnim();
											}
										}
									}
									else if (this.FollowTarget.CurrentAction == StudentActionType.SitAndTakeNotes && this.FollowTarget.Routine && !this.FollowTarget.InEvent && this.FollowTarget.DistanceToDestination < 1f && !this.FollowTarget.Meeting && this.Clock.HourTime < 15.5f)
									{
										Debug.Log("Raibaru just changed her destination to class.");
										this.GoToClass();
									}
									else if (this.FollowTarget.Routine && !this.FollowTarget.InEvent && this.FollowTarget.CurrentAction == StudentActionType.SitAndEatBento && this.FollowTarget.DistanceToDestination < 1f && !this.FollowTarget.Meeting)
									{
										Debug.Log("Raibaru just changed her destination to lunch.");
										ScheduleBlock scheduleBlock21 = this.ScheduleBlocks[this.Phase];
										scheduleBlock21.destination = "LunchSpot";
										scheduleBlock21.action = "SitAndEatBento";
										this.Actions[this.Phase] = StudentActionType.SitAndEatBento;
										this.CurrentAction = StudentActionType.SitAndEatBento;
										this.GetDestinations();
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
									}
									else if (this.FollowTarget.Routine && !this.FollowTarget.InEvent && this.FollowTarget.Phase == 8 && this.FollowTarget.DistanceToDestination < 1f)
									{
										Debug.Log("Raibaru just changed her destination to the lockers.");
										ScheduleBlock scheduleBlock22 = this.ScheduleBlocks[this.Phase];
										scheduleBlock22.destination = "Locker";
										scheduleBlock22.action = "Shoes";
										this.Actions[this.Phase] = StudentActionType.ChangeShoes;
										this.CurrentAction = StudentActionType.ChangeShoes;
										this.GetDestinations();
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
									}
									else if (this.FollowTarget.ConfessPhase == 5)
									{
										Debug.Log("Raibaru just changed her action to Sketch and her destination to Paint.");
										ScheduleBlock scheduleBlock23 = this.ScheduleBlocks[this.Phase];
										scheduleBlock23.destination = "Paint";
										scheduleBlock23.action = "Sketch";
										scheduleBlock23.time = 99f;
										this.GetDestinations();
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.Destinations[this.Phase];
										this.TargetDistance = 1f;
										this.MyController.radius = 0.1f;
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.IdleAnim);
										this.SpeechLines.Stop();
										if (this.SlideIn)
										{
											this.MoveTowardsTarget(this.CurrentDestination.position);
										}
										if (!this.FollowTarget.gameObject.activeInHierarchy || !this.FollowTarget.enabled)
										{
											if (base.transform.position.y > -1f)
											{
												Debug.Log("Raibaru cannot find Osana.");
												this.RaibaruCannotFindOsana();
											}
											else
											{
												Debug.Log("Osana left school, so Raibaru is disabling herself, too.");
												base.gameObject.SetActive(false);
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.IdleAnim);
										}
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Sulk)
							{
								if (this.StudentID == 51)
								{
									this.CharacterAnimation.CrossFade("f02_railingSulk_0" + this.SulkPhase.ToString(), 1f);
									this.SulkTimer += Time.deltaTime;
									if (this.SulkTimer > 7.66666f)
									{
										this.SulkTimer = 0f;
										this.SulkPhase++;
										if (this.SulkPhase == 3)
										{
											this.SulkPhase = 0;
										}
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.SulkAnim);
									if (this.StudentID == 76 && this.Clock.GameplayDay == 10)
									{
										this.BountyCollider.SetActive(true);
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Sleuth)
							{
								if (this.StudentManager.SleuthPhase != 3)
								{
									this.StudentManager.ConvoManager.CheckMe(this.StudentID);
									if (this.Alone)
									{
										if (this.Male)
										{
											this.CharacterAnimation.CrossFade("standTexting_00");
										}
										else
										{
											this.CharacterAnimation.CrossFade("f02_standTexting_00");
										}
										if (this.Male)
										{
											this.SmartPhone.transform.localPosition = new Vector3(0.025f, 0.0025f, 0.025f);
											this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, 180f);
										}
										else
										{
											this.SmartPhone.transform.localPosition = new Vector3(0.01f, 0.01f, 0.01f);
											this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -160f, 165f);
										}
										this.SmartPhone.SetActive(true);
										this.SpeechLines.Stop();
									}
									else
									{
										if (!this.SpeechLines.isPlaying)
										{
											this.SmartPhone.SetActive(false);
											this.SpeechLines.Play();
										}
										this.CharacterAnimation.CrossFade(this.RandomSleuthAnim, 1f);
										if (this.CharacterAnimation[this.RandomSleuthAnim].time >= this.CharacterAnimation[this.RandomSleuthAnim].length)
										{
											this.PickRandomSleuthAnim();
										}
										this.StudentManager.SleuthTimer += Time.deltaTime;
										if (this.StudentManager.SleuthTimer > 100f)
										{
											this.StudentManager.SleuthTimer = 0f;
											this.StudentManager.UpdateSleuths();
										}
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.SleuthScanAnim);
									if (this.CharacterAnimation[this.SleuthScanAnim].time >= this.CharacterAnimation[this.SleuthScanAnim].length)
									{
										this.GetSleuthTarget();
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Stalk)
							{
								this.CharacterAnimation.CrossFade(this.SleuthIdleAnim);
								if (this.DistanceToPlayer < 5f || this.StudentManager.LockerRoomArea.bounds.Contains(this.Yandere.transform.position))
								{
									if (Vector3.Distance(base.transform.position, this.StudentManager.FleeSpots[0].position) > 10f)
									{
										this.Pathfinding.target = this.StudentManager.FleeSpots[0];
										this.CurrentDestination = this.StudentManager.FleeSpots[0];
									}
									else
									{
										this.Pathfinding.target = this.StudentManager.FleeSpots[1];
										this.CurrentDestination = this.StudentManager.FleeSpots[1];
									}
									this.Pathfinding.speed = 4f;
									this.StalkerFleeing = true;
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Sketch)
							{
								this.CharacterAnimation.CrossFade(this.SketchAnim);
								this.Sketchbook.SetActive(true);
								this.Pencil.SetActive(true);
								if (this.Restless)
								{
									this.SewTimer += Time.deltaTime;
									if (this.SewTimer > 20f)
									{
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										ScheduleBlock scheduleBlock24 = this.ScheduleBlocks[this.Phase];
										scheduleBlock24.destination = "Patrol";
										scheduleBlock24.action = "Patrol";
										this.GetDestinations();
										this.EmptyHands();
										this.PatrolID = 1;
										this.PatrolTimer = 0f;
										this.CharacterAnimation[this.PatrolAnim].time = 0f;
										this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
										this.Pathfinding.target = this.CurrentDestination;
										this.SewTimer = 0f;
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Sunbathe)
							{
								bool flag4 = false;
								if (this.FollowTarget != null)
								{
									Debug.Log(this.Name + " is sunbathing.");
									if (!this.FollowTarget.Alive && this.FollowTarget.Ragdoll.StopAnimation)
									{
										this.RaibaruCannotFindOsana();
										flag4 = true;
									}
								}
								if (!flag4)
								{
									if (this.SunbathePhase == 0)
									{
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										this.StudentManager.CommunalLocker.Open = true;
										this.StudentManager.CommunalLocker.SpawnSteamNoSideEffects(this);
										this.MustChangeClothing = true;
										this.ChangeClothingPhase++;
										this.SunbathePhase++;
									}
									else if (this.SunbathePhase == 1)
									{
										this.CharacterAnimation.CrossFade(this.StripAnim);
										this.Pathfinding.canSearch = false;
										this.Pathfinding.canMove = false;
										if (this.CharacterAnimation[this.StripAnim].time >= 1.5f)
										{
											if (this.Schoolwear != 2)
											{
												Debug.Log(this.Name + " is calling ChangeSchoolwear() from here, specifically.");
												this.Schoolwear = 2;
												this.ChangeSchoolwear();
											}
											if (this.CharacterAnimation[this.StripAnim].time > this.CharacterAnimation[this.StripAnim].length)
											{
												this.Pathfinding.canSearch = true;
												this.Pathfinding.canMove = true;
												this.Stripping = false;
												if (!this.StudentManager.CommunalLocker.SteamCountdown)
												{
													this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
													this.Destinations[this.Phase] = this.StudentManager.SunbatheSpots[this.StudentID];
													this.Pathfinding.target = this.StudentManager.SunbatheSpots[this.StudentID];
													this.CurrentDestination = this.StudentManager.SunbatheSpots[this.StudentID];
													this.StudentManager.CommunalLocker.Student = null;
													this.SunbathePhase++;
												}
											}
										}
									}
									else if (this.SunbathePhase == 2)
									{
										if (this.Rival && this.StudentManager.PoolClosed)
										{
											this.Subtitle.CustomText = "I can't believe anyone would let a stupid sign stop them from sunbathing...";
											this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
										}
										this.MyRenderer.updateWhenOffscreen = true;
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										this.CharacterAnimation.CrossFade("f02_sunbatheStart_00");
										this.SmartPhone.SetActive(false);
										if (this.CharacterAnimation["f02_sunbatheStart_00"].time >= this.CharacterAnimation["f02_sunbatheStart_00"].length)
										{
											this.MyController.radius = 0f;
											this.SunbathePhase++;
										}
									}
									else if (this.SunbathePhase == 3)
									{
										if (this.Sleepy)
										{
											if (!this.Blind)
											{
												Debug.Log("Aaaaand...NOW! Rival is now asleep.");
												this.CharacterAnimation.CrossFade("f02_sunbatheSleep_00");
												this.Ragdoll.Zs.SetActive(true);
												this.Blind = true;
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade("f02_sunbatheLoop_00");
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Shock)
							{
								if (this.StudentManager.Students[36] == null)
								{
									this.Phase++;
								}
								else if (this.StudentManager.Students[36].Routine && this.StudentManager.Students[36].DistanceToDestination < 1f)
								{
									if (!this.StudentManager.GamingDoor.Locked && !this.StudentManager.GamingDoor.Open)
									{
										this.StudentManager.GamingDoor.OpenDoor();
									}
									ParticleSystem.EmissionModule emission2 = this.Hearts.emission;
									if (this.SmartPhone.activeInHierarchy)
									{
										this.Cosmetic.MyRenderer.materials[2].SetFloat("_BlendAmount", 1f);
										this.SmartPhone.SetActive(false);
										this.MyController.radius = 0f;
										emission2.rateOverTime = 5f;
										emission2.enabled = true;
										this.Hearts.Play();
									}
									if (!this.StudentManager.GamingDoor.Locked)
									{
										this.CharacterAnimation.CrossFade("f02_peeking_0" + (this.StudentID - 80).ToString());
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.AdmireAnim);
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.PatrolAnim);
									if (!this.SmartPhone.activeInHierarchy)
									{
										this.SmartPhone.SetActive(true);
										this.MyController.radius = 0.1f;
										if (this.BullyID == 2)
										{
											this.MyController.Move(base.transform.right * 1f * Time.timeScale * 0.2f);
										}
										else if (this.BullyID == 3)
										{
											this.MyController.Move(base.transform.right * -1f * Time.timeScale * 0.2f);
										}
										else if (this.BullyID == 4)
										{
											this.MyController.Move(base.transform.right * 1f * Time.timeScale * 0.2f);
										}
										else if (this.BullyID == 5)
										{
											this.MyController.Move(base.transform.right * -1f * Time.timeScale * 0.2f);
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Miyuki)
							{
								if (this.StudentManager.MiyukiEnemy.Enemy.activeInHierarchy)
								{
									if (this.StudentID == 37 && this.Clock.GameplayDay == 6)
									{
										this.BountyCollider.SetActive(true);
									}
									this.CharacterAnimation.CrossFade(this.MiyukiAnim);
									this.MiyukiTimer += Time.deltaTime;
									if (this.MiyukiTimer > 1f)
									{
										this.MiyukiTimer = 0f;
										this.Miyuki.Shoot();
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.VictoryAnim);
									this.BountyCollider.SetActive(false);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Meeting)
							{
								if (this.StudentID != 36)
								{
									this.StudentManager.Meeting = true;
									if (this.StudentManager.Speaker == this.StudentID)
									{
										if (!this.SpeechLines.isPlaying)
										{
											this.CharacterAnimation.CrossFade(this.RandomAnim);
											this.SpeechLines.Play();
										}
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.IdleAnim);
										if (this.SpeechLines.isPlaying)
										{
											this.SpeechLines.Stop();
										}
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.PeekAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Lyrics)
							{
								this.LyricsTimer += Time.deltaTime;
								if (this.LyricsPhase == 0)
								{
									this.CharacterAnimation.CrossFade("f02_writingLyrics_00");
									if (!this.Pencil.activeInHierarchy)
									{
										this.Pencil.SetActive(true);
									}
									if (this.LyricsTimer > 18f)
									{
										this.StudentManager.LyricsSpot.position = this.StudentManager.AirGuitarSpot.position;
										this.StudentManager.LyricsSpot.eulerAngles = this.StudentManager.AirGuitarSpot.eulerAngles;
										this.Pencil.SetActive(false);
										this.LyricsPhase = 1;
										this.LyricsTimer = 0f;
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade("f02_airGuitar_00");
									if (!this.AirGuitar.isPlaying)
									{
										this.AirGuitar.Play();
									}
									if (this.LyricsTimer > 18f)
									{
										this.StudentManager.LyricsSpot.position = this.StudentManager.OriginalLyricsSpot.position;
										this.StudentManager.LyricsSpot.eulerAngles = this.StudentManager.OriginalLyricsSpot.eulerAngles;
										this.AirGuitar.Stop();
										this.LyricsPhase = 0;
										this.LyricsTimer = 0f;
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Sew)
							{
								this.CharacterAnimation.CrossFade("sewing_00");
								this.PinkSeifuku.SetActive(true);
								if (this.SewTimer < 10f && this.StudentManager.TaskManager.TaskStatus[8] == 3)
								{
									this.SewTimer += Time.deltaTime;
									if (this.SewTimer > 10f)
									{
										UnityEngine.Object.Instantiate<GameObject>(this.Yandere.PauseScreen.DropsMenu.GetComponent<DropsScript>().InfoChanWindow.Drops[1], new Vector3(28.289f, 0.7718928f, 5.196f), Quaternion.identity);
									}
								}
								if (this.StudentID == 8 && this.Clock.GameplayDay == 4 && !this.BountyCollider.activeInHierarchy)
								{
									this.BountyCollider.transform.localPosition = new Vector3(0f, 0f, 0f);
									this.BountyCollider.GetComponent<BoxCollider>().center = new Vector3(0f, 0.65f, 0.175f);
									this.BountyCollider.GetComponent<BoxCollider>().size = new Vector3(0.45f, 1.3f, 0.55f);
									this.BountyCollider.SetActive(true);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Paint)
							{
								this.Painting.material.color += new Color(0f, 0f, 0f, Time.deltaTime * 0.00066666f);
								this.CharacterAnimation.CrossFade(this.PaintAnim);
								this.Paintbrush.SetActive(true);
								this.Palette.SetActive(true);
							}
							else if (this.Actions[this.Phase] == StudentActionType.UpdateAppearance)
							{
								Debug.Log("We have reached the ''UpdateAppearance'' code.");
								this.UpdateGemaAppearance();
							}
							else if (this.Actions[this.Phase] == StudentActionType.PlaceBag)
							{
								this.PlaceBag();
							}
							else if (this.Actions[this.Phase] == StudentActionType.Sleep)
							{
								if (this.Male)
								{
									this.CharacterAnimation.CrossFade("infirmaryRest_00");
								}
								else
								{
									this.CharacterAnimation.CrossFade("f02_infirmaryRest_00");
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.LightFire)
							{
								if (this.PyroPhase == 1)
								{
									this.CharacterAnimation.CrossFade("f02_startFire_00");
									if (this.DistanceToPlayer < 5f && this.Yandere.transform.position.z < base.transform.position.z)
									{
										this.Subtitle.CustomText = "...oh...I didn't realize someone was here...I'll just...be going, now...";
										this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
										this.PyroPhase = 4;
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > this.CharacterAnimation["f02_startFire_00"].length)
									{
										this.PyroPhase++;
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > 13f)
									{
										if (!this.StudentManager.PyroFlames.isPlaying)
										{
											this.StudentManager.PyroFlames.Play();
											this.StudentManager.PyroFlameSounds[1].Play();
											this.StudentManager.PyroFlameSounds[2].Play();
										}
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > 11.75f)
									{
										this.Note.transform.parent = null;
										this.Note.transform.position -= new Vector3(0f, Time.deltaTime, 0f);
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > 11.5f)
									{
										this.Lighter.SetActive(false);
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > 8f)
									{
										this.PaperFire.SetActive(true);
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > 4.5f)
									{
										this.Lighter.SetActive(true);
									}
									else if (this.CharacterAnimation["f02_startFire_00"].time > 1f && !this.Note.gameObject.activeInHierarchy)
									{
										this.Note.transform.parent = this.RightHand;
										this.Note.transform.localPosition = new Vector3(0f, -0.1f, -0.004f);
										this.Note.transform.localEulerAngles = new Vector3(0f, 135f, 45f);
										this.Note.transform.localScale = new Vector3(0.1f, 0.2f, 1f);
										this.Note.SetActive(true);
									}
								}
								else if (this.PyroPhase == 2)
								{
									if (this.PyroTimer == 0f && this.DistanceToPlayer < 5f)
									{
										this.Subtitle.CustomText = "...hehe...it's always just as spellbinding as the first time...";
										this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
									}
									this.CharacterAnimation.CrossFade("f02_inspectLoop_00");
									this.PyroTimer += Time.deltaTime;
									if (this.PyroTimer > 60f || this.Yandere.transform.position.z < base.transform.position.z)
									{
										if (this.DistanceToPlayer < 5f)
										{
											if (this.PyroTimer < 60f)
											{
												this.Subtitle.UpdateLabel(SubtitleType.AcceptFood, 0, 0f);
												this.Subtitle.CustomText = "...um...oh, my! Who started this fire? How dangerous! I'd better put it out...";
												this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
											}
											else
											{
												this.Subtitle.CustomText = "...well, that's enough for now...";
												this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
											}
										}
										this.StudentManager.PyroWateringCan.parent = this.RightHand;
										this.StudentManager.PyroWateringCan.localPosition = new Vector3(0.14f, -0.15f, -0.05f);
										this.StudentManager.PyroWateringCan.localEulerAngles = new Vector3(10f, 15f, 45f);
										this.Note.transform.parent = this.RightHand;
										this.Note.transform.localPosition = new Vector3(0f, -0.1f, -0.004f);
										this.Note.transform.localEulerAngles = new Vector3(0f, 135f, 45f);
										this.Note.transform.localScale = new Vector3(0.1f, 0.2f, 1f);
										this.PaperFire.SetActive(false);
										this.Lighter.SetActive(false);
										this.Note.SetActive(false);
										this.PyroPhase++;
									}
								}
								else if (this.PyroPhase == 3)
								{
									this.CharacterAnimation.CrossFade("f02_waterPlant_00");
									if (this.CharacterAnimation["f02_waterPlant_00"].time > this.CharacterAnimation["f02_waterPlant_00"].length)
									{
										this.WillCombust = true;
										this.PyroPhase++;
									}
								}
								else if (this.PyroPhase == 4)
								{
									this.StudentManager.PyroWateringCan.parent = null;
									this.StudentManager.PyroWateringCan.localPosition = new Vector3(-38.5f, 0f, 43f);
									this.StudentManager.PyroWateringCan.localEulerAngles = new Vector3(0f, 180f, 0f);
									if (this.StudentManager.GasInWateringCan && this.WillCombust)
									{
										this.Combust();
									}
									else
									{
										this.FinishPyro();
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Jog)
							{
								this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
								if (this.Schoolwear == 1)
								{
									if (this.SunbathePhase == 0)
									{
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										this.StudentManager.CommunalLocker.Open = true;
										this.StudentManager.CommunalLocker.SpawnSteamNoSideEffects(this);
										this.MustChangeClothing = true;
										this.ChangeClothingPhase++;
										this.SunbathePhase++;
									}
									else if (this.SunbathePhase == 1)
									{
										this.CharacterAnimation.CrossFade(this.StripAnim);
										this.Pathfinding.canSearch = false;
										this.Pathfinding.canMove = false;
										if (this.CharacterAnimation[this.StripAnim].time >= 1.5f)
										{
											if (this.Schoolwear != 3)
											{
												this.Schoolwear = 3;
												this.ChangeSchoolwear();
											}
											if (this.CharacterAnimation[this.StripAnim].time > this.CharacterAnimation[this.StripAnim].length)
											{
												this.Pathfinding.canSearch = true;
												this.Pathfinding.canMove = true;
												this.Stripping = false;
												if (!this.StudentManager.CommunalLocker.SteamCountdown)
												{
													this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
													this.Destinations[this.Phase] = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
													this.Pathfinding.target = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
													this.CurrentDestination = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
													this.StudentManager.CommunalLocker.Student = null;
													this.SunbathePhase++;
												}
											}
										}
									}
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.ClubAnim);
									if (this.ClubActivityPhase == 0)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											string str4 = "";
											if (!this.Male)
											{
												str4 = "f02_";
											}
											this.ClubActivityPhase++;
											this.ClubAnim = str4 + "stretch_01";
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase == 1)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											string str5 = "";
											if (!this.Male)
											{
												str5 = "f02_";
											}
											this.ClubActivityPhase++;
											this.ClubAnim = str5 + "stretch_02";
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
										}
									}
									else if (this.ClubActivityPhase == 2)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											string str6 = "";
											if (!this.Male)
											{
												str6 = "f02_";
											}
											this.WalkAnim = str6 + "trackJog_00";
											this.Hurry = true;
											this.ClubActivityPhase++;
											this.CharacterAnimation[this.ClubAnim].time = 0f;
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
											this.CurrentDestination = this.Destinations[this.Phase];
											this.Pathfinding.target = this.CurrentDestination;
											this.Pathfinding.speed = 4f;
										}
									}
									else if (this.ClubActivityPhase < 14)
									{
										if (this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
										{
											string str7 = "";
											if (!this.Male)
											{
												str7 = "f02_";
											}
											this.WalkAnim = str7 + "trackJog_00";
											this.Hurry = true;
											this.ClubActivityPhase++;
											this.CharacterAnimation[this.ClubAnim].time = 0f;
											this.Destinations[this.Phase] = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
											this.CurrentDestination = this.Destinations[this.Phase];
											this.Pathfinding.target = this.CurrentDestination;
											this.Pathfinding.speed = 4f;
										}
									}
									else if (this.ClubActivityPhase == 14 && this.CharacterAnimation[this.ClubAnim].time >= this.CharacterAnimation[this.ClubAnim].length)
									{
										this.WalkAnim = this.OriginalWalkAnim;
										this.Hurry = false;
										this.ClubActivityPhase = 0;
										if (this.Male)
										{
											this.ClubAnim = "stretch_00";
										}
										else
										{
											this.ClubAnim = "f02_stretch_00";
										}
										this.Destinations[this.Phase] = this.StudentManager.Clubs.List[66].GetChild(this.ClubActivityPhase);
										this.CurrentDestination = this.Destinations[this.Phase];
										this.Pathfinding.target = this.CurrentDestination;
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.PrepareFood)
							{
								if (!this.MyBento.gameObject.activeInHierarchy)
								{
									this.MyBento.Lid.SetActive(false);
									this.MyBento.Prompt.enabled = true;
									this.MyBento.transform.parent = null;
									this.MyBento.gameObject.SetActive(true);
									this.MyBento.transform.position = this.StudentManager.FoodTrays[this.StudentID].position;
									this.MyBento.transform.eulerAngles = this.StudentManager.FoodTrays[this.StudentID].eulerAngles;
								}
								this.CharacterAnimation.CrossFade(this.PrepareFoodAnim);
								this.ClubTimer += Time.deltaTime;
								if (this.ClubTimer > 60f)
								{
									this.MyBento.transform.parent = this.LeftItemParent;
									this.MyBento.transform.localPosition = new Vector3(-0.025f, -0.105f, 0f);
									this.MyBento.transform.localEulerAngles = new Vector3(0f, 165f, 82.5f);
									this.MyBento.gameObject.SetActive(false);
									this.MyBento.Prompt.enabled = false;
									this.MyBento.Prompt.Hide();
									ScheduleBlock scheduleBlock25 = this.ScheduleBlocks[this.Phase];
									scheduleBlock25.destination = "LunchSpot";
									scheduleBlock25.action = "Eat";
									this.GetDestinations();
									this.Pathfinding.target = this.Destinations[this.Phase];
									this.CurrentDestination = this.Destinations[this.Phase];
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Perform)
							{
								this.PlayMusicalInstrument();
							}
							else if (this.Actions[this.Phase] == StudentActionType.PhotoShoot)
							{
								if (this.StudentManager.Students[19] != null)
								{
									if (this.StudentManager.Students[19].ClubTimer > 0f && this.StudentManager.Students[19].DistanceToDestination < 1f)
									{
										if (this.Male)
										{
											this.CharacterAnimation.CrossFade("tripodUse_00");
										}
										else
										{
											this.CharacterAnimation.CrossFade("f02_tripodUse_00");
										}
									}
									else if (this.Male)
									{
										this.CharacterAnimation.CrossFade("impatientWait_00");
									}
									else
									{
										this.CharacterAnimation.CrossFade("f02_impatientWait_00");
									}
								}
								else if (this.Male)
								{
									this.CharacterAnimation.CrossFade("impatientWait_00");
								}
								else
								{
									this.CharacterAnimation.CrossFade("f02_impatientWait_00");
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.GravurePose)
							{
								if (!this.Hurry)
								{
									if (this.SunbathePhase < 2)
									{
										if (this.SunbathePhase == 0)
										{
											this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
											this.StudentManager.CommunalLocker.Open = true;
											this.StudentManager.CommunalLocker.SpawnSteamNoSideEffects(this);
											this.MustChangeClothing = true;
											this.ChangeClothingPhase++;
											this.SunbathePhase++;
										}
										else if (this.SunbathePhase == 1)
										{
											this.CharacterAnimation.CrossFade(this.StripAnim);
											this.Pathfinding.canSearch = false;
											this.Pathfinding.canMove = false;
											if (this.CharacterAnimation[this.StripAnim].time >= 1.5f)
											{
												Debug.Log("Chigusa has reached her destination, so she is now being instructed to change into a bikini and update her destination.");
												if (!this.WearingBikini)
												{
													this.WearBikini();
												}
												if (this.CharacterAnimation[this.StripAnim].time > this.CharacterAnimation[this.StripAnim].length && !this.StudentManager.CommunalLocker.SteamCountdown)
												{
													this.GoPose();
													this.SunbathePhase++;
												}
											}
										}
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.ClubAnim);
										this.ClubTimer += Time.deltaTime;
										if (this.ClubTimer > 5f)
										{
											this.ClubPhase++;
											if (this.ClubPhase == this.GravureAnims.Length - 1)
											{
												this.ClubPhase = 0;
											}
											this.ClubAnim = this.GravureAnims[this.ClubPhase];
											this.ClubTimer = 0f;
										}
									}
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Guard)
							{
								this.CharacterAnimation.CrossFade(this.IdleAnim);
							}
							else if (this.Actions[this.Phase] == StudentActionType.HelpTeacher)
							{
								this.CharacterAnimation.CrossFade(this.ThinkAnim);
								if (this.CharacterAnimation[this.ThinkAnim].time > this.CharacterAnimation[this.ThinkAnim].length)
								{
									this.GetTeacherTarget();
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Admire)
							{
								if (this.Stalker)
								{
									this.CharacterAnimation.CrossFade(this.AdmireAnim);
								}
								else
								{
									StudentScript studentScript2 = this.StudentManager.Students[this.InfatuationID];
									if (studentScript2 != null)
									{
										if (!studentScript2.gameObject.activeInHierarchy || !studentScript2.enabled)
										{
											Debug.Log("The target's disabled or dead.");
											this.CharacterAnimation.CrossFade(this.LookLeftRightAnim);
											this.SnackTimer += Time.deltaTime;
											if (this.SnackTimer >= 5f)
											{
												this.StopFollowingGravureModel();
											}
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.AdmireAnim);
										}
									}
									if (this.StudentManager.Students[this.InfatuationID] != null && this.StudentManager.Students[this.InfatuationID].Sleepy && this.StudentManager.Students[this.InfatuationID].DistanceToDestination < 1f)
									{
										this.StopFollowingGravureModel();
									}
									this.ScootAway();
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Rehearse)
							{
								if (this.StudentID < 28)
								{
									this.CharacterAnimation.CrossFade(this.ActAnim);
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.ThinkAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Stretch)
							{
								this.CharacterAnimation.CrossFade(this.StretchAnim);
								this.StretchTimer += Time.deltaTime;
								if (this.StretchTimer > 5f)
								{
									this.StretchPhase++;
									if (this.StretchPhase == this.StretchAnims.Length)
									{
										this.StretchPhase = 0;
									}
									this.StretchAnim = this.StretchAnims[this.StretchPhase];
									this.StretchTimer = 0f;
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.CustomHangout)
							{
								this.CharacterAnimation.CrossFade(this.CustomHangoutAnim);
							}
							else if (this.Actions[this.Phase] == StudentActionType.CustomPatrol)
							{
								this.DressCode = false;
								this.CharacterAnimation.CrossFade(this.CustomPatrolAnim);
								this.PatrolTimer += Time.deltaTime;
								if (this.PatrolTimer >= 10f || this.Pathfinding.target == null)
								{
									this.PatrolID++;
									if (this.PatrolID == this.StudentManager.CustomPatrols.List[this.StudentID].childCount || this.Pathfinding.target == null)
									{
										this.PatrolID = 0;
									}
									this.CurrentDestination = this.StudentManager.CustomPatrols.List[this.StudentID].GetChild(this.PatrolID);
									this.Pathfinding.target = this.CurrentDestination;
									this.PatrolTimer = 0f;
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.RandomPatrol)
							{
								this.DressCode = false;
								this.CharacterAnimation.CrossFade(this.CustomPatrolAnim);
								this.PatrolTimer += Time.deltaTime;
								if (this.PatrolTimer >= 10f || this.Pathfinding.target == null)
								{
									this.CurrentDestination = this.StudentManager.CustomPatrols.List[UnityEngine.Random.Range(1, 101)].GetChild(UnityEngine.Random.Range(0, 2));
									this.Pathfinding.target = this.CurrentDestination;
									this.PatrolTimer = 0f;
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.BakeSale)
							{
								if (this.BakeSalePhase == 0)
								{
									this.CharacterAnimation.CrossFade(this.PrepareFoodAnim);
									this.ClubTimer += Time.deltaTime;
									if (this.ClubTimer > 60f)
									{
										this.MyPlate.gameObject.SetActive(true);
										this.MyPlate.parent = this.RightHand;
										this.MyPlate.localPosition = new Vector3(0.02f, -0.02f, -0.15f);
										this.MyPlate.localEulerAngles = new Vector3(-5f, -90f, 172.5f);
										this.IdleAnim = this.PlateIdleAnim;
										this.WalkAnim = this.PlateWalkAnim;
										this.LeanAnim = this.PlateIdleAnim;
										this.StudentManager.BakeSalePrepSpots[this.StudentID].position = this.StudentManager.BakeSaleSpots[this.StudentID].position;
										this.StudentManager.BakeSalePrepSpots[this.StudentID].rotation = this.StudentManager.BakeSaleSpots[this.StudentID].rotation;
										this.StudentManager.BakeSaleSabotageScripts[this.StudentID].Disable();
										this.DistanceToDestination = 100f;
										this.BakeSalePhase++;
										this.CanTalk = false;
										this.ClubTimer = 0f;
									}
								}
								else if (this.BakeSalePhase == 1)
								{
									if (this.MyPlate != null)
									{
										if (this.MyPlate.parent == this.RightHand)
										{
											this.MyPlate.parent = null;
											this.MyPlate.position = this.StudentManager.BakeSalePlateParents[this.StudentID].position;
											this.MyPlate.rotation = this.StudentManager.BakeSalePlateParents[this.StudentID].rotation;
											this.IdleAnim = this.OriginalIdleAnim;
											this.WalkAnim = this.OriginalWalkAnim;
											this.LeanAnim = this.OriginalLeanAnim;
											this.StudentManager.BakeSale.enabled = true;
											this.BakeSalePhase++;
											this.CanTalk = true;
										}
										else if (this.StudentID == 12)
										{
											Debug.Log("Amai's MyPlate is not in her hand for some reason?");
										}
									}
								}
								else if (this.BakeSalePhase == 2)
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.Picnic)
							{
								if (this.Male)
								{
									this.CharacterAnimation.CrossFade("sit_02");
								}
								else
								{
									this.CharacterAnimation.CrossFade("f02_sit_02");
								}
							}
							else if (this.Actions[this.Phase] == StudentActionType.DeskDraw)
							{
								if (!this.Pen.activeInHierarchy)
								{
									this.Drawing.SetActive(true);
									this.Pen.SetActive(true);
								}
								this.CharacterAnimation.CrossFade(this.SitAnim);
								if (this.StudentID == 5 && this.Clock.GameplayDay == 1 && !this.BountyCollider.activeInHierarchy)
								{
									this.BountyCollider.transform.localPosition = new Vector3(0f, 0f, 0f);
									this.BountyCollider.GetComponent<BoxCollider>().center = new Vector3(0f, 0.65f, 0.175f);
									this.BountyCollider.GetComponent<BoxCollider>().size = new Vector3(0.45f, 1.3f, 0.55f);
									this.BountyCollider.SetActive(true);
								}
							}
						}
						else
						{
							this.CurrentDestination = this.StudentManager.GoAwaySpots.List[this.StudentID];
							this.Pathfinding.target = this.StudentManager.GoAwaySpots.List[this.StudentID];
							this.CharacterAnimation.CrossFade(this.IdleAnim);
							this.GoAwayTimer += Time.deltaTime;
							if (this.GoAwayTimer > 10f)
							{
								Debug.Log("This code is only called after a character has spent 10 seconds standing in a 'Go Away'' spot.");
								this.CurrentDestination = this.Destinations[this.Phase];
								this.Pathfinding.target = this.Destinations[this.Phase];
								this.GoAwayTimer = 0f;
								this.GoAway = false;
							}
						}
					}
					else
					{
						if (this.MeetTimer == 0f)
						{
							if (this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood == 0f && (double)this.Yandere.Sanity >= 66.66666 && (this.CurrentDestination == this.StudentManager.MeetSpots.List[8] || this.CurrentDestination == this.StudentManager.MeetSpots.List[9] || this.CurrentDestination == this.StudentManager.MeetSpots.List[10]))
							{
								if (this.StudentManager.Eighties && this.StudentID == this.StudentManager.RivalID)
								{
									if (this.StudentManager.EightiesOfferHelp != null)
									{
										this.StudentManager.EightiesOfferHelp.UpdateLocation();
										this.StudentManager.EightiesOfferHelp.enabled = true;
										this.StudentManager.EightiesOfferHelp.Prompt.enabled = true;
									}
								}
								else if (this.StudentID == this.StudentManager.RivalID)
								{
									this.StudentManager.OsanaOfferHelp.UpdateLocation();
									this.StudentManager.OsanaOfferHelp.enabled = true;
									this.StudentManager.OsanaOfferHelp.Prompt.enabled = true;
								}
								else if (this.StudentID == 5)
								{
									this.Yandere.BullyPhotoCheck();
									if (this.Yandere.BullyPhoto)
									{
										this.StudentManager.FragileOfferHelp.gameObject.SetActive(true);
										this.StudentManager.FragileOfferHelp.UpdateLocation();
										this.StudentManager.FragileOfferHelp.enabled = true;
										this.StudentManager.FragileOfferHelp.Prompt.enabled = true;
									}
								}
							}
							if (!SchoolGlobals.RoofFence && base.transform.position.y > 11f)
							{
								this.Pushable = true;
								this.StudentManager.UpdateMe(this.StudentID);
							}
							if (this.CurrentDestination == this.StudentManager.FountainSpot)
							{
								Debug.Log(this.Name + " is now drownable.");
								this.Drownable = true;
								this.StudentManager.UpdateMe(this.StudentID);
							}
						}
						this.MeetTimer += Time.deltaTime;
						if (this.Follower != null)
						{
							Debug.Log("Osana is now repositioning her own FollowTargetDestination.");
							if (this.Meeting)
							{
								this.FollowTargetDestination.localPosition = new Vector3(-1f, 0f, 0f);
							}
							else if (this.CurrentAction == StudentActionType.Clean)
							{
								this.FollowTargetDestination.localPosition = new Vector3(-1f, 0f, -1f);
							}
							else if (this.CurrentAction == StudentActionType.SitAndEatBento)
							{
								this.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 1f);
							}
							else
							{
								this.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 0f);
							}
						}
						if (this.BakeSale)
						{
							this.CharacterAnimation.CrossFade(this.PlateEatAnim);
							if ((double)this.CharacterAnimation[this.PlateEatAnim].time > 6.83333)
							{
								this.Fingerfood[12].SetActive(false);
							}
							else if ((double)this.CharacterAnimation[this.PlateEatAnim].time > 2.66666)
							{
								this.Fingerfood[12].SetActive(true);
							}
							this.MeetTimer += Time.deltaTime * 6.5f;
						}
						else if (this.Male)
						{
							this.CharacterAnimation.CrossFade("impatientWait_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade("f02_impatientWait_00");
						}
						if (this.MeetTimer > 60f)
						{
							if (!this.BakeSale)
							{
								if (!this.Male)
								{
									this.Subtitle.UpdateLabel(SubtitleType.NoteReaction, 4, 3f);
								}
								else if (this.StudentID == 28)
								{
									this.Subtitle.UpdateLabel(SubtitleType.NoteReactionMale, 6, 3f);
								}
								else
								{
									this.Subtitle.UpdateLabel(SubtitleType.NoteReactionMale, 4, 3f);
								}
							}
							if (this.Phase < this.ScheduleBlocks.Length)
							{
								while (this.Clock.HourTime >= this.ScheduleBlocks[this.Phase].time)
								{
									this.Phase++;
								}
							}
							this.CurrentDestination = this.Destinations[this.Phase];
							this.Pathfinding.target = this.Destinations[this.Phase];
							if (this.Follower != null)
							{
								this.Follower.CurrentDestination = this.Follower.FollowTarget.transform;
								this.Follower.Pathfinding.target = this.Follower.FollowTarget.transform;
								this.FollowTargetDestination.localPosition = new Vector3(0f, 0f, 0f);
							}
							this.StopMeeting();
							if (this.BakeSale && this.StudentManager.BakeSale.Poisoned)
							{
								this.Fingerfood[12].SetActive(false);
								this.GoPuke();
							}
							this.BakeSale = false;
						}
					}
				}
			}
		}
		else
		{
			if (this.CurrentDestination != null)
			{
				this.DistanceToDestination = Vector3.Distance(base.transform.position, this.CurrentDestination.position);
			}
			if (this.Fleeing)
			{
				if (this.WitnessedMurder && this.Persona == PersonaType.Heroic)
				{
					this.Pathfinding.target = this.Yandere.transform;
					this.CurrentDestination = this.Yandere.transform;
				}
				if (this.FollowTarget != null && Vector3.Distance(base.transform.position, this.FollowTarget.transform.position) < 10f && this.FollowTarget.Attacked && this.FollowTarget.Alive && !this.FollowTarget.Tranquil && !this.Blind)
				{
					Debug.Log("Raibaru should be aware that Osana is being attacked.");
					this.AwareOfMurder = true;
					this.Alarm = 200f;
				}
				if (this.Struggling && this.Lost)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Yandere.transform.rotation, 10f * Time.deltaTime);
					if (!this.StopSliding)
					{
						this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward * 0.425f);
					}
					if (this.StudentManager.Headmaster.Heartbroken.Dead)
					{
						this.StruggleBar.Struggling = false;
						this.Struggling = false;
					}
				}
				if (!this.Dying && !this.Spraying)
				{
					if (!this.PinningDown)
					{
						if (this.Persona == PersonaType.Dangerous)
						{
							this.Yandere.Pursuer = this;
							Debug.Log("This student council member is running to intercept Yandere-chan.");
							if (this.Yandere.Laughing)
							{
								this.Yandere.StopLaughing();
								this.Yandere.Chased = true;
								this.Yandere.CanMove = false;
							}
							if (this.Yandere.Cauterizing || this.Yandere.Dismembering || this.Yandere.WrappingCorpse)
							{
								if (this.Yandere.Dismembering)
								{
									this.Yandere.StopDismembering();
								}
								if (this.Yandere.WrappingCorpse)
								{
									this.Yandere.StopWrappingCorpse();
								}
								this.Yandere.Cauterizing = false;
								this.Yandere.Chased = true;
								this.Yandere.CanMove = false;
							}
							if (this.StudentManager.CombatMinigame.Path > 3 && this.StudentManager.CombatMinigame.Path < 7)
							{
								this.ReturnToRoutine();
							}
						}
						if (this.Pathfinding.target != null)
						{
							this.DistanceToDestination = Vector3.Distance(base.transform.position, this.Pathfinding.target.position);
						}
						if (this.AlarmTimer > 0f)
						{
							this.AlarmTimer = Mathf.MoveTowards(this.AlarmTimer, 0f, Time.deltaTime);
							if (this.StudentID == 1)
							{
								Debug.Log("Senpai entered his scared animation.");
							}
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
							if (this.AlarmTimer == 0f)
							{
								Debug.Log("Alarmed is being set to false from here.");
								this.WalkBack = false;
								this.Alarmed = false;
							}
							this.Pathfinding.canSearch = false;
							this.Pathfinding.canMove = false;
							if (this.WitnessedMurder)
							{
								this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
								base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
							}
							else if (this.WitnessedCorpse)
							{
								this.targetRotation = Quaternion.LookRotation(new Vector3(this.Corpse.AllColliders[0].transform.position.x, base.transform.position.y, this.Corpse.AllColliders[0].transform.position.z) - base.transform.position);
								base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
							}
						}
						else
						{
							if (this.Persona == PersonaType.TeachersPet && this.WitnessedMurder && this.ReportPhase == 0 && this.StudentManager.Reporter == null && !this.Police.Called)
							{
								Debug.Log(this.Name + " is setting their teacher as their destination at the beginning of Flee protocol.");
								this.Pathfinding.target = this.StudentManager.Teachers[this.Class].transform;
								this.CurrentDestination = this.StudentManager.Teachers[this.Class].transform;
								this.StudentManager.Reporter = this;
								this.ReportingMurder = true;
								this.DetermineCorpseLocation();
							}
							if (base.transform.position.y < -10f)
							{
								Debug.Log("A student has just run out of Akademi.");
								if (!this.StudentManager.Jammed)
								{
									if (this.Persona == PersonaType.PhoneAddict && this.WitnessedMurder && !this.SawMask)
									{
										this.PhoneAddictGameOver();
									}
									else if (this.FoundEnemyCorpse)
									{
										Debug.Log("Not going to call the cops, since the corpse belonged to an enemy.");
									}
									else if (this.Persona != PersonaType.Evil && this.Persona != PersonaType.Coward && this.Persona != PersonaType.Fragile)
									{
										Debug.Log(this.Name + " just called the cops.");
										this.Police.Called = true;
										this.Police.Show = true;
									}
								}
								base.transform.position = new Vector3(base.transform.position.x, -100f, base.transform.position.z);
								base.gameObject.SetActive(false);
							}
							if (base.transform.position.y < -11f)
							{
								if (this.Persona != PersonaType.Coward && this.Persona != PersonaType.Evil && this.Persona != PersonaType.Fragile && this.OriginalPersona != PersonaType.Evil)
								{
									this.Police.Witnesses--;
									if (!this.StudentManager.Jammed)
									{
										this.Police.Show = true;
										if (this.Countdown.gameObject.activeInHierarchy)
										{
											this.PhoneAddictGameOver();
										}
									}
								}
								base.transform.position = new Vector3(base.transform.position.x, -100f, base.transform.position.z);
								base.gameObject.SetActive(false);
							}
							if (base.transform.position.z < -99f)
							{
								this.Prompt.Hide();
								this.Prompt.enabled = false;
								this.Safe = true;
							}
							if (this.DistanceToDestination > this.TargetDistance)
							{
								if (!this.Phoneless)
								{
									this.CharacterAnimation.CrossFade(this.SprintAnim);
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.OriginalSprintAnim);
								}
								this.Pathfinding.canSearch = true;
								this.Pathfinding.canMove = true;
								if (this.Pursuing && this.Yandere.CanMove && !this.Yandere.Yakuza)
								{
									Debug.Log("A character is pursuing Yandere, yet Yandere can move?");
									this.PursueTimer += Time.deltaTime;
									if (this.PursueTimer > 1f)
									{
										this.Yandere.CanMove = false;
									}
								}
								if (this.Yandere.Chased && this.Yandere.Pursuer == this)
								{
									if (this.Persona == PersonaType.Protective && !this.StudentManager.ChallengeManager.InvincibleRaibaru && this.Yandere.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0)
									{
										this.Persona = PersonaType.Heroic;
									}
									Debug.Log(this.Name + " is now chasing Yandere-chan.");
									this.FocusOnYandere = false;
									this.CharacterAnimation.CrossFade(this.SprintAnim);
									this.Pathfinding.repathRate = 0f;
									this.Pathfinding.speed = 5f;
									this.ChaseTimer += Time.deltaTime;
									if (this.ChaseTimer > 10f && !this.Yandere.Yakuza)
									{
										base.transform.position = this.Yandere.transform.position + this.Yandere.transform.forward * 0.999f;
										base.transform.LookAt(this.Yandere.transform.position);
										Physics.SyncTransforms();
									}
									if (this.Yandere.Dismembering)
									{
										this.Yandere.StopDismembering();
									}
									if (this.Yandere.WrappingCorpse)
									{
										this.Yandere.StopWrappingCorpse();
									}
									if (this.Yandere.Hiding)
									{
										this.Yandere.PromptBar.ClearButtons();
										this.Yandere.PromptBar.Show = false;
										this.Yandere.Hiding = false;
									}
									if (this.Yandere.CanMove && !this.Yandere.Yakuza)
									{
										Debug.Log("At this place in the code, the player SHOULD be freezing in place and waiting for an attacker to approach.");
										this.Yandere.PreparedForStruggle = false;
										this.Yandere.CanMove = false;
									}
								}
								else
								{
									this.Pathfinding.speed = 4f;
								}
								if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless && !this.CrimeReported)
								{
									if (!this.StudentManager.Eighties)
									{
										this.Countdown.Sprite.fillAmount = Mathf.MoveTowards(this.Countdown.Sprite.fillAmount, 0f, Time.deltaTime * this.Countdown.Speed);
									}
									if (this.Countdown.Sprite.fillAmount == 0f)
									{
										this.Countdown.Sprite.fillAmount = 1f;
										this.CrimeReported = true;
										if (this.WitnessedMurder && !this.Countdown.MaskedPhoto)
										{
											this.PhoneAddictGameOver();
										}
										else
										{
											if (this.StudentManager.ChaseCamera == this.ChaseCamera)
											{
												this.StudentManager.ChaseCamera = null;
											}
											this.SprintAnim = this.OriginalSprintAnim;
											this.Countdown.gameObject.SetActive(false);
											this.ChaseCamera.SetActive(false);
											if (!this.StudentManager.Jammed)
											{
												Debug.Log(this.Name + " just called the cops.");
												this.Police.Called = true;
												this.Police.Show = true;
											}
										}
									}
									else if (!this.StudentManager.Eighties)
									{
										this.SprintAnim = this.PhoneAnims[2];
										if (this.StudentManager.ChaseCamera == null)
										{
											this.StudentManager.ChaseCamera = this.ChaseCamera;
											this.ChaseCamera.SetActive(true);
										}
									}
								}
							}
							else
							{
								this.Pathfinding.canSearch = false;
								this.Pathfinding.canMove = false;
								if (!this.Halt)
								{
									if (this.StudentID > 1)
									{
										this.MoveTowardsTarget(this.Pathfinding.target.position);
										if (!this.Teacher && !this.FocusOnYandere)
										{
											base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Pathfinding.target.rotation, 10f * Time.deltaTime);
										}
									}
								}
								else
								{
									if (this.Spraying)
									{
										this.CharacterAnimation.CrossFade(this.SprayAnim);
									}
									if (this.Persona == PersonaType.TeachersPet)
									{
										this.targetRotation = Quaternion.LookRotation(new Vector3(this.StudentManager.Teachers[this.Class].transform.position.x, base.transform.position.y, this.StudentManager.Teachers[this.Class].transform.position.z) - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
									}
									else if (this.Persona == PersonaType.Dangerous && !this.BreakingUpFight)
									{
										this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
									}
								}
								if (this.Persona == PersonaType.TeachersPet)
								{
									if (this.ReportingMurder || this.ReportingBlood)
									{
										if (this.StudentManager.Teachers[this.Class].Alarmed && this.ReportPhase < 100)
										{
											if (this.Club == ClubType.Council)
											{
												this.Pathfinding.target = this.StudentManager.CorpseLocation;
												this.CurrentDestination = this.StudentManager.CorpseLocation;
											}
											else
											{
												if (this.PetDestination == null)
												{
													this.PetDestination = UnityEngine.Object.Instantiate<GameObject>(this.EmptyGameObject, this.Seat.position + this.Seat.forward * -0.5f, Quaternion.identity).transform;
												}
												this.Pathfinding.target = this.PetDestination;
												this.CurrentDestination = this.PetDestination;
											}
											this.ReportPhase = 3;
										}
										if (this.ReportPhase == 0)
										{
											Debug.Log(this.Name + ", currently acting as a Teacher's Pet, is talking to a teacher.");
											if (this.MyTeacher == null)
											{
												this.MyTeacher = this.StudentManager.Teachers[this.Class];
											}
											if (!this.MyTeacher.Alive)
											{
												this.Persona = PersonaType.Loner;
												this.PersonaReaction();
											}
											else
											{
												this.Subtitle.Speaker = this;
												this.CharacterAnimation.CrossFade(this.ScaredAnim);
												if (this.WitnessedMurder)
												{
													this.Subtitle.UpdateLabel(SubtitleType.PetMurderReport, 2, 3f);
												}
												else if (this.WitnessedCorpse)
												{
													if (this.Club == ClubType.Council)
													{
														this.Subtitle.CustomText = "";
														this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 0f);
														if (this.StudentID == 86)
														{
															this.Subtitle.UpdateLabel(SubtitleType.StrictReport, 2, 5f);
														}
														else if (this.StudentID == 87)
														{
															this.Subtitle.UpdateLabel(SubtitleType.CasualReport, 2, 5f);
														}
														else if (this.StudentID == 88)
														{
															this.Subtitle.UpdateLabel(SubtitleType.GraceReport, 2, 5f);
														}
														else if (this.StudentID == 89)
														{
															this.Subtitle.UpdateLabel(SubtitleType.EdgyReport, 2, 5f);
														}
													}
													else
													{
														this.Subtitle.UpdateLabel(SubtitleType.PetCorpseReport, 2, 3f);
													}
												}
												else if (this.WitnessedLimb)
												{
													this.Subtitle.UpdateLabel(SubtitleType.PetLimbReport, 2, 3f);
												}
												else if (this.WitnessedBloodyWeapon)
												{
													this.Subtitle.UpdateLabel(SubtitleType.PetBloodyWeaponReport, 2, 3f);
												}
												else if (this.WitnessedBloodPool)
												{
													this.Subtitle.UpdateLabel(SubtitleType.PetBloodReport, 2, 3f);
												}
												else if (this.WitnessedWeapon)
												{
													this.Subtitle.UpdateLabel(SubtitleType.PetWeaponReport, 2, 3f);
												}
												this.MyTeacher = this.StudentManager.Teachers[this.Class];
												this.MyTeacher.CurrentDestination = this.MyTeacher.transform;
												this.MyTeacher.Pathfinding.target = this.MyTeacher.transform;
												this.MyTeacher.Pathfinding.canSearch = false;
												this.MyTeacher.Pathfinding.canMove = false;
												this.MyTeacher.CharacterAnimation.CrossFade(this.MyTeacher.IdleAnim);
												this.MyTeacher.ListeningToReport = true;
												this.MyTeacher.Routine = false;
												if (this.MyTeacher.Persona != PersonaType.Strict)
												{
													Debug.Log("The teacher wasn't strict. Must be Custom Mode. Turning the teacher strict, then.");
													this.MyTeacher.Persona = PersonaType.Strict;
												}
												if (this.StudentManager.Teachers[this.Class].Investigating)
												{
													this.StudentManager.Teachers[this.Class].StopInvestigating();
												}
												this.Halt = true;
												this.ReportPhase++;
											}
										}
										else if (this.ReportPhase == 1)
										{
											this.Pathfinding.target = this.StudentManager.Teachers[this.Class].transform;
											this.CurrentDestination = this.StudentManager.Teachers[this.Class].transform;
											if (this.WitnessedBloodPool || (this.WitnessedWeapon && !this.WitnessedBloodyWeapon))
											{
												this.CharacterAnimation.CrossFade(this.IdleAnim);
											}
											else if (this.WitnessedMurder || this.WitnessedCorpse || this.WitnessedLimb || this.WitnessedBloodyWeapon)
											{
												this.CharacterAnimation.CrossFade(this.ScaredAnim);
											}
											this.StudentManager.Teachers[this.Class].targetRotation = Quaternion.LookRotation(base.transform.position - this.StudentManager.Teachers[this.Class].transform.position);
											this.StudentManager.Teachers[this.Class].transform.rotation = Quaternion.Slerp(this.StudentManager.Teachers[this.Class].transform.rotation, this.StudentManager.Teachers[this.Class].targetRotation, 10f * Time.deltaTime);
											this.ReportTimer += Time.deltaTime;
											if (this.ReportTimer >= 3f)
											{
												this.StudentManager.Teachers[this.Class].ListeningToReport = false;
												this.StudentManager.Teachers[this.Class].MyReporter = this;
												this.StudentManager.Teachers[this.Class].Routine = false;
												this.StudentManager.Teachers[this.Class].Fleeing = true;
												this.ReportTimer = 0f;
												this.ReportPhase++;
											}
										}
										else if (this.ReportPhase == 2)
										{
											this.Pathfinding.target = this.StudentManager.Teachers[this.Class].transform;
											this.CurrentDestination = this.StudentManager.Teachers[this.Class].transform;
											if (this.WitnessedBloodPool || (this.WitnessedWeapon && !this.WitnessedBloodyWeapon))
											{
												this.CharacterAnimation.CrossFade(this.IdleAnim);
											}
											else if (this.WitnessedMurder || this.WitnessedCorpse || this.WitnessedLimb || this.WitnessedBloodyWeapon)
											{
												this.CharacterAnimation.CrossFade(this.ScaredAnim);
											}
										}
										else if (this.ReportPhase == 3)
										{
											this.Pathfinding.target = base.transform;
											this.CurrentDestination = base.transform;
											if (this.WitnessedBloodPool || (this.WitnessedWeapon && !this.WitnessedBloodyWeapon))
											{
												this.CharacterAnimation.CrossFade(this.IdleAnim);
											}
											else if (this.WitnessedMurder || this.WitnessedCorpse || this.WitnessedLimb || this.WitnessedBloodyWeapon)
											{
												this.CharacterAnimation.CrossFade(this.ParanoidAnim);
											}
										}
										else if (this.ReportPhase < 100)
										{
											this.CharacterAnimation.CrossFade(this.ParanoidAnim);
										}
										else
										{
											if (this.Pathfinding.target != base.transform)
											{
												Debug.Log("This character just set their destination to themself.");
												if (this.Club == ClubType.Council)
												{
													Debug.Log("The reporter was a student council member.");
													if (this.StudentID == 86)
													{
														this.Subtitle.UpdateLabel(SubtitleType.StrictReport, 3, 5f);
													}
													else if (this.StudentID == 87)
													{
														this.Subtitle.UpdateLabel(SubtitleType.CasualReport, 3, 5f);
													}
													else if (this.StudentID == 88)
													{
														this.Subtitle.UpdateLabel(SubtitleType.GraceReport, 3, 5f);
													}
													else if (this.StudentID == 89)
													{
														this.Subtitle.UpdateLabel(SubtitleType.EdgyReport, 3, 5f);
													}
												}
												else
												{
													this.Subtitle.Speaker = this;
													this.Subtitle.UpdateLabel(SubtitleType.PrankReaction, 1, 5f);
												}
											}
											this.Pathfinding.target = base.transform;
											this.CurrentDestination = base.transform;
											this.CharacterAnimation.CrossFade(this.ScaredAnim);
											this.ReportTimer += Time.deltaTime;
											if (this.ReportTimer >= 5f)
											{
												this.ReturnToNormal();
											}
										}
										if (this.MyTeacher != null && this.MyTeacher.Guarding && this.Club == ClubType.Council)
										{
											this.CharacterAnimation.CrossFade(this.GuardAnim);
											this.Persona = PersonaType.Dangerous;
											this.Guarding = true;
											this.Fleeing = false;
										}
									}
									else if (this.Club == ClubType.Council)
									{
										this.CharacterAnimation.CrossFade(this.GuardAnim);
										this.Persona = PersonaType.Dangerous;
										this.Guarding = true;
										this.Fleeing = false;
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.ParanoidAnim);
										this.ReportPhase = 100;
										this.Fleeing = false;
										this.Persona = this.OriginalPersona;
										this.IgnoringPettyActions = true;
										this.Guarding = true;
									}
								}
								else if (this.Persona == PersonaType.Heroic)
								{
									if (this.Yandere.Attacking || (this.Yandere.Struggling && this.Yandere.StruggleBar.Student != this))
									{
										Debug.Log(this.Name + " is waiting their turn to fight Yandere-chan.");
										this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
										this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
										this.Pathfinding.canSearch = false;
										this.Pathfinding.canMove = false;
									}
									else if (!this.Yandere.Attacking && !this.StudentManager.PinningDown && !this.Yandere.Shoved && !this.Yandere.Dumping && !this.Yandere.Dropping)
									{
										if (this.StudentID > 1)
										{
											if (!this.Yandere.Struggling && !this.Yandere.StruggleBar.gameObject.activeInHierarchy && this.Yandere.RPGCamera.enabled)
											{
												bool flag5 = false;
												if (!this.StudentManager.ChallengeManager.InvincibleRaibaru && this.Yandere.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0)
												{
													Debug.Log("Player meets the criteria to have a physical struggle with Raibaru.");
													flag5 = true;
												}
												if (this.Strength == 7)
												{
													Debug.Log(this.Name + " is calling Spray() from this place in the code.");
													this.Spray();
												}
												else if (this.Strength == 9 && !flag5)
												{
													Debug.Log(this.Name + " is calling InvincibleTakedown() from this place in the code.");
													this.InvincibleTakedown();
												}
												else if (this.Frame > 0)
												{
													Debug.Log(this.Name + " is calling BeginStruggle() from this place in the code.");
													this.BeginStruggle();
												}
												this.Frame++;
											}
											if (!this.Spraying)
											{
												this.CharacterAnimation.CrossFade(this.StruggleAnim);
												if (!this.Teacher)
												{
													this.CharacterAnimation[this.StruggleAnim].time = this.Yandere.CharacterAnimation["f02_struggleA_00"].time;
												}
												else
												{
													this.CharacterAnimation[this.StruggleAnim].time = this.Yandere.CharacterAnimation["f02_teacherStruggleA_00"].time;
												}
												base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Yandere.transform.rotation, 10f * Time.deltaTime);
												if (!this.StopSliding)
												{
													this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward * 0.425f);
												}
												if (!this.Yandere.Armed || (this.Yandere.Armed && this.Yandere.EquippedWeapon.Broken))
												{
													this.CheckForKnifeInInventory();
												}
												if (!this.Yandere.Won && (!this.Yandere.Armed || !this.Yandere.EquippedWeapon.Concealable || this.Yandere.EquippedWeapon.Broken || this.Yandere.EquippedWeapon.Type == WeaponType.Garrote))
												{
													Debug.Log("The game thinks that the player's equipped weapon is broken.");
													if (!this.Yandere.Lost)
													{
														this.BeginStruggle();
														this.Yandere.StruggleBar.HeroWins();
													}
												}
												if (this.Lost)
												{
													this.CharacterAnimation.CrossFade(this.StruggleWonAnim);
													if (this.CharacterAnimation[this.StruggleWonAnim].time > 1f)
													{
														this.EyeShrink = 1f;
													}
												}
												else if (this.Won)
												{
													this.CharacterAnimation.CrossFade(this.StruggleLostAnim);
												}
											}
										}
										else if (this.Yandere.Mask != null)
										{
											this.Yandere.EmptyHands();
											this.Pathfinding.canSearch = false;
											this.Pathfinding.canMove = false;
											this.TargetDistance = 1f;
											this.Yandere.CharacterAnimation.CrossFade("f02_unmasking_00");
											string text;
											if (this.Male)
											{
												text = "unmasking_00";
											}
											else
											{
												text = "f02_unmasking_01";
											}
											this.CharacterAnimation.CrossFade(text);
											this.Yandere.CanMove = false;
											this.targetRotation = Quaternion.LookRotation(this.Yandere.transform.position - base.transform.position);
											base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
											this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward * 1f);
											if (this.CharacterAnimation[text].time == 0f)
											{
												this.Yandere.ShoulderCamera.YandereNo();
												this.Yandere.Jukebox.GameOver();
											}
											if (this.CharacterAnimation[text].time >= 0.66666f && this.Yandere.Mask.transform.parent != this.LeftHand)
											{
												this.Yandere.CanMove = true;
												this.Yandere.EmptyHands();
												this.Yandere.CanMove = false;
												this.Yandere.Mask.transform.parent = this.LeftHand;
												this.Yandere.Mask.transform.localPosition = new Vector3(-0.1f, -0.05f, 0f);
												this.Yandere.Mask.transform.localEulerAngles = new Vector3(-90f, 90f, 0f);
												this.Yandere.Mask.transform.localScale = new Vector3(1f, 1f, 1f);
											}
											if (this.CharacterAnimation[text].time >= this.CharacterAnimation[text].length)
											{
												this.Yandere.Unmasked = true;
												this.Yandere.ShoulderCamera.GameOver();
												this.Yandere.Mask.Drop();
											}
										}
									}
									else if (!this.Struggling)
									{
										Debug.Log(this.Name + " is waiting their turn to fight Yandere-chan.");
										this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
									}
								}
								else if (this.Persona == PersonaType.Coward || this.Persona == PersonaType.Fragile)
								{
									this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
									base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
									this.CharacterAnimation.CrossFade(this.CowardAnim);
									if (!this.StudentManager.KokonaTutorial)
									{
										this.ReactionTimer += Time.deltaTime;
										if (this.ReactionTimer > 5f)
										{
											this.CurrentDestination = this.StudentManager.Exit;
											this.Pathfinding.target = this.StudentManager.Exit;
										}
									}
								}
								else if (this.Persona == PersonaType.Evil)
								{
									this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
									base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
									this.CharacterAnimation.CrossFade(this.EvilAnim);
									this.ReactionTimer += Time.deltaTime;
									if (this.ReactionTimer > 5f)
									{
										this.CurrentDestination = this.StudentManager.Exit;
										this.Pathfinding.target = this.StudentManager.Exit;
									}
								}
								else if (this.Persona == PersonaType.SocialButterfly)
								{
									if (this.ReportPhase < 4)
									{
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Pathfinding.target.rotation, 10f * Time.deltaTime);
									}
									if (this.ReportPhase == 1)
									{
										if (!this.SmartPhone.activeInHierarchy)
										{
											if (this.StudentManager.Reporter == null && !this.Police.Called)
											{
												this.CharacterAnimation.CrossFade(this.SocialReportAnim);
												this.Subtitle.UpdateLabel(SubtitleType.SocialReport, 1, 5f);
												this.StudentManager.Reporter = this;
												this.SmartPhone.SetActive(true);
												this.SmartPhone.transform.localPosition = new Vector3(-0.015f, -0.01f, 0f);
												this.SmartPhone.transform.localEulerAngles = new Vector3(0f, -170f, 165f);
											}
											else
											{
												this.ReportTimer = 5f;
											}
										}
										this.ReportTimer += Time.deltaTime;
										if (this.ReportTimer > 5f)
										{
											if (this.StudentManager.Reporter == this && !this.StudentManager.Jammed)
											{
												Debug.Log(this.Name + " just called the cops.");
												this.Police.Called = true;
												this.Police.Show = true;
											}
											this.CharacterAnimation.CrossFade(this.ParanoidAnim);
											this.SmartPhone.SetActive(false);
											this.ReportPhase++;
											this.Halt = false;
										}
									}
									else if (this.ReportPhase == 2)
									{
										if (this.WitnessedMurder && (!this.SawMask || (this.SawMask && this.Yandere.Mask != null)))
										{
											this.LookForYandere();
										}
									}
									else if (this.ReportPhase == 3)
									{
										this.CharacterAnimation.CrossFade(this.SocialFearAnim);
										this.Subtitle.UpdateLabel(SubtitleType.SocialFear, 1, 5f);
										this.SpawnAlarmDisc();
										this.ReportPhase++;
										this.Halt = true;
									}
									else if (this.ReportPhase == 4)
									{
										this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
										if (this.Yandere.Attacking)
										{
											this.LookForYandere();
										}
									}
									else if (this.ReportPhase == 5)
									{
										this.CharacterAnimation.CrossFade(this.SocialTerrorAnim);
										this.Subtitle.UpdateLabel(SubtitleType.SocialTerror, 1, 5f);
										this.VisionDistance = 0f;
										this.SpawnAlarmDisc();
										this.ReportPhase++;
									}
								}
								else if (this.Persona == PersonaType.Lovestruck)
								{
									if (this.ReportPhase < 3)
									{
										if (this.LovestruckTarget == 0 || this.StudentManager.Students[this.LovestruckTarget] == null || this.StudentManager.Students[this.LovestruckTarget].Fleeing)
										{
											Debug.Log("Lovestruck Target is null or fleeing, so destination is being set to Exit.");
											this.Pathfinding.target = this.StudentManager.Exit;
											this.CurrentDestination = this.StudentManager.Exit;
											this.ReportPhase = 3;
										}
										else if (this.StudentManager.Students[this.LovestruckTarget].InEvent && this.StudentManager.Students[this.LovestruckTarget].Ragdoll.Zs.activeInHierarchy)
										{
											Debug.Log("Lovestruck Target is asleep, so she needs to be woken up first.");
											this.StudentManager.Yandere.Class.Portal.OsanaThursdayEvent.EndEvent();
										}
									}
									if (this.ReportPhase == 1)
									{
										if (!this.StudentManager.Students[this.LovestruckTarget].gameObject.activeInHierarchy || this.StudentManager.Students[this.LovestruckTarget].Ragdoll.Concealed || this.StudentManager.Students[this.LovestruckTarget].Ragdoll.Zs.activeInHierarchy)
										{
											Debug.Log("A character wants to run to someone to tell them about murder, but that character is either gone or in a garbage bag.");
											this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 5, 10f);
											this.Pathfinding.target = this.StudentManager.Exit;
											this.CurrentDestination = this.StudentManager.Exit;
											this.Pathfinding.enabled = true;
											this.ReportPhase = 3;
										}
										else if (!this.StudentManager.Students[this.LovestruckTarget].Alive)
										{
											this.CurrentDestination = this.Corpse.Student.Hips;
											this.Pathfinding.target = this.Corpse.Student.Hips;
											this.SpecialRivalDeathReaction = true;
											this.WitnessRivalDiePhase = 1;
											this.Fleeing = false;
											this.Routine = false;
											this.TargetDistance = 0.5f;
										}
										else if (this.StudentManager.Students[this.LovestruckTarget].Routine && this.Yandere.CanMove)
										{
											if (this.StudentManager.Students[this.LovestruckTarget].Male)
											{
												this.StudentManager.Students[this.LovestruckTarget].CharacterAnimation.CrossFade("surprised_00");
											}
											else
											{
												this.StudentManager.Students[this.LovestruckTarget].CharacterAnimation.CrossFade("f02_surprised_00");
											}
											this.StudentManager.Students[this.LovestruckTarget].EmptyHands();
											this.CharacterAnimation.CrossFade(this.ScaredAnim);
											this.StudentManager.Students[this.LovestruckTarget].Pathfinding.canSearch = false;
											this.StudentManager.Students[this.LovestruckTarget].Pathfinding.canMove = false;
											this.StudentManager.Students[this.LovestruckTarget].Pathfinding.enabled = false;
											this.StudentManager.Students[this.LovestruckTarget].Investigating = false;
											this.StudentManager.Students[this.LovestruckTarget].CheckingNote = false;
											this.StudentManager.Students[this.LovestruckTarget].Meeting = false;
											this.StudentManager.Students[this.LovestruckTarget].AwareOfCorpse = true;
											this.StudentManager.Students[this.LovestruckTarget].Routine = false;
											this.StudentManager.Students[this.LovestruckTarget].Blind = true;
											this.ReportingMurderToSenpai = true;
											this.Pathfinding.enabled = false;
											if (this.WitnessedMurder && !this.SawMask)
											{
												this.Yandere.CharacterAnimation.CrossFade("f02_readyToFight_00");
												this.Yandere.Jukebox.gameObject.active = false;
												this.Yandere.MainCamera.enabled = false;
												this.Yandere.RPGCamera.enabled = false;
												this.Yandere.Jukebox.Volume = 0f;
												this.Yandere.CanMove = false;
												this.StudentManager.LovestruckCamera.transform.parent = base.transform;
												this.StudentManager.LovestruckCamera.transform.localPosition = new Vector3(1f, 1f, -1f);
												this.StudentManager.LovestruckCamera.transform.localEulerAngles = new Vector3(0f, -30f, 0f);
												this.StudentManager.LovestruckCamera.active = true;
												this.Yandere.transform.rotation = Quaternion.LookRotation(new Vector3(this.Hips.transform.position.x, this.Yandere.transform.position.y, this.Hips.transform.position.z) - this.Yandere.transform.position);
											}
											if (this.WitnessedMurder && !this.SawMask)
											{
												this.Subtitle.UpdateLabel(SubtitleType.LovestruckMurderReport, 0, 5f);
											}
											else if (this.LovestruckTarget == 1)
											{
												this.Subtitle.UpdateLabel(SubtitleType.LovestruckCorpseReport, 0, 5f);
											}
											else
											{
												this.Subtitle.UpdateLabel(SubtitleType.LovestruckCorpseReport, 1, 5f);
											}
											this.ReportPhase++;
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.ScaredAnim);
											this.StudentManager.Students[this.LovestruckTarget].LovestruckWaiting = true;
											if (this.StudentManager.Students[this.LovestruckTarget].Confessing)
											{
												this.StudentManager.Students[this.LovestruckTarget].Confessing = false;
												this.StudentManager.Students[this.LovestruckTarget].Routine = true;
											}
										}
									}
									else if (this.ReportPhase == 2)
									{
										this.targetRotation = Quaternion.LookRotation(new Vector3(this.StudentManager.Students[this.LovestruckTarget].transform.position.x, base.transform.position.y, this.StudentManager.Students[this.LovestruckTarget].transform.position.z) - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
										this.targetRotation = Quaternion.LookRotation(new Vector3(base.transform.position.x, this.StudentManager.Students[this.LovestruckTarget].transform.position.y, base.transform.position.z) - this.StudentManager.Students[this.LovestruckTarget].transform.position);
										this.StudentManager.Students[this.LovestruckTarget].transform.rotation = Quaternion.Slerp(this.StudentManager.Students[this.LovestruckTarget].transform.rotation, this.targetRotation, 10f * Time.deltaTime);
										this.ReportTimer += Time.deltaTime;
										if (this.ReportTimer > 5f)
										{
											if (this.WitnessedMurder && !this.SawMask)
											{
												this.Yandere.ShoulderCamera.HeartbrokenCamera.SetActive(true);
												this.Yandere.Police.EndOfDay.Heartbroken.Exposed = true;
												this.Yandere.CharacterAnimation.CrossFade("f02_down_22");
												this.Yandere.Collapse = true;
												this.StudentManager.LovestruckCamera.SetActive(false);
												this.StudentManager.StopMoving();
												this.ReportPhase++;
											}
											else
											{
												Debug.Log("Both reporter and Lovestruck Target should be heading to the Exit.");
												this.StudentManager.Students[this.LovestruckTarget].CharacterAnimation.CrossFade(this.StudentManager.Students[this.LovestruckTarget].SprintAnim);
												this.StudentManager.Students[this.LovestruckTarget].Pathfinding.target = this.StudentManager.Exit;
												this.StudentManager.Students[this.LovestruckTarget].CurrentDestination = this.StudentManager.Exit;
												this.StudentManager.Students[this.LovestruckTarget].Pathfinding.canSearch = true;
												this.StudentManager.Students[this.LovestruckTarget].Pathfinding.canMove = true;
												this.StudentManager.Students[this.LovestruckTarget].Pathfinding.enabled = true;
												this.StudentManager.Students[this.LovestruckTarget].Pathfinding.speed = 4f;
												this.Pathfinding.target = this.StudentManager.Exit;
												this.CurrentDestination = this.StudentManager.Exit;
												this.Pathfinding.enabled = true;
												this.ReportPhase++;
												if (this.StudentManager.Students[this.LovestruckTarget].InCouple && this.StudentManager.Students[this.LovestruckTarget].Partner != null && Vector3.Distance(this.StudentManager.Students[this.LovestruckTarget].transform.position, this.StudentManager.Students[this.LovestruckTarget].Partner.transform.position) < 5f)
												{
													Debug.Log("Lovestruck Target's partner should be leaving, too.");
													StudentScript partner = this.StudentManager.Students[this.LovestruckTarget].Partner;
													partner.CharacterAnimation.CrossFade(partner.SprintAnim);
													partner.Pathfinding.target = this.StudentManager.Exit;
													partner.CurrentDestination = this.StudentManager.Exit;
													partner.Pathfinding.canSearch = true;
													partner.Pathfinding.canMove = true;
													partner.Pathfinding.enabled = true;
													partner.Pathfinding.speed = 4f;
												}
											}
										}
									}
									else
									{
										Debug.Log("Lovestruck student is running for an exit.");
										if (this.StudentID == 10)
										{
											Debug.Log("The Lovestruck student is Raibaru.");
										}
										if (this.FollowTarget != null)
										{
											Debug.Log("Raibaru knows about Osana.");
										}
										if (this.FollowTarget != null && Vector3.Distance(base.transform.position, this.FollowTarget.transform.position) < 10f && this.FollowTarget.Attacked && this.FollowTarget.Alive && !this.FollowTarget.Tranquil && !this.Blind)
										{
											Debug.Log("Raibaru should be aware that Osana is being attacked.");
											this.AwareOfMurder = true;
											this.Alarm = 200f;
										}
									}
								}
								else if (this.Persona == PersonaType.Dangerous || this.Strength == 7)
								{
									if (this.Pathfinding.target == this.Yandere.transform && !this.Yandere.Attacking && !this.StudentManager.PinningDown && !this.Yandere.Struggling && !this.Yandere.Noticed && !this.Yandere.Invisible)
									{
										Debug.Log("Calling ''Spray()'' from this part of the code. 1");
										this.Spray();
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
									}
								}
								else if (this.Persona == PersonaType.Protective || (this.Strength == 9 && this.Chasing))
								{
									if (!this.Yandere.Dumping && !this.Yandere.Attacking && !this.Yandere.Struggling)
									{
										bool flag6 = false;
										if (!this.StudentManager.ChallengeManager.InvincibleRaibaru && this.Yandere.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0)
										{
											Debug.Log("Player meets the criteria to have a physical struggle with Raibaru.");
											flag6 = true;
										}
										if (this.Strength >= 9 && !flag6)
										{
											Debug.Log("A protective student is calling InvincibleTakedown() now.");
											this.InvincibleTakedown();
										}
										else
										{
											Debug.Log("This student's Strength is less than 9, so they should enter a struggle when trying to apprehend the player.");
											this.CheckForKnifeInInventory();
											this.BeginStruggle();
										}
									}
									else if (!this.Struggling)
									{
										this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
									}
								}
								else if (this.Persona == PersonaType.Violent)
								{
									if (!this.Yandere.Attacking && !this.Yandere.Struggling && !this.Yandere.Dumping && !this.Yandere.SneakingShot && !this.StudentManager.PinningDown && !this.RespectEarned)
									{
										if (!this.Yandere.DelinquentFighting)
										{
											Debug.Log(this.Name + " is supposed to begin the combat minigame now.");
											this.SmartPhone.SetActive(false);
											this.Threatened = true;
											this.Fleeing = false;
											this.Alarmed = true;
											this.NoTalk = false;
											this.Patience = 0;
										}
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
									}
								}
								else if (this.Persona == PersonaType.Strict)
								{
									if (!this.WitnessedMurder)
									{
										if (this.ReportPhase == 0)
										{
											if (this.MyReporter.WitnessedMurder || this.MyReporter.WitnessedCorpse)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherReportReaction, 0, 3f);
												this.InvestigatingPossibleDeath = true;
											}
											else if (this.MyReporter.WitnessedLimb)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherReportReaction, 2, 3f);
											}
											else if (this.MyReporter.WitnessedBloodyWeapon)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherReportReaction, 3, 3f);
											}
											else if (this.MyReporter.WitnessedBloodPool)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherReportReaction, 1, 3f);
											}
											else if (this.MyReporter.WitnessedWeapon)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherReportReaction, 4, 3f);
											}
											this.ReportPhase++;
										}
										else if (this.ReportPhase == 1)
										{
											this.CharacterAnimation.CrossFade(this.IdleAnim);
											this.ReportTimer += Time.deltaTime;
											if (this.ReportTimer >= 3f)
											{
												StudentScript studentScript3 = null;
												if (this.MyReporter != null)
												{
													Debug.Log("MyReporter.WitnessedMurder is: " + this.MyReporter.WitnessedMurder.ToString());
													Debug.Log("MyReporter.WitnessedCorpse is: " + this.MyReporter.WitnessedCorpse.ToString());
													Debug.Log("MyReporter.WitnessedBloodPool is: " + this.MyReporter.WitnessedBloodPool.ToString());
													Debug.Log("MyReporter.WitnessedLimb is: " + this.MyReporter.WitnessedLimb.ToString());
													Debug.Log("MyReporter.WitnessedWeapon is: " + this.MyReporter.WitnessedWeapon.ToString());
													if (this.MyReporter.WitnessedMurder || this.MyReporter.WitnessedCorpse)
													{
														studentScript3 = this.StudentManager.Reporter;
													}
													else if (this.MyReporter.WitnessedBloodPool || this.MyReporter.WitnessedLimb || this.MyReporter.WitnessedWeapon)
													{
														Debug.Log("Assigning RelevantReporter here.");
														studentScript3 = this.StudentManager.BloodReporter;
													}
													if (this.MyReporter.WitnessedLimb)
													{
														this.InvestigatingPossibleLimb = true;
													}
													if (this.MyReporter.WitnessedBloodPool)
													{
														this.InvestigatingPossibleBlood = true;
													}
													if (studentScript3 == null)
													{
														Debug.Log("For some reason, RelevantReporter was null...");
													}
													if (!studentScript3.Teacher)
													{
														if (this.MyReporter.WitnessedMurder || this.MyReporter.WitnessedCorpse)
														{
															if (this.ExamineCorpseTarget == null)
															{
																Debug.Log(this.Name + "'s CurrentDestination is manually being forced to CorpseLocation.");
																this.StudentManager.Reporter.CurrentDestination = this.StudentManager.CorpseLocation;
																this.StudentManager.Reporter.Pathfinding.target = this.StudentManager.CorpseLocation;
																this.CurrentDestination = this.StudentManager.CorpseLocation;
																this.Pathfinding.target = this.StudentManager.CorpseLocation;
															}
															this.StudentManager.Reporter.TargetDistance = 2f;
														}
														else if (this.MyReporter.WitnessedBloodPool || this.MyReporter.WitnessedLimb || this.MyReporter.WitnessedWeapon)
														{
															Debug.Log("Setting BloodReporter's destination to BloodLocation.");
															this.StudentManager.BloodReporter.CurrentDestination = this.StudentManager.BloodLocation;
															this.StudentManager.BloodReporter.Pathfinding.target = this.StudentManager.BloodLocation;
															this.CurrentDestination = this.StudentManager.BloodLocation;
															this.Pathfinding.target = this.StudentManager.BloodLocation;
															this.StudentManager.BloodReporter.TargetDistance = 2f;
														}
													}
												}
												this.TargetDistance = 1f;
												this.ReportTimer = 0f;
												this.ReportPhase++;
											}
										}
										else if (this.ReportPhase == 2)
										{
											if (this.WitnessedCorpse)
											{
												Debug.Log("A teacher has just witnessed a corpse while on their way to investigate a student's report of a corpse.");
												this.DetermineCorpseLocation();
												if (!this.Corpse.Poisoned)
												{
													this.Subtitle.Speaker = this;
													this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 1, 5f);
												}
												else
												{
													this.Subtitle.Speaker = this;
													this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 2, 2f);
												}
												this.ReportPhase++;
											}
											else if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
											{
												Debug.Log("A teacher has just witnessed an alarming object while on their way to investigate a student's report - a " + this.BloodPool.name);
												this.DetermineBloodLocation();
												if (!this.VerballyReacted)
												{
													if (this.WitnessedLimb)
													{
														this.Subtitle.Speaker = this;
														this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 4, 5f);
													}
													else if (this.WitnessedBloodPool || this.WitnessedBloodyWeapon)
													{
														this.Subtitle.Speaker = this;
														this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 3, 5f);
													}
													else if (this.WitnessedWeapon)
													{
														this.Subtitle.Speaker = this;
														this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 5, 5f);
													}
												}
												PromptScript component = this.BloodPool.GetComponent<PromptScript>();
												WeaponScript component2 = this.BloodPool.GetComponent<WeaponScript>();
												bool flag7 = false;
												if (component2 != null)
												{
													if (component2.BroughtFromHome)
													{
														Debug.Log("This weapon was brought from home!");
														flag7 = true;
													}
													else
													{
														Debug.Log("This weapon was not brought from home.");
													}
												}
												if (component != null && !flag7)
												{
													Debug.Log("Disabling an object's prompt.");
													component.Hide();
													component.enabled = false;
													this.TargetDistance = 1.5f;
												}
												this.ReportPhase++;
											}
											else
											{
												this.CharacterAnimation.CrossFade(this.GuardAnim);
												if (this.BloodPool == null && this.StudentManager.Police.BloodParent.childCount > 0 && !this.InvestigatingPossibleLimb)
												{
													this.UpdateVisibleBlood();
												}
												this.ReportTimer += Time.deltaTime;
												if (this.ReportTimer > 5f)
												{
													this.Subtitle.Speaker = this;
													this.Subtitle.UpdateLabel(SubtitleType.TeacherPrankReaction, 1, 7f);
													this.ReportPhase = 98;
													this.ReportTimer = 0f;
												}
											}
										}
										else if (this.ReportPhase == 3)
										{
											if (this.WitnessedCorpse)
											{
												this.targetRotation = Quaternion.LookRotation(new Vector3(this.Corpse.AllColliders[0].transform.position.x, base.transform.position.y, this.Corpse.AllColliders[0].transform.position.z) - base.transform.position);
												base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
												this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
												if (!this.Male)
												{
													this.CharacterAnimation.CrossFade(this.InspectAnim);
												}
												else
												{
													this.CharacterAnimation.CrossFade(this.GuardAnim);
												}
											}
											else if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
											{
												if (this.BloodPool != null)
												{
													this.targetRotation = Quaternion.LookRotation(new Vector3(this.BloodPool.transform.position.x, base.transform.position.y, this.BloodPool.transform.position.z) - base.transform.position);
												}
												base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
												this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
												this.CharacterAnimation[this.InspectBloodAnim].speed = 0.66666f;
												this.CharacterAnimation.CrossFade(this.InspectBloodAnim);
											}
											this.ReportTimer += Time.deltaTime;
											if (this.ReportTimer >= 6f)
											{
												this.ReportTimer = 0f;
												if (this.WitnessedWeapon && !this.WitnessedBloodyWeapon)
												{
													this.ReportPhase = 7;
												}
												else
												{
													Debug.Log("A character is now checking whether or not they have a phone.");
													if (this.Teacher)
													{
														this.Phoneless = false;
													}
													if (!this.Phoneless)
													{
														Debug.Log("This character has a phone.");
														this.ReportPhase++;
													}
													else
													{
														Debug.Log("This character has no phone.");
														this.ReportPhase += 2;
													}
												}
											}
										}
										else if (this.ReportPhase == 4)
										{
											Debug.Log("A teacher is now supposed to be saying a line of dialogue about a corpse or blood.");
											if (this.WitnessedCorpse)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherPoliceReport, 0, 5f);
											}
											else if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
											{
												this.Subtitle.Speaker = this;
												this.Subtitle.UpdateLabel(SubtitleType.TeacherPoliceReport, 1, 5f);
											}
											if (!this.StudentManager.Eighties)
											{
												this.SmartPhone.transform.localPosition = new Vector3(-0.01f, -0.005f, -0.02f);
												this.SmartPhone.transform.localEulerAngles = new Vector3(-10f, -145f, 170f);
											}
											else
											{
												this.SmartPhone.transform.localPosition = new Vector3(0f, -0.022f, 0f);
												this.SmartPhone.transform.localEulerAngles = new Vector3(90f, 45f, 0f);
												this.SmartPhone.transform.localScale = new Vector3(66.66666f, 66.66666f, 66.66666f);
											}
											if (this.Teacher)
											{
												this.SmartPhone.SetActive(true);
											}
											else
											{
												this.SmartPhone.SetActive(false);
											}
											this.ReportPhase++;
										}
										else if (this.ReportPhase == 5)
										{
											Debug.Log("A teacher is now supposed to be performing a ''calling police'' animation.");
											if (this.Teacher)
											{
												this.CharacterAnimation.CrossFade(this.CallAnim);
											}
											else
											{
												this.Pathfinding.target = this.StudentManager.Exit;
												this.CurrentDestination = this.StudentManager.Exit;
											}
											this.ReportTimer += Time.deltaTime;
											if (this.ReportTimer >= 5f)
											{
												this.CharacterAnimation.CrossFade(this.GuardAnim);
												this.SmartPhone.SetActive(false);
												this.WitnessedBloodyWeapon = false;
												this.WitnessedBloodPool = false;
												this.WitnessedSomething = false;
												this.WitnessedWeapon = false;
												this.WitnessedLimb = false;
												this.IgnoringPettyActions = true;
												if (!this.StudentManager.Jammed)
												{
													Debug.Log(this.Name + " just called the cops.");
													this.Police.Called = true;
													this.Police.Show = true;
												}
												this.ReportTimer = 0f;
												this.Guarding = true;
												this.Alarmed = false;
												this.Fleeing = false;
												this.Reacted = false;
												this.ReportPhase++;
												if (this.MyReporter != null && this.MyReporter.ReportingBlood)
												{
													Debug.Log("The blood reporter has just been instructed to stop following the teacher.");
													this.MyReporter.ReportPhase++;
												}
											}
										}
										else if (this.ReportPhase == 6)
										{
											if (this.Corpse != null && this.Corpse.Concealed)
											{
												this.Alarm = 200f;
												this.Yandere.PotentiallyMurderousTimer = 1f;
												this.Witnessed = StudentWitnessType.Murder;
											}
										}
										else if (this.ReportPhase == 7)
										{
											if (this.StudentManager.BloodReporter != this)
											{
												this.StudentManager.BloodReporter = null;
											}
											this.StudentManager.UpdateStudents(0);
											this.BloodPool.GetComponent<WeaponScript>().Prompt.enabled = false;
											this.BloodPool.GetComponent<WeaponScript>().Prompt.Hide();
											this.BloodPool.GetComponent<WeaponScript>().enabled = false;
											Debug.Log("A WeaponScript has been disabled from this part of the code. 1");
											this.ReportPhase++;
										}
										else if (this.ReportPhase == 8)
										{
											this.CharacterAnimation.CrossFade("f02_teacherPickUp_00");
											if (this.CharacterAnimation["f02_teacherPickUp_00"].time >= 0.33333f)
											{
												this.Handkerchief.SetActive(true);
											}
											if (this.CharacterAnimation["f02_teacherPickUp_00"].time >= 2f)
											{
												this.BloodPool.parent = this.RightHand;
												this.BloodPool.localPosition = new Vector3(0f, 0f, 0f);
												this.BloodPool.localEulerAngles = new Vector3(0f, 0f, 0f);
												this.BloodPool.GetComponent<WeaponScript>().Returner = this;
											}
											if (this.CharacterAnimation["f02_teacherPickUp_00"].time >= this.CharacterAnimation["f02_teacherPickUp_00"].length)
											{
												this.CurrentDestination = this.StudentManager.WeaponBoxSpot;
												this.Pathfinding.target = this.StudentManager.WeaponBoxSpot;
												this.Pathfinding.speed = this.WalkSpeed;
												this.Hurry = false;
												this.ReportPhase++;
												if (this.MyReporter != null)
												{
													Debug.Log("Telling reporter to go back to their normal routine.");
													this.MyReporter.CurrentDestination = this.MyReporter.Destinations[this.MyReporter.Phase];
													this.MyReporter.Pathfinding.target = this.MyReporter.Destinations[this.MyReporter.Phase];
													this.MyReporter.Pathfinding.speed = 1f;
													this.MyReporter.ReportTimer = 0f;
													this.MyReporter.AlarmTimer = 0f;
													this.MyReporter.TargetDistance = 1f;
													this.MyReporter.ReportPhase = 0;
													this.MyReporter.WitnessedSomething = false;
													this.MyReporter.WitnessedWeapon = false;
													this.MyReporter.Distracted = false;
													this.MyReporter.Reacted = false;
													this.MyReporter.Alarmed = false;
													this.MyReporter.Fleeing = false;
													this.MyReporter.Routine = true;
													this.MyReporter.Halt = false;
													this.MyReporter.Persona = this.MyReporter.OriginalPersona;
													this.MyReporter.BloodPool = null;
													if (this.MyReporter.Club == ClubType.Council)
													{
														this.MyReporter.Persona = PersonaType.Dangerous;
													}
													this.ID = 0;
													while (this.ID < this.MyReporter.Outlines.Length)
													{
														if (this.MyReporter.Outlines[this.ID] != null)
														{
															this.MyReporter.Outlines[this.ID].color = new Color(1f, 1f, 0f, 1f);
														}
														this.ID++;
													}
													if (this.MyReporter.BeforeReturnAnim != "")
													{
														this.MyReporter.WalkAnim = this.MyReporter.BeforeReturnAnim;
													}
												}
											}
										}
										else if (this.ReportPhase == 9)
										{
											this.DropWeaponInBox();
											this.CharacterAnimation.CrossFade(this.RunAnim);
											this.CurrentDestination = this.Destinations[this.Phase];
											this.Pathfinding.target = this.Destinations[this.Phase];
											this.Pathfinding.canSearch = true;
											this.Pathfinding.canMove = true;
											this.Pathfinding.speed = this.WalkSpeed;
											this.WitnessedSomething = false;
											this.VerballyReacted = false;
											this.WitnessedWeapon = false;
											this.YandereInnocent = false;
											this.ReportingBlood = false;
											this.Distracted = false;
											this.Alarmed = false;
											this.Fleeing = false;
											this.Routine = true;
											this.Halt = false;
											this.ReportTimer = 0f;
											this.ReportPhase = 0;
										}
										else if (this.ReportPhase == 98)
										{
											this.CharacterAnimation.CrossFade(this.IdleAnim);
											this.targetRotation = Quaternion.LookRotation(this.MyReporter.transform.position - base.transform.position);
											base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
											this.ReportTimer += Time.deltaTime;
											if (this.ReportTimer > 7f)
											{
												this.ReportPhase++;
											}
										}
										else if (this.ReportPhase == 99)
										{
											this.CharacterAnimation.CrossFade(this.RunAnim);
											this.CurrentDestination = this.Destinations[this.Phase];
											this.Pathfinding.target = this.Destinations[this.Phase];
											this.Pathfinding.canSearch = true;
											this.Pathfinding.canMove = true;
											this.MyReporter.Persona = PersonaType.TeachersPet;
											this.MyReporter.ReportPhase = 100;
											this.MyReporter.Fleeing = true;
											this.ReportTimer = 0f;
											this.ReportPhase = 0;
											this.InvestigatingPossibleBlood = false;
											this.InvestigatingPossibleDeath = false;
											this.InvestigatingPossibleLimb = false;
											this.Alarmed = false;
											this.Fleeing = false;
											this.Routine = true;
										}
									}
									else if (!this.Yandere.Dumping && !this.Yandere.Attacking && !this.Yandere.DelinquentFighting)
									{
										bool flag8 = false;
										if (this.Yandere.Class.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0)
										{
											flag8 = true;
										}
										Debug.Log("StudentManager.ChallengeManager.InvincibleRaibaru is: " + this.StudentManager.ChallengeManager.InvincibleRaibaru.ToString());
										if (this.Strength == 9 && this.StudentManager.ChallengeManager.InvincibleRaibaru)
										{
											flag8 = false;
										}
										if ((this.Yandere.Armed && flag8 && this.Yandere.EquippedWeapon.Type == WeaponType.Knife) || (this.Yandere.Club == ClubType.MartialArts && this.Yandere.Armed && this.Yandere.EquippedWeapon.Type == WeaponType.Knife) || (flag8 && this.Yandere.Weapon[1] != null && this.Yandere.Weapon[1].Type == WeaponType.Knife && !this.Yandere.Weapon[1].Broken) || (flag8 && this.Yandere.Weapon[2] != null && this.Yandere.Weapon[2].Type == WeaponType.Knife && !this.Yandere.Weapon[2].Broken))
										{
											Debug.Log("Yandere-chan is in a state that allows her to enter struggles with teachers, so this teacher is changing into the ''Heroic'' Persona to have a struggle.");
											this.Persona = PersonaType.Heroic;
										}
										else
										{
											Debug.Log("A teacher is taking down Yandere-chan.");
											if (this.Yandere.Aiming)
											{
												this.Yandere.StopAiming();
											}
											this.Yandere.Mopping = false;
											this.Yandere.EmptyHands();
											this.AttackReaction();
											this.CharacterAnimation[this.CounterAnim].time = 5f;
											this.Yandere.CharacterAnimation["f02_teacherCounterA_00"].time = 5f;
											this.Yandere.ShoulderCamera.Timer = 5f;
											this.Yandere.ShoulderCamera.Phase = 3;
											this.Police.Show = false;
											this.CheckForKnifeInInventory();
											if ((this.Yandere.Armed && this.Yandere.Class.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0 && this.Yandere.EquippedWeapon.Type == WeaponType.Knife) || (this.Yandere.Club == ClubType.MartialArts && this.Yandere.Armed && this.Yandere.EquippedWeapon.Type == WeaponType.Knife))
											{
												Debug.Log("Ayano is able to counter this teacher.");
											}
											else
											{
												Debug.Log("A game over will now occur.");
												this.Yandere.CameraEffects.MurderWitnessed();
												this.Yandere.Jukebox.GameOver();
											}
										}
									}
									else
									{
										this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
									}
								}
								else if (this.Persona == PersonaType.LandlineUser)
								{
									if (this.ReportPhase == 1)
									{
										if (this.StudentManager.Reporter == null && !this.Police.Called)
										{
											this.Subtitle.UpdateLabel(SubtitleType.SocialReport, 1, 5f);
											this.CharacterAnimation.CrossFade(this.LandLineAnim);
											this.StudentManager.Reporter = this;
											this.SpawnAlarmDisc();
											this.ReportPhase++;
										}
										else
										{
											this.CharacterAnimation.CrossFade(this.ParanoidAnim);
											this.ReportPhase += 2;
											this.Halt = true;
										}
									}
									else if (this.ReportPhase == 2)
									{
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Pathfinding.target.rotation, 10f * Time.deltaTime);
										this.StudentManager.LandLinePhone.SetBlendShapeWeight(1, this.ReportTimer * 200f);
										this.ReportTimer += Time.deltaTime;
										if (this.ReportTimer > 5f)
										{
											if (this.StudentManager.Reporter == this)
											{
												this.StudentManager.LandLinePhone.SetBlendShapeWeight(1, 0f);
												if (!this.StudentManager.Jammed)
												{
													Debug.Log(this.Name + " just called the cops.");
													this.Police.Called = true;
													this.Police.Show = true;
												}
											}
											UnityEngine.Object.Instantiate<GameObject>(this.EnterGuardStateCollider, base.transform.position, Quaternion.identity);
											this.CharacterAnimation.CrossFade(this.ParanoidAnim);
											this.ReportPhase++;
										}
									}
									else if (this.ReportPhase == 3)
									{
										if (this.WitnessedMurder && (!this.SawMask || (this.SawMask && this.Yandere.Mask != null)))
										{
											this.LookForYandere();
										}
									}
									else if (this.ReportPhase == 4)
									{
										this.CharacterAnimation.CrossFade(this.SocialFearAnim);
										this.Subtitle.UpdateLabel(SubtitleType.SocialFear, 1, 5f);
										this.SpawnAlarmDisc();
										this.ReportPhase++;
										this.Halt = true;
									}
									else if (this.ReportPhase == 5)
									{
										this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
										if (this.Yandere.Attacking)
										{
											this.LookForYandere();
										}
									}
									else if (this.ReportPhase == 6)
									{
										this.CharacterAnimation.CrossFade(this.SocialTerrorAnim);
										this.Subtitle.UpdateLabel(SubtitleType.SocialTerror, 1, 5f);
										this.VisionDistance = 0f;
										this.SpawnAlarmDisc();
										this.ReportPhase++;
									}
								}
							}
							if (this.Persona == PersonaType.Strict && this.BloodPool != null && this.BloodPool.parent == this.Yandere.RightHand)
							{
								Debug.Log("Yandere-chan picked up the weapon that the teacher was investigating!");
								this.WitnessedBloodyWeapon = false;
								this.WitnessedBloodPool = false;
								this.WitnessedSomething = false;
								this.WitnessedCorpse = false;
								this.WitnessedMurder = false;
								this.WitnessedWeapon = false;
								this.WitnessedLimb = false;
								this.YandereVisible = true;
								this.ReportTimer = 0f;
								this.BloodPool = null;
								this.ReportPhase = 0;
								this.Alarmed = false;
								this.Fleeing = false;
								this.Routine = true;
								this.Reacted = false;
								this.AlarmTimer = 0f;
								this.Alarm = 200f;
								this.BecomeAlarmed();
							}
						}
					}
					else if (this.PinPhase == 0)
					{
						if (this.DistanceToDestination < 1f)
						{
							if (this.Pathfinding.canSearch)
							{
								this.Pathfinding.canSearch = false;
								this.Pathfinding.canMove = false;
							}
							this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
							base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
							this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
							this.MoveTowardsTarget(this.CurrentDestination.position);
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.SprintAnim);
							if (!this.Pathfinding.canSearch)
							{
								this.Pathfinding.canSearch = true;
								this.Pathfinding.canMove = true;
							}
						}
					}
					else if (this.PinDownID > 0)
					{
						this.CharacterAnimation.CrossFade(this.PinDownAnim);
						this.CurrentDestination = this.StudentManager.PinDownSpots[this.PinDownID];
						this.Pathfinding.target = this.StudentManager.PinDownSpots[this.PinDownID];
						this.MoveTowardsTarget(this.CurrentDestination.position);
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
					}
				}
			}
			if (this.Following && !this.Waiting)
			{
				this.DistanceToDestination = Vector3.Distance(base.transform.position, this.Pathfinding.target.position);
				if (this.DistanceToDestination > 2f)
				{
					this.FollowCountdown.Sprite.fillAmount = Mathf.MoveTowards(this.FollowCountdown.Sprite.fillAmount, 0f, Time.deltaTime * 0.01f);
					this.CharacterAnimation.CrossFade(this.RunAnim);
					this.Pathfinding.speed = 4f;
					this.Obstacle.enabled = false;
				}
				else if (this.DistanceToDestination > 1f)
				{
					this.FollowCountdown.Sprite.fillAmount = Mathf.MoveTowards(this.FollowCountdown.Sprite.fillAmount, 0f, Time.deltaTime * 0.01f);
					this.CharacterAnimation.CrossFade(this.OriginalWalkAnim);
					this.Pathfinding.canMove = true;
					this.Obstacle.enabled = false;
					this.Pathfinding.speed = this.WalkSpeed;
				}
				else
				{
					if (!this.Yandere.Succubus)
					{
						if (this.StudentManager.TranqArea.bounds.Contains(this.Yandere.transform.position))
						{
							this.FollowCountdown.Sprite.fillAmount = Mathf.MoveTowards(this.FollowCountdown.Sprite.fillAmount, 0f, Time.deltaTime * 0.01f);
						}
						else
						{
							this.FollowCountdown.Sprite.fillAmount = Mathf.MoveTowards(this.FollowCountdown.Sprite.fillAmount, 0f, Time.deltaTime * 0.1f);
						}
					}
					this.CharacterAnimation.CrossFade(this.IdleAnim);
					this.Pathfinding.canMove = false;
					this.Obstacle.enabled = true;
				}
				if (this.Phase < this.ScheduleBlocks.Length && !this.Yandere.Succubus && (this.FollowCountdown.Sprite.fillAmount == 0f || this.Clock.HourTime >= this.ScheduleBlocks[this.Phase].time || this.StudentManager.LockerRoomArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.IncineratorArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.HeadmasterArea.bounds.Contains(this.Yandere.transform.position) || this.Yandere.Class.Portal.Transition || this.Yandere.TimeSkipping || this.Yandere.Trespassing))
				{
					Debug.Log("This student will now stop following Ayano.");
					if (this.Clock.HourTime >= this.ScheduleBlocks[this.Phase].time)
					{
						this.Phase++;
					}
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.Hearts.emission.enabled = false;
					this.FollowCountdown.gameObject.SetActive(false);
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.Yandere.Follower = null;
					this.Yandere.Followers--;
					this.Following = false;
					this.Routine = true;
					this.Pathfinding.speed = this.WalkSpeed;
					if (this.StudentManager.LockerRoomArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.IncineratorArea.bounds.Contains(this.Yandere.transform.position) || this.StudentManager.HeadmasterArea.bounds.Contains(this.Yandere.transform.position) || this.Yandere.Trespassing)
					{
						this.Subtitle.UpdateLabel(SubtitleType.StopFollowApology, 1, 3f);
					}
					else if (this.Yandere.TimeSkipping)
					{
						this.Subtitle.CustomText = "If you're just going to stand there spacing out, I'm leaving...";
						this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 10f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.StopFollowApology, 0, 3f);
					}
					if (!this.StudentManager.Eighties && this.StudentID == 41)
					{
						this.Subtitle.CustomText = "I'm leaving.";
						this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					}
					this.Prompt.Label[0].text = "     Talk";
				}
			}
			if (this.Wet)
			{
				if (this.DistanceToDestination < this.TargetDistance)
				{
					if (!this.Splashed)
					{
						if (!this.InDarkness)
						{
							if (this.NotActuallyWet && this.BathePhase < 5)
							{
								this.BathePhase = 5;
							}
							if (this.BathePhase == 1)
							{
								if (this.StudentManager.CommunalLocker.Student == null)
								{
									this.CharacterAnimation[this.WetAnim].weight = 0f;
									this.StudentManager.CommunalLocker.Open = true;
									this.StudentManager.CommunalLocker.Student = this;
									this.StudentManager.CommunalLocker.SpawnSteam();
									this.Pathfinding.speed = this.WalkSpeed;
									this.Schoolwear = 0;
									this.BathePhase++;
									this.Distracted = true;
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
									this.Pathfinding.canSearch = false;
									this.Pathfinding.canMove = false;
								}
							}
							else if (this.BathePhase == 2)
							{
								base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
								this.MoveTowardsTarget(this.CurrentDestination.position);
								if (this.Club == ClubType.Cooking && this.ApronAttacher.newRenderer.enabled)
								{
									this.ApronAttacher.newRenderer.enabled = false;
								}
							}
							else if (this.BathePhase == 3)
							{
								this.StudentManager.CommunalLocker.Open = false;
								this.CharacterAnimation.CrossFade(this.WalkAnim);
								if (!this.BatheFast)
								{
									if (!this.Male)
									{
										this.CurrentDestination = this.StudentManager.FemaleBatheSpot;
										this.Pathfinding.target = this.StudentManager.FemaleBatheSpot;
									}
									else
									{
										this.CurrentDestination = this.StudentManager.MaleBatheSpot;
										this.Pathfinding.target = this.StudentManager.MaleBatheSpot;
									}
								}
								else if (!this.Male)
								{
									this.CurrentDestination = this.StudentManager.FastBatheSpot;
									this.Pathfinding.target = this.StudentManager.FastBatheSpot;
								}
								else
								{
									this.CurrentDestination = this.StudentManager.MaleBatheSpot;
									this.Pathfinding.target = this.StudentManager.MaleBatheSpot;
								}
								this.Pathfinding.canSearch = true;
								this.Pathfinding.canMove = true;
								this.BathePhase++;
							}
							else if (this.BathePhase == 4)
							{
								if (!this.Male)
								{
									this.StudentManager.OpenValue = Mathf.Lerp(this.StudentManager.OpenValue, 0f, Time.deltaTime * 10f);
									this.StudentManager.FemaleShowerCurtain.SetBlendShapeWeight(1, this.StudentManager.OpenValue);
								}
								base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
								this.MoveTowardsTarget(this.CurrentDestination.position);
								this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
								this.CharacterAnimation.CrossFade(this.BathingAnim);
								if (this.CharacterAnimation[this.BathingAnim].time >= this.CharacterAnimation[this.BathingAnim].length)
								{
									this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
									if (!this.Male)
									{
										this.StudentManager.OpenCurtain = true;
										this.HorudaCollider.gameObject.SetActive(false);
									}
									this.LiquidProjector.enabled = false;
									this.Bloody = false;
									this.BathePhase++;
									this.Gas = false;
									this.GoChange();
									this.UnWet();
								}
							}
							else if (this.BathePhase == 5)
							{
								if (this.StudentManager.CommunalLocker.Student == null)
								{
									this.StudentManager.CommunalLocker.Open = true;
									this.StudentManager.CommunalLocker.Student = this;
									this.StudentManager.CommunalLocker.SpawnSteam();
									this.Schoolwear = (this.InEvent ? 1 : 3);
									if (this.NotActuallyWet)
									{
										this.Schoolwear = 1;
										if (this.Follower != null && this.Follower.FollowTarget != null && this.Follower.CurrentAction == StudentActionType.Sunbathe)
										{
											this.Follower.Schoolwear = 1;
											this.Follower.ChangeSchoolwear();
											this.Follower.CurrentAction = StudentActionType.Follow;
										}
									}
									Debug.Log("Time to decide if a special case applies to this character.");
									if (this.Club == ClubType.Sports && this.Clock.Period > 5 && !this.StudentManager.PoolClosed)
									{
										Debug.Log("Sports Club special case! Swimsuit!");
										this.Schoolwear = 2;
									}
									this.BathePhase++;
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.IdleAnim);
									this.Pathfinding.canSearch = false;
									this.Pathfinding.canMove = false;
								}
							}
							else if (this.BathePhase == 6)
							{
								base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
								this.MoveTowardsTarget(this.CurrentDestination.position);
								if (this.Club == ClubType.Cooking && !this.ApronAttacher.newRenderer.enabled)
								{
									this.ApronAttacher.newRenderer.enabled = true;
									Debug.Log("We are being told to re-enable this apron attacher...");
								}
								this.BatheTimer += Time.deltaTime;
								if (this.BatheTimer > 5f)
								{
									Debug.Log("Something went wrong. This student didn't change clothing like they were supposed to.");
									this.BatheTimer = 0f;
									this.BathePhase--;
								}
							}
							else if (this.BathePhase == 7)
							{
								this.BatheTimer = 0f;
								if (!this.Yandere.Inventory.Ring)
								{
									this.StudentManager.CommunalLocker.Rings.gameObject.SetActive(false);
								}
								if (this.StudentManager.CommunalLocker.RivalPhone.Stolen && this.Yandere.Inventory.RivalPhoneID == this.StudentID)
								{
									this.CharacterAnimation.CrossFade("f02_losingPhone_00");
									this.Subtitle.UpdateLabel(this.LostPhoneSubtitleType, 1, 5f);
									this.RealizePhoneIsMissing();
									this.Phoneless = true;
									this.BatheTimer = this.CharacterAnimation["f02_losingPhone_00"].length;
									this.BathePhase++;
								}
								else
								{
									this.StudentManager.CommunalLocker.RivalPhone.gameObject.SetActive(false);
									this.BathePhase++;
								}
							}
							else if (this.BathePhase == 8)
							{
								if (this.BatheTimer == 0f)
								{
									this.BathePhase++;
								}
								else
								{
									this.BatheTimer = Mathf.MoveTowards(this.BatheTimer, 0f, Time.deltaTime);
								}
							}
							else if (this.BathePhase == 9)
							{
								if ((this.StudentManager.Eighties && this.StudentID == 30 && this.StudentManager.CommunalLocker.Rings.Stolen) || (!this.StudentManager.Eighties && this.StudentID == 2 && this.StudentManager.CommunalLocker.Rings.Stolen))
								{
									this.CharacterAnimation.CrossFade("f02_losingPhone_00");
									if (this.StudentManager.Eighties)
									{
										this.Subtitle.CustomText = "Huh? One of my rings is missing...did someone steal it?!";
									}
									else
									{
										this.Subtitle.CustomText = "Huh? My ring is missing...did someone take it?";
									}
									this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
									this.Depressed = true;
									this.BatheTimer = this.CharacterAnimation["f02_losingPhone_00"].length;
									this.BathePhase++;
								}
								else
								{
									if (!this.StudentManager.Eighties && this.StudentID == 2)
									{
										this.Cosmetic.FemaleAccessories[this.Cosmetic.Accessory].SetActive(true);
									}
									else if (this.StudentManager.Eighties && this.StudentID == 30)
									{
										this.Cosmetic.EnableRings();
									}
									this.BathePhase++;
								}
							}
							else if (this.BathePhase == 10)
							{
								if (this.BatheTimer == 0f)
								{
									this.BathePhase++;
								}
								else
								{
									this.BatheTimer = Mathf.MoveTowards(this.BatheTimer, 0f, Time.deltaTime);
								}
							}
							else if (this.BathePhase == 11)
							{
								this.CharacterAnimation[this.WetAnim].weight = 0f;
								if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
								{
									this.SmartPhone.SetActive(true);
								}
								else
								{
									this.WalkAnim = this.OriginalOriginalWalkAnim;
									this.RunAnim = this.OriginalSprintAnim;
									this.IdleAnim = this.OriginalIdleAnim;
								}
								this.StudentManager.CommunalLocker.Student = null;
								this.StudentManager.CommunalLocker.Open = false;
								if (this.Phase == 1)
								{
									this.Phase++;
								}
								if (this.Club == ClubType.Sports && this.Clock.Period > 4 && !this.StudentManager.PoolClosed)
								{
									this.ChangeClubwear();
									this.DressCode = true;
								}
								this.CurrentDestination = this.Destinations[this.Phase];
								this.Pathfinding.target = this.Destinations[this.Phase];
								this.Pathfinding.canSearch = true;
								this.Pathfinding.canMove = true;
								this.DistanceToDestination = 100f;
								this.Routine = true;
								this.Wet = false;
								if (this.FleeWhenClean)
								{
									this.CurrentDestination = this.StudentManager.Exit;
									this.Pathfinding.target = this.StudentManager.Exit;
									this.TargetDistance = 0f;
									this.Routine = false;
									this.Fleeing = true;
								}
								else
								{
									this.Hurry = false;
								}
								if (!this.StudentManager.Eighties && this.Phoneless)
								{
									this.SprintAnim = this.OriginalOriginalSprintAnim;
									this.RunAnim = this.OriginalOriginalSprintAnim;
									this.Pathfinding.speed = 4f;
									this.Hurry = true;
								}
								if (this.CurrentAction == StudentActionType.PhotoShoot || this.CurrentAction == StudentActionType.GravurePose)
								{
									this.Hurry = false;
								}
							}
						}
						else if (this.BathePhase == -1)
						{
							this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
							this.Subtitle.UpdateLabel(SubtitleType.LightSwitchReaction, 2, 5f);
							this.CharacterAnimation.CrossFade("f02_electrocution_00");
							this.Pathfinding.canSearch = false;
							this.Pathfinding.canMove = false;
							this.Distracted = true;
							this.BathePhase++;
						}
						else if (this.BathePhase == 0)
						{
							base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
							this.MoveTowardsTarget(this.CurrentDestination.position);
							if (this.CharacterAnimation["f02_electrocution_00"].time > 2f && this.CharacterAnimation["f02_electrocution_00"].time < 5.9500003f)
							{
								if (!this.LightSwitch.Panel.useGravity)
								{
									if (!this.Bloody)
									{
										this.Subtitle.Speaker = this;
										this.Subtitle.UpdateLabel(this.SplashSubtitleType, 2, 5f);
									}
									else
									{
										this.Subtitle.Speaker = this;
										this.Subtitle.UpdateLabel(this.SplashSubtitleType, 4, 5f);
									}
									this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
									this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
									Debug.Log("Sprinting becasue bathing.");
									this.Pathfinding.canSearch = true;
									this.Pathfinding.canMove = true;
									this.Pathfinding.speed = 4f;
									this.CharacterAnimation.CrossFade(this.WalkAnim);
									this.BathePhase++;
									this.LightSwitch.Prompt.Label[0].text = "     Turn Off";
									this.LightSwitch.BathroomLight.SetActive(true);
									this.LightSwitch.GetComponent<AudioSource>().clip = this.LightSwitch.Flick[0];
									this.LightSwitch.GetComponent<AudioSource>().Play();
									this.InDarkness = false;
								}
								else
								{
									if (!this.LightSwitch.Flicker)
									{
										this.CharacterAnimation["f02_electrocution_00"].speed = 0.85f;
										GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(this.LightSwitch.Electricity, base.transform.position, Quaternion.identity);
										gameObject5.transform.parent = this.Bones[1].transform;
										gameObject5.transform.localPosition = Vector3.zero;
										this.Subtitle.UpdateLabel(SubtitleType.LightSwitchReaction, 3, 0f);
										this.LightSwitch.GetComponent<AudioSource>().clip = this.LightSwitch.Flick[2];
										this.LightSwitch.Flicker = true;
										this.LightSwitch.GetComponent<AudioSource>().Play();
										this.EyeShrink = 1f;
										this.ElectroSteam[0].SetActive(true);
										this.ElectroSteam[1].SetActive(true);
										this.ElectroSteam[2].SetActive(true);
										this.ElectroSteam[3].SetActive(true);
									}
									this.RightDrill.eulerAngles = new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
									this.LeftDrill.eulerAngles = new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
									this.ElectroTimer += Time.deltaTime;
									if (this.ElectroTimer > 0.1f)
									{
										this.ElectroTimer = 0f;
										if (this.MyRenderer.enabled)
										{
											this.Spook();
										}
										else
										{
											this.Unspook();
										}
									}
								}
							}
							else if (this.CharacterAnimation["f02_electrocution_00"].time > 5.9500003f && this.CharacterAnimation["f02_electrocution_00"].time < this.CharacterAnimation["f02_electrocution_00"].length)
							{
								if (this.LightSwitch.Flicker)
								{
									this.CharacterAnimation["f02_electrocution_00"].speed = 1f;
									this.Prompt.Label[0].text = "     Turn Off";
									this.LightSwitch.BathroomLight.SetActive(true);
									this.RightDrill.localEulerAngles = new Vector3(0f, 0f, 68.30447f);
									this.LeftDrill.localEulerAngles = new Vector3(0f, -180f, -159.417f);
									this.LightSwitch.Flicker = false;
									this.Unspook();
									this.UnWet();
								}
							}
							else if (this.CharacterAnimation["f02_electrocution_00"].time >= this.CharacterAnimation["f02_electrocution_00"].length)
							{
								this.Police.ElectrocutedStudentName = this.Name;
								this.Police.ElectroScene = true;
								this.Electrocuted = true;
								this.BecomeRagdoll();
								this.DeathType = DeathType.Electrocution;
							}
						}
					}
				}
				else if (this.Pathfinding.canMove)
				{
					if (this.BathePhase == 1 || this.FleeWhenClean)
					{
						if (!this.NotActuallyWet)
						{
							if (!this.WitnessedCorpse)
							{
								this.CharacterAnimation[this.WetAnim].weight = 1f;
								this.CharacterAnimation.Play(this.WetAnim);
							}
							this.Pathfinding.speed = 4f;
							if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
							{
								this.CharacterAnimation.CrossFade(this.OriginalSprintAnim);
							}
							else
							{
								this.CharacterAnimation.CrossFade(this.SprintAnim);
							}
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
						}
					}
					else
					{
						if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
						{
							this.CharacterAnimation.CrossFade(this.OriginalWalkAnim);
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
						}
						this.Pathfinding.speed = this.WalkSpeed;
					}
				}
			}
			if (this.Distracting)
			{
				if (this.DistractionTarget == null)
				{
					this.Distracting = false;
				}
				else if (this.DistractionTarget.Dying)
				{
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.DistractionTarget.TargetedForDistraction = false;
					this.DistractionTarget.Distracted = false;
					this.DistractionTarget.EmptyHands();
					this.Pathfinding.speed = this.WalkSpeed;
					this.Distracting = false;
					this.Distracted = false;
					this.CanTalk = true;
					this.Routine = true;
				}
				else
				{
					if (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Cooking && this.ClubActivityPhase > 0 && this.DistractionTarget.InEvent)
					{
						this.GetFoodTarget();
					}
					if (this.DistanceToDestination < 5f || this.DistractionTarget.Leaving)
					{
						if (this.DistractionTarget.HelpOffered || this.DistractionTarget.InEvent || this.DistractionTarget.Talking || this.DistractionTarget.Following || this.DistractionTarget.TurnOffRadio || this.DistractionTarget.Splashed || this.DistractionTarget.Shoving || this.DistractionTarget.Spraying || this.DistractionTarget.FocusOnYandere || this.DistractionTarget.ShoeRemoval.enabled || this.DistractionTarget.Posing || this.DistractionTarget.ClubActivityPhase >= 16 || !this.DistractionTarget.enabled || this.DistractionTarget.Alarmed || this.DistractionTarget.Fleeing || this.DistractionTarget.Wet || this.DistractionTarget.EatingSnack || this.DistractionTarget.MyBento.Tampered || this.DistractionTarget.Meeting || this.DistractionTarget.Sedated || this.DistractionTarget.Sleepy || this.DistractionTarget.InvestigatingBloodPool || this.DistractionTarget.ReturningMisplacedWeapon || this.StudentManager.LockerRoomArea.bounds.Contains(this.DistractionTarget.transform.position) || this.StudentManager.MaleLockerRoomArea.bounds.Contains(this.DistractionTarget.transform.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.DistractionTarget.transform.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.DistractionTarget.transform.position) || this.StudentManager.HeadmasterArea.bounds.Contains(this.DistractionTarget.transform.position) || (this.DistractionTarget.Actions[this.DistractionTarget.Phase] == StudentActionType.Bully && this.DistractionTarget.DistanceToDestination < 1f) || (this.DistractionTarget.Leaving || this.DistractionTarget.CameraReacting || this.DistractionTarget.SentToLocker || (this.MyPlate != null && this.MyPlate.parent == this.RightHand && this.DistractionTarget.AlreadyFed)) || this.DistractionTarget.AwareOfCorpse)
						{
							if (this.DistractionTarget.InEvent)
							{
								this.Yandere.NotificationManager.CustomText = this.Name + " is backing off.";
								this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
								this.Yandere.NotificationManager.CustomText = this.DistractionTarget.Name + " is busy.";
								this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
								this.Yandere.NotificationManager.CustomText = this.Name + " can see that";
								this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
							}
							this.CurrentDestination = this.Destinations[this.Phase];
							this.Pathfinding.target = this.Destinations[this.Phase];
							this.DistractionTarget.TargetedForDistraction = false;
							this.Distracting = false;
							this.Distracted = false;
							this.SpeechLines.Stop();
							this.CanTalk = true;
							this.Routine = true;
							this.Pathfinding.speed = this.WalkSpeed;
							if (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Cooking && this.ClubActivityPhase > 0)
							{
								this.GetFoodTarget();
							}
						}
						else if (this.DistanceToDestination < this.TargetDistance)
						{
							if (!this.DistractionTarget.Distracted)
							{
								if (this.StudentID > 1 && this.DistractionTarget.StudentID > 1 && this.Persona != PersonaType.Fragile && this.DistractionTarget.Persona != PersonaType.Fragile && ((this.Club != ClubType.Bully && this.DistractionTarget.Club == ClubType.Bully) || (this.Club == ClubType.Bully && this.DistractionTarget.Club != ClubType.Bully)))
								{
									this.BullyPhotoCollider.SetActive(true);
								}
								if (this.DistractionTarget.Investigating)
								{
									this.DistractionTarget.StopInvestigating();
								}
								this.StudentManager.UpdateStudents(this.DistractionTarget.StudentID);
								this.DistractionTarget.Pathfinding.canSearch = false;
								this.DistractionTarget.Pathfinding.canMove = false;
								this.DistractionTarget.OccultBook.SetActive(false);
								this.DistractionTarget.SmartPhone.SetActive(false);
								this.DistractionTarget.Distraction = base.transform;
								this.DistractionTarget.CameraReacting = false;
								this.DistractionTarget.Pathfinding.speed = 0f;
								this.DistractionTarget.Pen.SetActive(false);
								this.DistractionTarget.Drownable = false;
								this.DistractionTarget.Distracted = true;
								this.DistractionTarget.Pushable = false;
								this.DistractionTarget.Routine = false;
								this.DistractionTarget.CanTalk = false;
								this.DistractionTarget.ReadPhase = 0;
								this.DistractionTarget.SpeechLines.Stop();
								this.DistractionTarget.ChalkDust.Stop();
								this.DistractionTarget.CleanTimer = 0f;
								this.DistractionTarget.EmptyHands();
								this.DistractionTarget.Distractor = this;
								this.Pathfinding.speed = 0f;
								this.Distracted = true;
							}
							this.targetRotation = Quaternion.LookRotation(new Vector3(this.DistractionTarget.transform.position.x, base.transform.position.y, this.DistractionTarget.transform.position.z) - base.transform.position);
							base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
							if (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Cooking && this.ClubActivityPhase > 0)
							{
								this.CharacterAnimation.CrossFade(this.IdleAnim);
							}
							else
							{
								this.DistractionTarget.SpeechLines.Play();
								this.SpeechLines.Play();
								this.CharacterAnimation.CrossFade(this.RandomAnim);
								if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
								{
									this.PickRandomAnim();
								}
							}
							this.DistractTimer -= Time.deltaTime;
							if (this.DistractTimer <= 0f)
							{
								if (this.DistractionTarget.SunbathePhase == 0)
								{
									this.DistractionTarget.CurrentDestination = this.DistractionTarget.Destinations[this.DistractionTarget.Phase];
									this.DistractionTarget.Pathfinding.target = this.DistractionTarget.Destinations[this.DistractionTarget.Phase];
								}
								else
								{
									this.DistractionTarget.CurrentDestination = this.StudentManager.SunbatheSpots[this.DistractionTarget.StudentID];
									this.DistractionTarget.Pathfinding.target = this.StudentManager.SunbatheSpots[this.DistractionTarget.StudentID];
									Debug.Log(this.DistractionTarget.Name + " was sunbathing at the time of being distracted, and should be returning to their sunbathing spot now.");
									this.DistractionTarget.SunbathePhase = 2;
								}
								this.DistractionTarget.TargetedForDistraction = false;
								this.DistractionTarget.Pathfinding.canSearch = true;
								this.DistractionTarget.Pathfinding.canMove = true;
								this.DistractionTarget.Pathfinding.speed = 1f;
								this.DistractionTarget.Octodog.SetActive(false);
								this.DistractionTarget.Distraction = null;
								this.DistractionTarget.Distracted = false;
								this.DistractionTarget.CanTalk = true;
								this.DistractionTarget.Routine = true;
								this.DistractionTarget.Hurry = false;
								this.DistractionTarget.EquipCleaningItems();
								this.DistractionTarget.EatingSnack = false;
								this.Private = false;
								if (this.DistractionTarget.Persona == PersonaType.PhoneAddict)
								{
									this.DistractionTarget.SmartPhone.SetActive(true);
								}
								this.DistractionTarget.Distractor = null;
								this.DistractionTarget.SpeechLines.Stop();
								this.SpeechLines.Stop();
								this.CurrentDestination = this.Destinations[this.Phase];
								this.Pathfinding.target = this.Destinations[this.Phase];
								this.BullyPhotoCollider.SetActive(false);
								this.Pathfinding.speed = this.WalkSpeed;
								this.Distracting = false;
								this.Distracted = false;
								this.CanTalk = true;
								this.Routine = true;
								this.Hurry = false;
								if (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Cooking && this.ClubActivityPhase > 0)
								{
									this.DistractionTarget.AlreadyFed = true;
									this.GetFoodTarget();
								}
								if (this.StudentID == this.StudentManager.SuitorID && this.StudentManager.DatingMinigame.SuitorAndRivalTalking)
								{
									Debug.Log("Fire ''CalculateLove()''");
									this.StudentManager.LoveManager.Courted = true;
									this.DialogueWheel.AdviceWindow.CalculateLove();
									this.StudentManager.DatingMinigame.SuitorAndRivalTalking = false;
									this.Yandere.PromptParent.gameObject.SetActive(false);
								}
							}
						}
						else if (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Cooking && this.ClubActivityPhase > 0)
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
							this.Pathfinding.canSearch = true;
							this.Pathfinding.canMove = true;
						}
						else if (this.Pathfinding.speed == this.WalkSpeed)
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.SprintAnim);
						}
					}
					else if (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Cooking && this.ClubActivityPhase > 0)
					{
						this.CharacterAnimation.CrossFade(this.WalkAnim);
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						if (this.Phase < this.ScheduleBlocks.Length - 1 && this.Clock.HourTime >= this.ScheduleBlocks[this.Phase].time)
						{
							this.Routine = true;
						}
					}
					else if (this.Pathfinding.speed == this.WalkSpeed)
					{
						this.CharacterAnimation.CrossFade(this.WalkAnim);
					}
					else
					{
						this.CharacterAnimation.CrossFade(this.SprintAnim);
					}
				}
			}
			if (this.Hunting)
			{
				this.HuntTimer += Time.deltaTime;
				if (this.HuntTimer > 1f && base.transform.position.x < 22f && base.transform.position.x > -22f)
				{
					GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(this.AlarmDisc, base.transform.position + Vector3.up, Quaternion.identity);
					gameObject6.GetComponent<AlarmDiscScript>().Originator = this;
					gameObject6.GetComponent<AlarmDiscScript>().Shocking = true;
					gameObject6.GetComponent<AlarmDiscScript>().NoScream = true;
					gameObject6.GetComponent<AlarmDiscScript>().Silent = true;
					gameObject6.GetComponent<AlarmDiscScript>().Slave = true;
					this.HuntTimer = 0f;
				}
				if (this.HuntTarget != null)
				{
					if (this.HuntTarget.Prompt.enabled && !this.HuntTarget.FightingSlave)
					{
						this.HuntTarget.Prompt.Hide();
						this.HuntTarget.Prompt.enabled = false;
					}
					this.Pathfinding.target = this.HuntTarget.transform;
					this.CurrentDestination = this.HuntTarget.transform;
					this.DistanceToDestination = Vector3.Distance(base.transform.position, this.HuntTarget.transform.position);
					if (this.HuntTarget.Alive && !this.HuntTarget.Tranquil && !this.HuntTarget.PinningDown)
					{
						if (this.DistanceToDestination > this.TargetDistance)
						{
							if (this.MurderSuicidePhase == 0)
							{
								if (this.CharacterAnimation["f02_brokenStandUp_00"].time >= this.CharacterAnimation["f02_brokenStandUp_00"].length)
								{
									this.MurderSuicidePhase++;
									this.Pathfinding.canSearch = true;
									this.Pathfinding.canMove = true;
									this.CharacterAnimation.CrossFade(this.WalkAnim);
									this.Pathfinding.speed = 1.15f;
								}
							}
							else if (this.MurderSuicidePhase == 1)
							{
								if (!this.Male)
								{
									this.CharacterAnimation.CrossFade(this.WalkAnim);
								}
								else
								{
									this.CharacterAnimation.CrossFade(this.RunAnim);
								}
								this.Pathfinding.canSearch = true;
								this.Pathfinding.canMove = true;
							}
							else if (this.MurderSuicidePhase > 1)
							{
								this.CharacterAnimation.CrossFade(this.WalkAnim);
								this.HuntTarget.MoveTowardsTarget(base.transform.position + base.transform.forward * 0.01f);
							}
							if (this.HuntTarget.Dying || this.HuntTarget.Struggling || this.HuntTarget.Ragdoll.enabled || (this.HuntTarget.Hunter != null && this.HuntTarget.Hunter != this))
							{
								this.Hunting = false;
								this.Suicide = true;
							}
						}
						else if (this.HuntTarget.ClubActivityPhase >= 16 || this.HuntTarget.Shoving || this.HuntTarget.ChangingShoes || this.HuntTarget.Chasing || this.Yandere.Pursuer == this.HuntTarget || this.HuntTarget.SeekingMedicine || this.HuntTarget.EndSearch || (this.StudentManager.CombatMinigame.Delinquent == this.HuntTarget && this.StudentManager.CombatMinigame.Path == 5) || !this.HuntTarget.enabled || this.HuntTarget.BreakingUpFight || (this.HuntTarget.Cheer != null && this.HuntTarget.Cheer.enabled))
						{
							Debug.Log("The mind-broken slave has to wait for something...");
							this.CharacterAnimation.CrossFade(this.IdleAnim);
							this.Pathfinding.canSearch = false;
							this.Pathfinding.canMove = false;
						}
						else
						{
							if (!this.Male && !this.Broken.Done)
							{
								this.Pathfinding.canSearch = true;
								this.Pathfinding.canMove = true;
							}
							if (!this.NEStairs.bounds.Contains(base.transform.position) && !this.NWStairs.bounds.Contains(base.transform.position) && !this.SEStairs.bounds.Contains(base.transform.position) && !this.SWStairs.bounds.Contains(base.transform.position) && !this.StudentManager.EastGazebo.bounds.Contains(base.transform.position) && !this.StudentManager.WestGazebo.bounds.Contains(base.transform.position) && !this.PoolStairs.bounds.Contains(base.transform.position) && !this.NEStairs.bounds.Contains(this.HuntTarget.transform.position) && !this.NWStairs.bounds.Contains(this.HuntTarget.transform.position) && !this.SEStairs.bounds.Contains(this.HuntTarget.transform.position) && !this.SWStairs.bounds.Contains(this.HuntTarget.transform.position) && !this.StudentManager.EastGazebo.bounds.Contains(this.HuntTarget.transform.position) && !this.StudentManager.WestGazebo.bounds.Contains(this.HuntTarget.transform.position) && !this.PoolStairs.bounds.Contains(this.HuntTarget.transform.position))
							{
								if (this.Pathfinding.canMove)
								{
									Debug.Log("Slave is attacking target!");
									if (this.HuntTarget.InvestigatingBloodPool)
									{
										this.HuntTarget.ForgetAboutBloodPool();
									}
									if (this.HuntTarget.Strength == 9)
									{
										Debug.Log("Target is Invincible! Attack should fail!");
										this.AttackWillFail = true;
										if (!this.StudentManager.CustomMode && !this.StudentManager.Eighties && this.StudentID == 11)
										{
											Debug.Log("But, wait, it's Osana attacking Raibaru! Attack should succeed!");
											this.AttackWillFail = false;
										}
									}
									if (!this.AttackWillFail)
									{
										this.CharacterAnimation.CrossFade(this.MurderSuicideAnim);
									}
									else
									{
										if (!this.Male)
										{
											this.CharacterAnimation.CrossFade("f02_brokenAttackFailA_00");
										}
										else
										{
											this.CharacterAnimation.CrossFade("brokenAttackFailA_00");
										}
										this.CharacterAnimation[this.WetAnim].weight = 0f;
										this.Police.CorpseList[this.Police.Corpses] = this.Ragdoll;
										this.Police.Corpses++;
										GameObjectUtils.SetLayerRecursively(base.gameObject, 11);
										this.MapMarker.gameObject.layer = 10;
										base.tag = "Blood";
										this.Ragdoll.MurderSuicideAnimation = true;
										this.Ragdoll.Disturbing = true;
										this.Dying = true;
										this.HipCollider.enabled = true;
										this.HipCollider.radius = 0.5f;
										this.MurderSuicidePhase = 9;
									}
									this.Pathfinding.canSearch = false;
									this.Pathfinding.canMove = false;
									if (!this.Male)
									{
										this.Broken.Subtitle.text = string.Empty;
										this.Broken.Done = true;
									}
									this.MyController.radius = 0f;
									if (!this.AttackWillFail)
									{
										this.SpawnTimeRespectingAudioSource(this.MurderSuicideSounds);
										this.SpawnTimeRespectingAudioSource(this.MurderSuicideKiller);
									}
									if (this.HuntTarget.Shoving)
									{
										this.Yandere.CannotRecover = false;
									}
									if (this.StudentManager.CombatMinigame.Delinquent == this.HuntTarget)
									{
										this.StudentManager.CombatMinigame.Stop();
										this.StudentManager.CombatMinigame.ReleaseYandere();
									}
									if (!this.AttackWillFail)
									{
										this.HuntTarget.HipCollider.enabled = true;
										this.HuntTarget.HipCollider.radius = 1f;
										this.HuntTarget.DetectionMarker.Tex.enabled = false;
									}
									this.HuntTarget.CharacterAnimation[this.HuntTarget.WetAnim].weight = 0f;
									if (!this.HuntTarget.Male)
									{
										this.HuntTarget.CharacterAnimation[this.HuntTarget.ShyAnim].weight = 0f;
										if (this.HuntTarget.Club == ClubType.LightMusic)
										{
											this.HuntTarget.Instruments[this.HuntTarget.ClubMemberID].gameObject.SetActive(false);
										}
									}
									if (this.HuntTarget.Rival)
									{
										this.HuntTarget.MapMarker.gameObject.SetActive(false);
									}
									if (this.HuntTarget.ReturningMisplacedWeapon)
									{
										this.HuntTarget.DropMisplacedWeapon();
									}
									bool flag9 = false;
									using (IEnumerator enumerator = this.HuntTarget.ItemParent.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											if (((Transform)enumerator.Current).gameObject.activeSelf)
											{
												flag9 = true;
											}
										}
									}
									if (flag9)
									{
										this.HuntTarget.EmptyHands();
									}
									this.HuntTarget.TargetedForDistraction = false;
									this.HuntTarget.Pathfinding.canSearch = false;
									this.HuntTarget.Pathfinding.canMove = false;
									this.HuntTarget.WitnessCamera.Show = false;
									this.HuntTarget.CameraReacting = false;
									this.HuntTarget.FocusOnStudent = false;
									this.HuntTarget.FocusOnYandere = false;
									this.HuntTarget.Investigating = false;
									this.HuntTarget.Distracting = false;
									this.HuntTarget.SentHome = false;
									this.HuntTarget.Splashed = false;
									this.HuntTarget.Alarmed = false;
									this.HuntTarget.Burning = false;
									this.HuntTarget.Fleeing = false;
									this.HuntTarget.Routine = false;
									this.HuntTarget.Shoving = false;
									this.HuntTarget.Tripped = false;
									this.HuntTarget.Blind = true;
									this.HuntTarget.Wet = false;
									this.HuntTarget.Hunter = this;
									Debug.Log(this.HuntTarget.Name + " should now recognize " + this.Name + " as their Hunter.");
									this.HuntTarget.Prompt.Hide();
									this.HuntTarget.Prompt.enabled = false;
									this.Distracted = true;
									this.Blind = true;
									if (this.Yandere.Pursuer == this.HuntTarget)
									{
										this.Yandere.Chased = false;
										this.Yandere.Pursuer = null;
									}
									if (!this.AttackWillFail)
									{
										if (!this.HuntTarget.Male)
										{
											this.HuntTarget.CharacterAnimation.CrossFade("f02_murderSuicide_01");
										}
										else
										{
											this.HuntTarget.CharacterAnimation.CrossFade("murderSuicide_01");
										}
										this.HuntTarget.CharacterAnimation[this.HuntTarget.WetAnim].weight = 0f;
										this.HuntTarget.Subtitle.UpdateLabel(SubtitleType.Dying, 0, 1f);
										this.SpawnTimeRespectingAudioSource(this.HuntTarget.MurderSuicideVictim);
										this.Police.CorpseList[this.Police.Corpses] = this.HuntTarget.Ragdoll;
										this.Police.Corpses++;
										GameObjectUtils.SetLayerRecursively(this.HuntTarget.gameObject, 11);
										this.MapMarker.gameObject.layer = 10;
										this.HuntTarget.tag = "Blood";
										this.HuntTarget.Ragdoll.MurderSuicideAnimation = true;
										this.HuntTarget.Ragdoll.Disturbing = true;
										this.HuntTarget.Dying = true;
										this.HuntTarget.MurderSuicidePhase = 100;
									}
									else
									{
										if (!this.HuntTarget.Male)
										{
											this.HuntTarget.CharacterAnimation.CrossFade("f02_brokenAttackFailB_00");
										}
										else
										{
											this.HuntTarget.CharacterAnimation.CrossFade("brokenAttackFailB_00");
										}
										this.HuntTarget.FightingSlave = true;
										this.HuntTarget.Distracted = true;
										this.HuntTarget.Blind = false;
										this.MyWeapon.transform.parent = this.ItemParent;
										this.MyWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
										this.MyWeapon.transform.localPosition = Vector3.zero;
										this.MyWeapon.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
										this.StudentManager.UpdateMe(this.HuntTarget.StudentID);
									}
									this.HuntTarget.MyController.radius = 0f;
									this.HuntTarget.SpeechLines.Stop();
									this.HuntTarget.EyeShrink = 1f;
									this.HuntTarget.SpawnAlarmDisc();
									if (this.HuntTarget.Following)
									{
										this.Yandere.Follower = null;
										this.Yandere.Followers--;
										this.Hearts.emission.enabled = false;
										this.HuntTarget.FollowCountdown.gameObject.SetActive(false);
										this.HuntTarget.Following = false;
									}
									this.OriginalYPosition = this.HuntTarget.transform.position.y;
									if (this.MurderSuicidePhase == 0)
									{
										this.MurderSuicidePhase++;
									}
								}
								else
								{
									if (!this.AttackWillFail)
									{
										if (!this.HuntTarget.Male)
										{
											this.HuntTarget.CharacterAnimation.CrossFade("f02_murderSuicide_01");
										}
										else
										{
											this.HuntTarget.CharacterAnimation.CrossFade("murderSuicide_01");
										}
									}
									this.TooCloseToWall = false;
									this.CheckForWallInFront(1f);
									if (this.TooCloseToWall)
									{
										this.MyController.Move(base.transform.forward * Time.deltaTime * -0.1f);
									}
									if (this.Dying)
									{
										this.Yandere.NearMindSlave = (Vector3.Distance(base.transform.position, this.Yandere.transform.position) < 5f);
									}
									if (this.MurderSuicidePhase == 0 && this.CharacterAnimation["f02_brokenStandUp_00"].time >= this.CharacterAnimation["f02_brokenStandUp_00"].length)
									{
										this.Pathfinding.canSearch = true;
										this.Pathfinding.canMove = true;
									}
									if (this.MurderSuicidePhase > 0)
									{
										if (!this.AttackWillFail)
										{
											this.HuntTarget.targetRotation = Quaternion.LookRotation(this.HuntTarget.transform.position - base.transform.position);
											this.HuntTarget.MoveTowardsTarget(base.transform.position + base.transform.forward * 0.01f);
										}
										else
										{
											this.HuntTarget.targetRotation = Quaternion.LookRotation(base.transform.position - this.HuntTarget.transform.position);
											this.HuntTarget.MoveTowardsTarget(base.transform.position + base.transform.forward * 1f);
										}
										this.HuntTarget.transform.rotation = Quaternion.Slerp(this.HuntTarget.transform.rotation, this.HuntTarget.targetRotation, Time.deltaTime * 10f);
										base.transform.position = new Vector3(base.transform.position.x, this.OriginalYPosition, base.transform.position.z);
										this.HuntTarget.transform.position = new Vector3(this.HuntTarget.transform.position.x, this.OriginalYPosition, this.HuntTarget.transform.position.z);
										this.targetRotation = Quaternion.LookRotation(this.HuntTarget.transform.position - base.transform.position);
										base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, Time.deltaTime * 10f);
										Physics.SyncTransforms();
									}
									if (this.MurderSuicidePhase == 1)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 2.4f)
										{
											this.MyWeapon.transform.parent = this.ItemParent;
											this.MyWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
											this.MyWeapon.transform.localPosition = Vector3.zero;
											this.MyWeapon.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 2)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 3.3f)
										{
											GameObject gameObject7 = UnityEngine.Object.Instantiate<GameObject>(this.Ragdoll.BloodPoolSpawner.BloodPool, base.transform.position + base.transform.up * 0.021f + base.transform.forward, Quaternion.identity);
											gameObject7.transform.localEulerAngles = new Vector3(90f, UnityEngine.Random.Range(0f, 360f), 0f);
											gameObject7.transform.parent = this.Police.BloodParent;
											this.MyWeapon.Victims[this.HuntTarget.StudentID] = true;
											this.MyWeapon.Blood.enabled = true;
											this.MyWeapon.StainWithBlood();
											if (!this.MyWeapon.Evidence)
											{
												Debug.Log("A mind-broken slave is running the code for staining her equipped weapon with blood and marking it as evidence.");
												this.MyWeapon.MurderWeapon = true;
												this.MyWeapon.Evidence = true;
												this.MyWeapon.Bloody = true;
												this.Police.MurderWeapons++;
											}
											UnityEngine.Object.Instantiate<GameObject>(this.BloodEffect, this.MyWeapon.transform.position, Quaternion.identity);
											this.KnifeDown = true;
											this.LiquidProjector.material = this.BloodMaterial;
											this.LiquidProjector.gameObject.SetActive(true);
											this.LiquidProjector.enabled = true;
											this.HuntTarget.LiquidProjector.material = this.HuntTarget.BloodMaterial;
											this.HuntTarget.LiquidProjector.gameObject.SetActive(true);
											this.HuntTarget.LiquidProjector.enabled = true;
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 3)
									{
										if (!this.KnifeDown)
										{
											if (this.MyWeapon.transform.position.y < base.transform.position.y + 0.33333334f)
											{
												UnityEngine.Object.Instantiate<GameObject>(this.BloodEffect, this.MyWeapon.transform.position, Quaternion.identity);
												this.KnifeDown = true;
												Debug.Log("Stab!");
											}
										}
										else if (this.MyWeapon.transform.position.y > base.transform.position.y + 0.33333334f)
										{
											this.KnifeDown = false;
										}
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 16.666666f)
										{
											Debug.Log("Released knife!");
											this.MyWeapon.transform.parent = null;
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 4)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 18.9f)
										{
											Debug.Log("Yanked out knife!");
											UnityEngine.Object.Instantiate<GameObject>(this.BloodEffect, this.MyWeapon.transform.position, Quaternion.identity);
											this.MyWeapon.transform.parent = this.ItemParent;
											this.MyWeapon.transform.localPosition = Vector3.zero;
											this.MyWeapon.transform.localEulerAngles = Vector3.zero;
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 5)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 26.166666f)
										{
											Debug.Log("Stabbed neck!");
											UnityEngine.Object.Instantiate<GameObject>(this.BloodEffect, this.MyWeapon.transform.position, Quaternion.identity);
											this.MyWeapon.Victims[this.StudentID] = true;
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 6)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 30.5f)
										{
											Debug.Log("BLOOD FOUNTAIN!");
											if (!this.Male)
											{
												this.BloodSprayCollider.SetActive(true);
												this.BloodSprayCollider.layer = 2;
											}
											this.BloodFountain.Play();
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 7)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= 31.5f)
										{
											this.MurderSuicidePhase++;
										}
									}
									else if (this.MurderSuicidePhase == 8)
									{
										if (this.CharacterAnimation[this.MurderSuicideAnim].time >= this.CharacterAnimation[this.MurderSuicideAnim].length)
										{
											this.Yandere.NearMindSlave = false;
											this.MyWeapon.transform.parent = null;
											this.MyWeapon.DoNotRelocate = true;
											this.MyWeapon.Drop();
											this.MyWeapon = null;
											this.StudentManager.StopHesitating();
											this.HuntTarget.HipCollider.radius = 0.5f;
											this.HuntTarget.BecomeRagdoll();
											this.HuntTarget.MurderedByStudent = true;
											this.HuntTarget.Ragdoll.Disturbing = false;
											this.HuntTarget.Ragdoll.MurderSuicide = true;
											this.HuntTarget.DeathType = DeathType.Weapon;
											if (this.FragileSlave)
											{
												this.HuntTarget.MurderedByFragile = true;
												this.HuntTarget.Hunted = true;
											}
											if (this.HuntTarget.Follower != null)
											{
												Debug.Log("This mind-broken slave just killed someone who had a follower.");
												if (this.HuntTarget.Follower.WitnessedMindBrokenMurder)
												{
													Debug.Log("The follower's ''Corpse'' variable is being set to: " + this.HuntTarget.Ragdoll.Student.Name);
													this.HuntTarget.Follower.Corpse = this.HuntTarget.Ragdoll;
												}
											}
											if (this.BloodSprayCollider != null)
											{
												this.BloodSprayCollider.SetActive(false);
											}
											this.BecomeRagdoll();
											this.DeathType = DeathType.Weapon;
											this.StudentManager.MurderTakingPlace = false;
											this.Ragdoll.MurderSuicide = true;
											this.MurderSuicide = true;
											if (!this.Male)
											{
												this.Broken.HairPhysics[0].enabled = true;
												this.Broken.HairPhysics[1].enabled = true;
												this.Broken.enabled = false;
											}
											this.Hunting = false;
											if (this.StudentID > 10 && this.StudentID < 21)
											{
												Debug.Log("A former rival killed herself as a mind-broken slave. StudentManager will be informed.");
												this.StudentManager.UpdateRivalEliminationDetails(this.StudentID);
											}
											Debug.Log("MurderTakingPlace *should* now be false...");
										}
									}
									else if (this.MurderSuicidePhase == 9)
									{
										if (!this.Male)
										{
											this.CharacterAnimation.CrossFade("f02_brokenAttackFailA_00");
										}
										else
										{
											this.CharacterAnimation.CrossFade("brokenAttackFailA_00");
										}
										if ((!this.Male && this.CharacterAnimation["f02_brokenAttackFailA_00"].time >= this.CharacterAnimation["f02_brokenAttackFailA_00"].length) || (this.Male && this.CharacterAnimation["brokenAttackFailA_00"].time >= this.CharacterAnimation["brokenAttackFailA_00"].length))
										{
											Debug.Log("A mind-broken slave just failed to kill her target.");
											this.Yandere.NearMindSlave = false;
											this.MurderSuicidePhase = 1;
											this.Hunting = false;
											this.Suicide = true;
											this.HuntTarget.MyController.radius = 0.1f;
											this.HuntTarget.Distracted = false;
											this.HuntTarget.Routine = true;
											this.HuntTarget.FightingSlave = false;
											this.StudentManager.UpdateMe(this.HuntTarget.StudentID);
											this.StudentManager.MurderTakingPlace = false;
											if (this.StudentID > 10 && this.StudentID < 21)
											{
												Debug.Log("A former rival killed herself as a mind-broken slave. StudentManager will be informed.");
												this.StudentManager.UpdateRivalEliminationDetails(this.StudentID);
											}
											Debug.Log("MurderTakingPlace *should* now be false...");
										}
										else if (((!this.Male && this.CharacterAnimation["f02_brokenAttackFailA_00"].time >= 6.5f) || (this.Male && this.CharacterAnimation["brokenAttackFailA_00"].time >= 6.5f)) && this.HuntTarget.FightingSlave)
										{
											this.HuntTarget.FightingSlave = false;
											this.StudentManager.UpdateMe(this.HuntTarget.StudentID);
										}
									}
								}
							}
						}
					}
					else
					{
						this.Hunting = false;
						this.Suicide = true;
					}
				}
				else
				{
					this.Hunting = false;
					this.Suicide = true;
				}
			}
			if (this.Suicide)
			{
				if (this.MurderSuicidePhase == 0)
				{
					if (this.CharacterAnimation["f02_brokenStandUp_00"].time >= this.CharacterAnimation["f02_brokenStandUp_00"].length)
					{
						this.MurderSuicidePhase++;
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.Pathfinding.speed = 0f;
						this.CharacterAnimation.CrossFade("f02_suicide_00");
					}
				}
				else if (this.MurderSuicidePhase == 1)
				{
					if (this.Pathfinding.speed > 0f)
					{
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.Pathfinding.speed = 0f;
						this.CharacterAnimation.CrossFade("f02_suicide_00");
					}
					if (this.CharacterAnimation["f02_suicide_00"].time >= 0.73333335f)
					{
						this.MyWeapon.transform.parent = this.ItemParent;
						this.MyWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
						this.MyWeapon.transform.localPosition = Vector3.zero;
						this.MyWeapon.transform.localEulerAngles = Vector3.zero;
						this.Broken.Subtitle.text = string.Empty;
						this.Broken.Done = true;
						this.MurderSuicidePhase++;
					}
				}
				else if (this.MurderSuicidePhase == 2)
				{
					if (this.CharacterAnimation["f02_suicide_00"].time >= 4.1666665f)
					{
						Debug.Log("Stabbed neck!");
						UnityEngine.Object.Instantiate<GameObject>(this.StabBloodEffect, this.MyWeapon.transform.position, Quaternion.identity);
						this.MyWeapon.Victims[this.StudentID] = true;
						this.MyWeapon.Blood.enabled = true;
						this.MyWeapon.StainWithBlood();
						if (!this.MyWeapon.Evidence)
						{
							this.MyWeapon.Evidence = true;
							this.Police.MurderWeapons++;
						}
						this.MurderSuicidePhase++;
					}
				}
				else if (this.MurderSuicidePhase == 3)
				{
					if (this.CharacterAnimation["f02_suicide_00"].time >= 6.1666665f)
					{
						Debug.Log("BLOOD FOUNTAIN! (After losing struggle.)");
						if (!this.Male)
						{
							this.BloodSprayCollider.SetActive(true);
							this.BloodSprayCollider.layer = 2;
						}
						this.BloodFountain.gameObject.GetComponent<AudioSource>().Play();
						this.BloodFountain.Play();
						this.MurderSuicidePhase++;
					}
				}
				else if (this.MurderSuicidePhase == 4)
				{
					if (this.CharacterAnimation["f02_suicide_00"].time >= 7f)
					{
						this.Ragdoll.BloodPoolSpawner.SpawnPool(base.transform);
						this.MurderSuicidePhase++;
					}
				}
				else if (this.MurderSuicidePhase == 5 && this.CharacterAnimation["f02_suicide_00"].time >= this.CharacterAnimation["f02_suicide_00"].length)
				{
					this.MyWeapon.transform.parent = null;
					this.MyWeapon.DoNotRelocate = true;
					this.MyWeapon.Drop();
					this.MyWeapon = null;
					this.StudentManager.StopHesitating();
					if (this.BloodSprayCollider != null)
					{
						this.BloodSprayCollider.SetActive(false);
					}
					this.BecomeRagdoll();
					this.DeathType = DeathType.Weapon;
					this.Broken.HairPhysics[0].enabled = true;
					this.Broken.HairPhysics[1].enabled = true;
					this.Broken.enabled = false;
					this.StudentManager.MurderTakingPlace = false;
					if (this.StudentID > 10 && this.StudentID < 21)
					{
						Debug.Log("A former rival killed herself as a mind-broken slave. StudentManager will be informed.");
						this.StudentManager.UpdateRivalEliminationDetails(this.StudentID);
					}
					Debug.Log("MurderTakingPlace *should* now be false...");
				}
			}
			if (this.CameraReacting)
			{
				this.PhotoPatience = Mathf.MoveTowards(this.PhotoPatience, 0f, Time.deltaTime);
				this.Prompt.Circle[0].fillAmount = 1f;
				if (!this.Yandere.Selfie)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
				}
				else
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Smartphone.transform.position.x, base.transform.position.y, this.Yandere.Smartphone.transform.position.z) - base.transform.position);
				}
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				if (!this.Yandere.ClubAccessories[7].activeInHierarchy || this.Club == ClubType.Delinquent)
				{
					if (this.CameraReactPhase == 1)
					{
						if (this.CharacterAnimation[this.CameraAnims[1]].time >= this.CharacterAnimation[this.CameraAnims[1]].length)
						{
							this.CharacterAnimation.CrossFade(this.CameraAnims[2]);
							this.CameraReactPhase = 2;
							this.CameraPoseTimer = 1f;
						}
					}
					else if (this.CameraReactPhase == 2)
					{
						this.CameraPoseTimer -= Time.deltaTime;
						if (this.CameraPoseTimer <= 0f)
						{
							this.CharacterAnimation.CrossFade(this.CameraAnims[3]);
							this.CameraReactPhase = 3;
						}
					}
					else if (this.CameraReactPhase == 3)
					{
						if (this.CameraPoseTimer == 1f)
						{
							this.CharacterAnimation.CrossFade(this.CameraAnims[2]);
							this.CameraReactPhase = 2;
						}
						if (this.CharacterAnimation[this.CameraAnims[3]].time >= this.CharacterAnimation[this.CameraAnims[3]].length)
						{
							if (!this.Phoneless && (this.Persona == PersonaType.PhoneAddict || this.Sleuthing))
							{
								this.SmartPhone.SetActive(true);
							}
							if (this.Cigarette.activeInHierarchy)
							{
								this.SmartPhone.SetActive(false);
							}
							this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
							this.Obstacle.enabled = false;
							this.CameraReacting = false;
							this.Routine = true;
							this.ReadPhase = 0;
							if (this.Actions[this.Phase] == StudentActionType.Clean)
							{
								this.Scrubber.SetActive(true);
								if (this.CleaningRole == 5)
								{
									this.Eraser.SetActive(true);
								}
							}
						}
					}
				}
				else if (this.Yandere.Shutter.TargetStudent != this.StudentID)
				{
					this.CameraPoseTimer = Mathf.MoveTowards(this.CameraPoseTimer, 0f, Time.deltaTime);
					if (this.CameraPoseTimer == 0f)
					{
						if (!this.Phoneless && (this.Persona == PersonaType.PhoneAddict || this.Sleuthing))
						{
							this.SmartPhone.SetActive(true);
						}
						this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
						this.Obstacle.enabled = false;
						this.CameraReacting = false;
						this.Routine = true;
						this.ReadPhase = 0;
						if (this.Actions[this.Phase] == StudentActionType.Clean)
						{
							this.Scrubber.SetActive(true);
							if (this.CleaningRole == 5)
							{
								this.Eraser.SetActive(true);
							}
						}
					}
				}
				else
				{
					this.CameraPoseTimer = 1f;
				}
				if (this.InEvent)
				{
					this.CameraReacting = false;
					this.ReadPhase = 0;
				}
			}
			if (this.Investigating)
			{
				if (!this.YandereInnocent && this.InvestigationPhase < 100 && this.CanSeeObject(this.Yandere.gameObject, new Vector3(this.Yandere.HeadPosition.x, this.Yandere.transform.position.y + this.Yandere.Zoom.Height, this.Yandere.HeadPosition.z)))
				{
					if (Vector3.Distance(this.Yandere.transform.position, this.Giggle.transform.position) > 2.5f)
					{
						this.YandereInnocent = true;
					}
					else
					{
						this.CharacterAnimation.CrossFade(this.IdleAnim);
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.InvestigationPhase = 100;
						this.InvestigationTimer = 0f;
					}
				}
				if (this.InvestigationPhase == 0)
				{
					if (this.InvestigationTimer < 5f)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("heardSuspiciousNoise_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade("f02_heardSuspiciousNoise_03");
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						if ((this.Persona == PersonaType.Heroic && this.HeardScream) || this.Persona == PersonaType.Protective)
						{
							this.InvestigationTimer += Time.deltaTime * 5f;
						}
						else
						{
							this.InvestigationTimer += Time.deltaTime;
						}
						this.targetRotation = Quaternion.LookRotation(new Vector3(this.Giggle.transform.position.x, base.transform.position.y, this.Giggle.transform.position.z) - base.transform.position);
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					}
					else
					{
						if (this.Giggle != null)
						{
							this.Pathfinding.target = this.Giggle.transform;
							this.CurrentDestination = this.Giggle.transform;
						}
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						if ((this.Persona == PersonaType.Heroic && this.HeardScream) || this.Persona == PersonaType.Protective || this.Hurry || this.WalkSpeed == 4f || this.Pathfinding.speed == 4f)
						{
							this.CharacterAnimation.CrossFade(this.SprintAnim);
							this.Pathfinding.speed = 4f;
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
							this.Pathfinding.speed = this.WalkSpeed;
						}
						this.InvestigationPhase++;
					}
				}
				else if (this.InvestigationPhase == 1)
				{
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					if (this.InvestigationDistance == 0f)
					{
						this.InvestigationDistance = 0.8f;
					}
					if (this.DistanceToDestination > this.InvestigationDistance)
					{
						if ((this.Persona == PersonaType.Heroic && this.HeardScream) || this.Persona == PersonaType.Protective || this.Hurry || this.WalkSpeed == 4f || this.Pathfinding.speed == 4f)
						{
							this.CharacterAnimation.CrossFade(this.SprintAnim);
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.WalkAnim);
						}
					}
					else
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("lookLeftRight_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade("f02_lookLeftRight_00");
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.InvestigationPhase++;
					}
				}
				else if (this.InvestigationPhase == 2)
				{
					if (this.Persona == PersonaType.Protective)
					{
						this.InvestigationTimer += Time.deltaTime * 2.5f;
					}
					else
					{
						this.InvestigationTimer += Time.deltaTime;
					}
					if (this.InvestigationTimer > 10f)
					{
						this.StopInvestigating();
					}
				}
				else if (this.InvestigationPhase == 100)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					this.InvestigationTimer += Time.deltaTime;
					if (this.InvestigationTimer > 2f)
					{
						this.StopInvestigating();
					}
				}
				if (this.StudentID == 9 && this.Clock.GameplayDay == 5 && base.transform.position.x > 17f && base.transform.position.x < 23f && base.transform.position.z > 50f && base.transform.position.z < 54f)
				{
					this.BountyCollider.SetActive(true);
				}
			}
			if (this.EndSearch)
			{
				if (this.PatrolTimer == 0f)
				{
					Debug.Log(this.Name + " just got her phone back.");
				}
				this.MoveTowardsTarget(this.Pathfinding.target.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Pathfinding.target.rotation, 10f * Time.deltaTime);
				this.PatrolTimer += Time.deltaTime * this.CharacterAnimation[this.DiscoverPhoneAnim].speed;
				if (this.PatrolTimer >= this.CharacterAnimation[this.DiscoverPhoneAnim].length)
				{
					Debug.Log(this.Name + " is now attempting to return to her previous routine.");
					ScheduleBlock scheduleBlock26 = this.ScheduleBlocks[2];
					scheduleBlock26.destination = "Hangout";
					scheduleBlock26.action = "Hangout";
					if (this.Club == ClubType.Cooking || this.Club == ClubType.MartialArts)
					{
						scheduleBlock26.destination = "Club";
						scheduleBlock26.action = "Club";
					}
					if (this.Club == ClubType.LightMusic)
					{
						scheduleBlock26.destination = "Practice";
						scheduleBlock26.action = "Practice";
					}
					ScheduleBlock scheduleBlock27 = this.ScheduleBlocks[4];
					scheduleBlock27.destination = "LunchSpot";
					scheduleBlock27.action = "Eat";
					ScheduleBlock scheduleBlock28 = this.ScheduleBlocks[7];
					scheduleBlock28.destination = "Hangout";
					scheduleBlock28.action = "Hangout";
					Debug.Log("ScheduleBlocks[2].destination is: " + this.ScheduleBlocks[2].destination);
					this.RestoreOriginalScheduleBlocks();
					this.RestoreOriginalActions();
					if (this.Actions[2] == StudentActionType.Graffiti && !this.StudentManager.Bully)
					{
						scheduleBlock26.destination = "Patrol";
						scheduleBlock26.action = "Patrol";
					}
					Debug.Log("And now, ScheduleBlocks[2].destination is: " + this.ScheduleBlocks[2].destination);
					this.GetDestinations();
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.CurrentAction = this.Actions[this.Phase];
					this.DistanceToDestination = 100f;
					this.SearchingForPhone = false;
					this.EndSearch = false;
					this.Phoneless = false;
					this.Routine = true;
					this.Hurry = false;
					this.SprintAnim = this.OriginalSprintAnim;
					this.RunAnim = this.OriginalSprintAnim;
					this.WalkAnim = this.OriginalWalkAnim;
					if (this.Persona == PersonaType.PhoneAddict && !this.StudentManager.Eighties)
					{
						this.WalkAnim = this.PhoneAnims[1];
					}
					this.WalkSpeed = 1f;
					this.Pathfinding.speed = this.WalkSpeed;
					this.Hurry = false;
					this.StudentManager.CommunalLocker.RivalPhone.ReturnToOrigin();
					if (this.Follower != null)
					{
						this.Follower.TargetDistance = 0.5f;
					}
					if (this.Persona == PersonaType.PhoneAddict)
					{
						this.SmartPhone.SetActive(true);
					}
				}
			}
			if (this.Shoving)
			{
				if (this.SprayTimer > 0f)
				{
					this.SprayTimer = Mathf.MoveTowards(this.SprayTimer, 0f, Time.deltaTime);
				}
				this.CharacterAnimation.CrossFade(this.ShoveAnim);
				if (this.CharacterAnimation[this.ShoveAnim].time > this.CharacterAnimation[this.ShoveAnim].length)
				{
					if ((this.Club != ClubType.Council && this.Persona != PersonaType.Violent && this.StudentID != 20) || this.RespectEarned)
					{
						this.Patience = 999;
					}
					if (this.Patience > 0)
					{
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.Distracted = false;
						this.Shoving = false;
						this.Routine = true;
						this.Paired = false;
						this.InstantNoticeTimer = 1f;
					}
					else if (this.Club == ClubType.Council || this.Shovey)
					{
						Debug.Log("Calling ''Spray()'' from this part of the code. 2");
						this.Shoving = false;
						this.Spray();
					}
					else
					{
						this.SpawnAlarmDisc();
						this.SmartPhone.SetActive(false);
						this.Distracted = true;
						this.Threatened = true;
						this.Shoving = false;
						this.Alarmed = true;
					}
				}
			}
			if (this.Spraying)
			{
				this.CharacterAnimation.CrossFade(this.SprayAnim);
				this.Yandere.CharacterAnimation.CrossFade("f02_sprayed_00");
				this.Yandere.Attacking = false;
				if ((double)this.CharacterAnimation[this.SprayAnim].time > 0.66666)
				{
					if (this.Yandere.Armed)
					{
						this.Yandere.EquippedWeapon.Drop();
					}
					this.Yandere.EmptyHands();
					this.PepperSprayEffect.Play();
					this.Spraying = false;
					base.enabled = false;
				}
			}
			if (this.SentHome)
			{
				if (this.SentHomePhase == 0)
				{
					if (this.Shy)
					{
						this.CharacterAnimation[this.ShyAnim].weight = 0f;
					}
					this.CharacterAnimation.CrossFade(this.SentHomeAnim);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.SentHomePhase++;
					if (this.SmartPhone.activeInHierarchy)
					{
						this.CharacterAnimation[this.SentHomeAnim].time = 1.5f;
						this.SentHomePhase++;
					}
				}
				else if (this.SentHomePhase == 1)
				{
					this.CharacterAnimation.CrossFade(this.SentHomeAnim);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					if (this.CharacterAnimation[this.SentHomeAnim].time > 0.66666f)
					{
						this.SmartPhone.SetActive(true);
						this.SentHomePhase++;
					}
				}
				else if (this.SentHomePhase == 2)
				{
					this.CharacterAnimation.CrossFade(this.SentHomeAnim);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					if (this.CharacterAnimation[this.SentHomeAnim].time > this.CharacterAnimation[this.SentHomeAnim].length)
					{
						Debug.Log("Sprinting because sent home.");
						this.SprintAnim = this.OriginalSprintAnim;
						this.CharacterAnimation.CrossFade(this.SprintAnim);
						this.CurrentDestination = this.StudentManager.Exit;
						this.Pathfinding.target = this.StudentManager.Exit;
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.SmartPhone.SetActive(false);
						this.Pathfinding.speed = 4f;
						this.SentHomePhase++;
					}
				}
				else if (this.SentHomePhase == 3)
				{
					this.CharacterAnimation.CrossFade(this.SprintAnim);
					this.Pathfinding.speed = 4f;
				}
				if (base.transform.position.y < -11f)
				{
					base.transform.position = new Vector3(base.transform.position.x, -100f, base.transform.position.z);
					base.gameObject.SetActive(false);
				}
			}
			if (this.DramaticReaction)
			{
				this.DramaticCamera.transform.Translate(Vector3.forward * Time.deltaTime * 0.01f);
				if (this.DramaticPhase == 0)
				{
					this.DramaticCamera.rect = new Rect(0f, Mathf.Lerp(this.DramaticCamera.rect.y, 0.25f, Time.deltaTime * 10f), 1f, Mathf.Lerp(this.DramaticCamera.rect.height, 0.5f, Time.deltaTime * 10f));
					this.DramaticTimer += Time.deltaTime;
					if (this.DramaticTimer > 1f)
					{
						this.DramaticTimer = 0f;
						this.DramaticPhase++;
					}
				}
				else if (this.DramaticPhase == 1)
				{
					this.DramaticCamera.rect = new Rect(0f, Mathf.Lerp(this.DramaticCamera.rect.y, 0.5f, Time.deltaTime * 10f), 1f, Mathf.Lerp(this.DramaticCamera.rect.height, 0f, Time.deltaTime * 10f));
					this.DramaticTimer += Time.deltaTime;
					if (this.DramaticCamera.rect.height < 0.01f || this.DramaticTimer > 0.5f)
					{
						Debug.Log("Disabling Dramatic Camera.");
						this.DramaticCamera.gameObject.SetActive(false);
						this.AttackReaction();
						this.DramaticPhase++;
					}
				}
			}
			if (this.HitReacting && this.CharacterAnimation[this.SithReactAnim].time >= this.CharacterAnimation[this.SithReactAnim].length)
			{
				this.Persona = PersonaType.SocialButterfly;
				this.PersonaReaction();
				this.HitReacting = false;
			}
			if (this.Eaten)
			{
				if (this.Yandere.Eating)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				}
				if (this.CharacterAnimation[this.EatVictimAnim].time >= this.CharacterAnimation[this.EatVictimAnim].length)
				{
					this.BecomeRagdoll();
				}
			}
			if (this.Electrified)
			{
				Debug.Log(this.Name + " is now being electrocuted.");
				this.CharacterAnimation.CrossFade(this.ElectroAnim);
				if (this.CharacterAnimation[this.ElectroAnim].time >= this.CharacterAnimation[this.ElectroAnim].length || this.TooCloseToWall)
				{
					Debug.Log(this.Name + "'s TooCloseToWall is " + this.TooCloseToWall.ToString());
					this.Electrocuted = true;
					this.BecomeRagdoll();
					this.DeathType = DeathType.Electrocution;
					if (this.OsanaHairL != null)
					{
						this.OsanaHairL.GetComponent<DynamicBone>().enabled = true;
						this.OsanaHairR.GetComponent<DynamicBone>().enabled = true;
						this.OsanaHairL.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
						this.OsanaHairR.transform.localEulerAngles = new Vector3(0f, -90f, 180f);
					}
				}
				else if ((double)this.CharacterAnimation[this.ElectroAnim].time >= 9.5)
				{
					this.CheckForWallBehind();
				}
				else if (this.CharacterAnimation[this.ElectroAnim].time < 6f && this.OsanaHairL != null)
				{
					this.OsanaHairL.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
					this.OsanaHairR.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
				}
			}
			if (this.BreakingUpFight)
			{
				this.targetRotation = this.Yandere.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward * 0.5f);
				if (this.StudentID == 87 && this.CandyBar != null)
				{
					if (this.CharacterAnimation[this.BreakUpAnim].time >= 4f)
					{
						this.CandyBar.SetActive(false);
					}
					else if ((double)this.CharacterAnimation[this.BreakUpAnim].time >= 0.16666666666)
					{
						this.CandyBar.SetActive(true);
					}
				}
				if (this.CharacterAnimation[this.BreakUpAnim].time != 0f && this.CharacterAnimation[this.BreakUpAnim].time >= this.CharacterAnimation[this.BreakUpAnim].length)
				{
					this.ReturnToRoutine();
				}
			}
			if (this.Tripping)
			{
				this.MoveTowardsTarget(new Vector3(0f, 0f, base.transform.position.z));
				this.CharacterAnimation.CrossFade("trip_00");
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
				if (this.CharacterAnimation["trip_00"].time >= 0.5f && this.CharacterAnimation["trip_00"].time <= 5.5f && this.StudentManager.Gate.Crushing)
				{
					this.BecomeRagdoll();
					this.DeathType = DeathType.Weight;
					this.Ragdoll.Decapitated = true;
					UnityEngine.Object.Instantiate<GameObject>(this.SquishyBloodEffect, this.Head.position, Quaternion.identity);
				}
				if (this.CharacterAnimation["trip_00"].time >= this.CharacterAnimation["trip_00"].length)
				{
					this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.Distracted = true;
					this.MustTrip = false;
					this.Tripping = false;
					this.Routine = true;
					this.Tripped = true;
					this.Blind = false;
					this.BountyCollider.SetActive(false);
				}
			}
			if (this.SeekingMedicine)
			{
				if (this.StudentManager.Students[90] == null)
				{
					if (this.SeekMedicinePhase < 5)
					{
						this.SeekMedicinePhase = 5;
					}
				}
				else if ((!this.StudentManager.Students[90].gameObject.activeInHierarchy || this.StudentManager.Students[90].Dying) && this.SeekMedicinePhase < 5)
				{
					this.SeekMedicinePhase = 5;
				}
				if (this.SeekMedicinePhase == 0)
				{
					this.CurrentDestination = this.StudentManager.Students[90].transform;
					this.Pathfinding.target = this.StudentManager.Students[90].transform;
					this.SeekMedicinePhase++;
				}
				else if (this.SeekMedicinePhase == 1)
				{
					this.CharacterAnimation.CrossFade(this.SprintAnim);
					if (this.DistanceToDestination < 1f)
					{
						StudentScript studentScript4 = this.StudentManager.Students[90];
						this.CharacterAnimation.CrossFade(this.IdleAnim);
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.Pathfinding.speed = 0f;
						if (!studentScript4.Fleeing && !studentScript4.Guarding)
						{
							if (studentScript4.Investigating)
							{
								studentScript4.StopInvestigating();
							}
							this.StudentManager.UpdateStudents(studentScript4.StudentID);
							studentScript4.Pathfinding.canSearch = false;
							studentScript4.Pathfinding.canMove = false;
							studentScript4.RetrieveMedicinePhase = 0;
							studentScript4.RetreivingMedicine = true;
							studentScript4.Pathfinding.speed = 0f;
							studentScript4.CameraReacting = false;
							studentScript4.TargetStudent = this;
							studentScript4.Routine = false;
							this.Subtitle.UpdateLabel(SubtitleType.RequestMedicine, 0, 5f);
							this.SeekMedicinePhase++;
						}
						else
						{
							this.SeekMedicinePhase = 5;
						}
					}
				}
				else if (this.SeekMedicinePhase == 2)
				{
					StudentScript studentScript5 = this.StudentManager.Students[90];
					this.targetRotation = Quaternion.LookRotation(new Vector3(studentScript5.transform.position.x, base.transform.position.y, studentScript5.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					this.MedicineTimer += Time.deltaTime;
					if (this.MedicineTimer > 5f)
					{
						this.SeekMedicinePhase++;
						this.MedicineTimer = 0f;
						studentScript5.Pathfinding.canSearch = true;
						studentScript5.Pathfinding.canMove = true;
						studentScript5.RetrieveMedicinePhase++;
					}
				}
				else if (this.SeekMedicinePhase != 3)
				{
					if (this.SeekMedicinePhase == 4)
					{
						StudentScript studentScript6 = this.StudentManager.Students[90];
						this.targetRotation = Quaternion.LookRotation(new Vector3(studentScript6.transform.position.x, base.transform.position.y, studentScript6.transform.position.z) - base.transform.position);
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					}
					else if (this.SeekMedicinePhase == 5)
					{
						this.GoSitInInfirmary();
					}
				}
			}
			if (this.RetreivingMedicine)
			{
				if (this.RetrieveMedicinePhase == 0)
				{
					this.CharacterAnimation.CrossFade(this.IdleAnim);
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.TargetStudent.transform.position.x, base.transform.position.y, this.TargetStudent.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				}
				else if (this.RetrieveMedicinePhase == 1)
				{
					this.CharacterAnimation.CrossFade(this.WalkAnim);
					this.CurrentDestination = this.StudentManager.MedicineCabinet;
					this.Pathfinding.target = this.StudentManager.MedicineCabinet;
					this.Pathfinding.speed = this.WalkSpeed;
					this.RetrieveMedicinePhase++;
				}
				else if (this.RetrieveMedicinePhase == 2)
				{
					if (this.DistanceToDestination < 1f)
					{
						this.StudentManager.CabinetDoor.Locked = false;
						this.StudentManager.CabinetDoor.Open = true;
						this.StudentManager.CabinetDoor.Timer = 0f;
						this.CurrentDestination = this.TargetStudent.transform;
						this.Pathfinding.target = this.TargetStudent.transform;
						this.RetrieveMedicinePhase++;
					}
				}
				else if (this.RetrieveMedicinePhase == 3)
				{
					if (this.DistanceToDestination < 1f)
					{
						this.CurrentDestination = this.TargetStudent.transform;
						this.Pathfinding.target = this.TargetStudent.transform;
						this.RetrieveMedicinePhase++;
					}
				}
				else if (this.RetrieveMedicinePhase == 4)
				{
					if (this.DistanceToDestination < 1f)
					{
						this.CharacterAnimation.CrossFade("f02_giveItem_00");
						if (this.TargetStudent.Male)
						{
							this.TargetStudent.CharacterAnimation.CrossFade("eatItem_00");
						}
						else
						{
							this.TargetStudent.CharacterAnimation.CrossFade("f02_eatItem_00");
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.TargetStudent.SeekMedicinePhase++;
						this.RetrieveMedicinePhase++;
					}
				}
				else if (this.RetrieveMedicinePhase == 5)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.TargetStudent.transform.position.x, base.transform.position.y, this.TargetStudent.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					this.MedicineTimer += Time.deltaTime;
					if (this.MedicineTimer > 3f)
					{
						this.CharacterAnimation.CrossFade(this.WalkAnim);
						this.CurrentDestination = this.StudentManager.MedicineCabinet;
						this.Pathfinding.target = this.StudentManager.MedicineCabinet;
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.TargetStudent.SeekMedicinePhase++;
						this.RetrieveMedicinePhase++;
					}
				}
				else if (this.RetrieveMedicinePhase == 6 && this.DistanceToDestination < 1f)
				{
					this.StudentManager.CabinetDoor.Locked = true;
					this.StudentManager.CabinetDoor.Open = false;
					this.StudentManager.CabinetDoor.Timer = 0f;
					this.RetreivingMedicine = false;
					this.RetrieveMedicinePhase = 0;
					this.Routine = true;
				}
			}
			if (this.EatingSnack)
			{
				if (this.SnackPhase == 0)
				{
					this.CharacterAnimation.CrossFade(this.EatChipsAnim);
					this.SmartPhone.SetActive(false);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.SnackTimer += Time.deltaTime;
					if (this.SnackTimer > 10f)
					{
						UnityEngine.Object.Destroy(this.BagOfChips);
						bool flag10 = false;
						if (!this.StudentManager.Eighties && !this.StudentManager.MissionMode && this.StudentID == 11)
						{
							flag10 = true;
						}
						if (!flag10)
						{
							this.StudentManager.GetNearestFountain(this);
							if (this.Persona == PersonaType.Protective)
							{
								this.Pathfinding.speed = 4f;
							}
							this.Pathfinding.target = this.DrinkingFountain.DrinkPosition;
							this.CurrentDestination = this.DrinkingFountain.DrinkPosition;
							this.Pathfinding.canSearch = true;
							this.Pathfinding.canMove = true;
							this.SnackTimer = 0f;
						}
						this.SnackPhase++;
					}
				}
				else if (this.SnackPhase == 1)
				{
					if (this.Pathfinding.speed < 4f)
					{
						this.CharacterAnimation.CrossFade(this.WalkAnim);
					}
					else
					{
						this.CharacterAnimation.CrossFade(this.RunAnim);
					}
					if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
					{
						this.SmartPhone.SetActive(true);
					}
					if (this.DistanceToDestination < 1f)
					{
						this.SmartPhone.SetActive(false);
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.SnackPhase++;
					}
				}
				else if (this.SnackPhase == 2)
				{
					this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
					this.CharacterAnimation.CrossFade(this.DrinkFountainAnim);
					this.MoveTowardsTarget(this.DrinkingFountain.DrinkPosition.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.DrinkingFountain.DrinkPosition.rotation, 10f * Time.deltaTime);
					if (this.CharacterAnimation[this.DrinkFountainAnim].time >= this.CharacterAnimation[this.DrinkFountainAnim].length)
					{
						this.StopDrinking();
						this.CurrentDestination = this.Destinations[this.Phase];
						this.Pathfinding.target = this.Destinations[this.Phase];
					}
					else if (this.CharacterAnimation[this.DrinkFountainAnim].time > 0.5f && this.CharacterAnimation[this.DrinkFountainAnim].time < 1.5f)
					{
						if (!this.DrinkingFountain.Sabotaged)
						{
							this.DrinkingFountain.WaterStream.Play();
						}
						else
						{
							this.StopDrinking();
							UnityEngine.Object.Instantiate<GameObject>(this.DrinkingFountain.WaterCollider, base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
							this.DrinkingFountain.WaterBlast.Play();
						}
					}
				}
			}
			if (this.Dodging)
			{
				if (this.CharacterAnimation[this.DodgeAnim].time >= this.CharacterAnimation[this.DodgeAnim].length)
				{
					Debug.Log(this.Name + " has finished their dodging animation.");
					this.Dodging = false;
					if (!this.TurnOffRadio)
					{
						this.Routine = true;
					}
					else
					{
						Debug.Log("Hey. You. Walk.");
						this.CharacterAnimation.CrossFade(this.WalkAnim);
					}
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					if (this.GasWarned)
					{
						this.Yandere.Subtitle.UpdateLabel(SubtitleType.GasWarning, 2, 5f);
						this.GasWarned = false;
					}
				}
				else if (this.CharacterAnimation[this.DodgeAnim].time < 0.66666f)
				{
					this.MyController.Move(base.transform.forward * -1f * this.DodgeSpeed * Time.deltaTime);
					this.MyController.Move(Physics.gravity * 0.1f);
					if (this.DodgeSpeed > 0f)
					{
						this.DodgeSpeed = Mathf.MoveTowards(this.DodgeSpeed, 0f, Time.deltaTime * 3f);
					}
				}
			}
			if (this.Guarding && this.Corpse != null && this.Corpse.Concealed && !this.Alarmed)
			{
				this.Alarm = 200f;
				this.Yandere.PotentiallyMurderousTimer = 1f;
				this.Witnessed = StudentWitnessType.Murder;
			}
			if (!this.Guarding && this.InvestigatingBloodPool)
			{
				if (this.BloodPool != null)
				{
					if (Vector3.Distance(base.transform.position, new Vector3(this.BloodPool.position.x, base.transform.position.y, this.BloodPool.position.z)) < 1f)
					{
						bool flag11 = false;
						if (this.BloodPool == null || (this.WitnessedWeapon && this.BloodPool.parent != null) || (this.WitnessedBloodPool && this.BloodPool.parent == this.Yandere.RightHand) || this.BloodPool.transform.position != this.OriginalBloodPoolLocation || (this.WitnessedWeapon && this.BloodPool.GetComponent<WeaponScript>().Returner))
						{
							Debug.Log("ForgetAboutBloodPool() was called from this place in the code. 0");
							this.ForgetAboutBloodPool();
							flag11 = true;
						}
						if (!flag11)
						{
							this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
							this.CharacterAnimation[this.InspectBloodAnim].speed = 1f;
							this.CharacterAnimation.CrossFade(this.InspectBloodAnim);
							this.Pathfinding.canSearch = false;
							this.Pathfinding.canMove = false;
							this.Distracted = true;
							if (this.CharacterAnimation[this.InspectBloodAnim].time >= this.CharacterAnimation[this.InspectBloodAnim].length || this.Persona == PersonaType.Strict)
							{
								bool flag12 = false;
								bool flag13 = false;
								bool flag14 = false;
								if ((this.Club == ClubType.Cooking && this.CurrentAction == StudentActionType.ClubAction) || (this.StudentID == 15 && this.StudentManager.Eighties))
								{
									flag13 = true;
								}
								if (this.CurrentAction == StudentActionType.SitAndEatBento)
								{
									flag14 = true;
								}
								if (this.WitnessedWeapon)
								{
									bool flag15 = false;
									if (!this.Teacher && this.BloodPool.GetComponent<WeaponScript>().Metal && this.StudentManager.MetalDetectors)
									{
										flag15 = true;
									}
									if (this.Schoolwear == 2)
									{
										flag12 = true;
									}
									if (!this.WitnessedBloodyWeapon && this.StudentID > 1 && !flag15 && this.CurrentAction != StudentActionType.SitAndTakeNotes && this.Indoors && !flag13 && this.Club != ClubType.Delinquent && !flag12 && !flag14 && !this.BloodPool.GetComponent<WeaponScript>().Dangerous && this.BloodPool.GetComponent<WeaponScript>().Returner == null && this.BloodPool.GetComponent<WeaponScript>().Origin != null)
									{
										Debug.Log(this.Name + " is now picking up a weapon with intent to return it to its original location.");
										this.CharacterAnimation[this.PickUpAnim].time = 0f;
										this.BloodPool.GetComponent<WeaponScript>().Prompt.Hide();
										this.BloodPool.GetComponent<WeaponScript>().Prompt.enabled = false;
										this.BloodPool.GetComponent<WeaponScript>().enabled = false;
										this.BloodPool.GetComponent<WeaponScript>().Returner = this;
										Debug.Log("A WeaponScript has been disabled from this part of the code. 2");
										if (this.StudentID == 41 && !this.StudentManager.Eighties)
										{
											this.Subtitle.UpdateLabel(SubtitleType.Impatience, 6, 5f);
										}
										else
										{
											this.Subtitle.UpdateLabel(SubtitleType.ReturningWeapon, 0, 5f);
										}
										this.ReturningMisplacedWeapon = true;
										this.ReportingBlood = false;
										this.Distracted = false;
										this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
										this.Yandere.WeaponManager.ReturnWeaponID = this.BloodPool.GetComponent<WeaponScript>().GlobalID;
										this.Yandere.WeaponManager.ReturnStudentID = this.StudentID;
									}
								}
								this.InvestigatingBloodPool = false;
								this.WitnessCooldownTimer = 5f;
								if (this.WitnessedLimb)
								{
									this.SpawnAlarmDisc();
								}
								if (!this.ReturningMisplacedWeapon)
								{
									if (this.StudentManager.BloodReporter == this && this.WitnessedWeapon && !this.BloodPool.GetComponent<WeaponScript>().Dangerous)
									{
										this.StudentManager.BloodReporter = null;
									}
									if (this.StudentManager.BloodReporter == this && this.StudentID > 1)
									{
										if (this.Persona != PersonaType.Strict && this.Persona != PersonaType.Violent)
										{
											Debug.Log(this.Name + " has changed from their original Persona into a Teacher's Pet.");
											this.Persona = PersonaType.TeachersPet;
										}
										this.PersonaReaction();
									}
									else
									{
										this.Distracted = false;
										if (this.WitnessedWeapon && !this.WitnessedBloodyWeapon)
										{
											Debug.Log(this.Name + " is not going to bother reporting what they found.");
											this.StopInvestigating();
											this.CurrentDestination = this.Destinations[this.Phase];
											this.Pathfinding.target = this.Destinations[this.Phase];
											this.LastSuspiciousObject2 = this.LastSuspiciousObject;
											this.LastSuspiciousObject = this.BloodPool;
											this.Pathfinding.canSearch = true;
											this.Pathfinding.canMove = true;
											this.Pathfinding.speed = this.WalkSpeed;
											if (this.StudentID == 41 && !this.StudentManager.Eighties)
											{
												this.Subtitle.UpdateLabel(SubtitleType.Impatience, 6, 5f);
											}
											else if (this.StudentID == 15 && this.StudentManager.Eighties)
											{
												this.Subtitle.CustomText = "How weird. But, frankly, I don't care.";
												this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
											}
											else if (this.Club == ClubType.Delinquent)
											{
												this.Subtitle.UpdateLabel(SubtitleType.PetWeaponReaction, 2, 3f);
											}
											else if (this.BloodPool.GetComponent<WeaponScript>().Dangerous)
											{
												this.Subtitle.UpdateLabel(SubtitleType.PetWeaponReaction, 0, 3f);
											}
											else if (flag12)
											{
												this.Subtitle.CustomText = "Weird, but I'm not doing anything about it now; I'm in a swimsuit...";
												this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
											}
											else if (flag14)
											{
												this.Subtitle.CustomText = "Normally I'd put it back where it belongs, but right now I'm busy eating...";
												this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
											}
											else
											{
												this.Subtitle.UpdateLabel(SubtitleType.PetWeaponReaction, 1, 3f);
											}
											this.WitnessedSomething = false;
											this.WitnessedWeapon = false;
											this.Routine = true;
											this.BloodPool = null;
											if (this.StudentManager.BloodReporter == this)
											{
												this.StudentManager.BloodReporter = null;
											}
										}
										else
										{
											Debug.Log(this.Name + " just found something scary on the ground and is going to react to it now.");
											if (this.Persona != PersonaType.Strict && this.Persona != PersonaType.Violent)
											{
												Debug.Log(this.Name + " has changed from their original Persona into a Teacher's Pet.");
												this.Persona = PersonaType.TeachersPet;
											}
											this.PersonaReaction();
										}
									}
									this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
								}
							}
						}
					}
					else
					{
						if (this.Persona == PersonaType.Strict)
						{
							if (this.WitnessedWeapon && this.BloodPool.GetComponent<WeaponScript>().Returner)
							{
								this.Subtitle.UpdateLabel(SubtitleType.StudentFarewell, 0, 3f);
								this.CurrentDestination = this.Destinations[this.Phase];
								this.Pathfinding.target = this.Destinations[this.Phase];
								this.InvestigatingBloodPool = false;
								this.WitnessedBloodyWeapon = false;
								this.WitnessedBloodPool = false;
								this.WitnessedSomething = false;
								this.WitnessedWeapon = false;
								this.Distracted = false;
								this.Routine = true;
								this.BloodPool = null;
								this.WitnessCooldownTimer = 5f;
							}
							else if (this.BloodPool.parent == this.Yandere.RightHand || !this.BloodPool.gameObject.activeInHierarchy)
							{
								Debug.Log("Yandere-chan just picked up the weapon that was being investigated.");
								this.InvestigatingBloodPool = false;
								this.WitnessedBloodyWeapon = false;
								this.WitnessedBloodPool = false;
								this.WitnessedSomething = false;
								this.WitnessedWeapon = false;
								this.Distracted = false;
								this.Routine = true;
								if (this.BloodPool.GetComponent<WeaponScript>() != null && this.BloodPool.GetComponent<WeaponScript>().Suspicious)
								{
									this.WitnessCooldownTimer = 5f;
									this.AlarmTimer = 0f;
									this.Alarm = 200f;
								}
								this.BloodPool = null;
							}
						}
						if (this.BloodPool == null || (this.WitnessedWeapon && this.BloodPool.parent != null) || (this.WitnessedBloodPool && this.BloodPool.parent == this.Yandere.RightHand) || this.BloodPool.transform.position != this.OriginalBloodPoolLocation || (this.WitnessedWeapon && this.BloodPool.GetComponent<WeaponScript>().Returner))
						{
							Debug.Log("ForgetAboutBloodPool() was called from this place in the code. 1");
							this.ForgetAboutBloodPool();
						}
					}
				}
				else
				{
					Debug.Log("ForgetAboutBloodPool() was called from this place in the code. 2");
					this.ForgetAboutBloodPool();
				}
				if (this.FollowTarget != null && (Vector3.Distance(base.transform.position, this.FollowTarget.transform.position) < 5f || this.FollowTarget.transform.position.z < -50f) && this.FollowTarget.Attacked && this.FollowTarget.Alive && !this.FollowTarget.Tranquil && !this.Blind)
				{
					Debug.Log("Raibaru should be aware that Osana is being attacked.");
					this.ForgetAboutBloodPool();
					this.FocusOnYandere = true;
					this.AwareOfMurder = true;
					this.Alarm = 200f;
				}
			}
			if (this.ReturningMisplacedWeapon)
			{
				if (this.ReturningMisplacedWeaponPhase == 0)
				{
					this.CharacterAnimation.CrossFade(this.PickUpAnim);
					if ((this.Club == ClubType.Council || this.Teacher) && this.CharacterAnimation[this.PickUpAnim].time >= 0.33333f)
					{
						this.Handkerchief.SetActive(true);
					}
					if (this.CharacterAnimation[this.PickUpAnim].time >= 2f)
					{
						this.BloodPool.parent = this.RightHand;
						this.BloodPool.localPosition = new Vector3(0f, 0f, 0f);
						this.BloodPool.localEulerAngles = new Vector3(0f, 0f, 0f);
						if (this.Club != ClubType.Council && !this.Teacher)
						{
							this.BloodPool.GetComponent<WeaponScript>().FingerprintID = this.StudentID;
						}
					}
					if (this.CharacterAnimation[this.PickUpAnim].time >= this.CharacterAnimation[this.PickUpAnim].length)
					{
						this.CurrentDestination = this.BloodPool.GetComponent<WeaponScript>().Origin;
						this.Pathfinding.target = this.BloodPool.GetComponent<WeaponScript>().Origin;
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.Pathfinding.speed = this.WalkSpeed;
						this.Hurry = false;
						this.TargetWeaponDistance = this.BloodPool.GetComponent<WeaponScript>().TargetWeaponDistance;
						this.ReturningMisplacedWeaponPhase++;
					}
				}
				else
				{
					this.CharacterAnimation.CrossFade(this.WalkAnim);
					this.BloodPool.localPosition = new Vector3(0f, 0f, 0f);
					this.BloodPool.localEulerAngles = new Vector3(0f, 0f, 0f);
					if (this.DistanceToDestination < this.TargetWeaponDistance)
					{
						this.ReturnMisplacedWeapon();
					}
				}
			}
			if (this.SentToLocker && !this.CheckingNote)
			{
				this.CharacterAnimation.CrossFade(this.RunAnim);
			}
			if (this.Stripping)
			{
				this.CharacterAnimation.CrossFade(this.StripAnim);
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
				if (this.CharacterAnimation[this.StripAnim].time >= 1.5f)
				{
					if (this.Schoolwear != 1)
					{
						Debug.Log(this.Name + "is calling ChangeSchoolwear() from here, specifically.");
						this.Schoolwear = 1;
						this.ChangeSchoolwear();
						this.WalkAnim = this.OriginalWalkAnim;
					}
					if (this.CharacterAnimation[this.StripAnim].time > this.CharacterAnimation[this.StripAnim].length)
					{
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.Stripping = false;
						this.Routine = true;
					}
				}
			}
			if (this.SenpaiWitnessingRivalDie)
			{
				this.Fleeing = false;
				if (this.DistanceToDestination < 1f)
				{
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
				}
				if (this.WitnessRivalDiePhase == 0)
				{
					this.CharacterAnimation.CrossFade("witnessPoisoning_00");
					this.MoveTowardsTarget(this.CurrentDestination.position);
					this.Chopsticks[0].SetActive(true);
					this.Chopsticks[1].SetActive(true);
					this.Bento.SetActive(true);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, 10f * Time.deltaTime);
					if (this.CharacterAnimation["witnessPoisoning_00"].time >= 18.5f && this.Bento.activeInHierarchy)
					{
						this.Chopsticks[0].SetActive(false);
						this.Chopsticks[1].SetActive(false);
						this.Bento.SetActive(false);
						this.WitnessRivalDiePhase++;
					}
				}
				else if (this.WitnessRivalDiePhase == 1)
				{
					if (this.CharacterAnimation["witnessPoisoning_00"].time >= 22.5f)
					{
						this.Subtitle.UpdateLabel(SubtitleType.SenpaiRivalDeathReaction, 0, 8f);
						this.WitnessRivalDiePhase++;
					}
				}
				else if (this.WitnessRivalDiePhase == 2)
				{
					if (this.CharacterAnimation["witnessPoisoning_00"].time >= this.CharacterAnimation["witnessPoisoning_00"].length)
					{
						base.transform.position = new Vector3(this.Hips.position.x, base.transform.position.y, this.Hips.position.z);
						Physics.SyncTransforms();
						this.CharacterAnimation.Play("senpaiRivalCorpseReaction_00");
						this.TargetDistance = 1.5f;
						this.WitnessRivalDiePhase++;
						this.RivalDeathTimer = 0f;
					}
				}
				else if (this.WitnessRivalDiePhase == 3)
				{
					this.TargetDistance = 1.5f;
					if (this.DistanceToDestination <= this.TargetDistance)
					{
						this.CharacterAnimation.Play("senpaiRivalCorpseReaction_00");
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						if (this.WitnessedCorpse)
						{
							base.transform.LookAt(this.StudentManager.Students[this.StudentManager.RivalID].Head.position);
							base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y - 90f, 0f);
						}
					}
					if (this.RivalDeathTimer == 0f)
					{
						this.Subtitle.UpdateLabel(SubtitleType.SenpaiRivalDeathReaction, 2, 15f);
					}
					this.RivalDeathTimer += Time.deltaTime;
					if (this.CharacterAnimation["senpaiRivalCorpseReaction_00"].time >= this.CharacterAnimation["senpaiRivalCorpseReaction_00"].length)
					{
						if (!this.Phoneless)
						{
							this.Subtitle.UpdateLabel(SubtitleType.SenpaiRivalDeathReaction, 3, 15f);
							this.CharacterAnimation.CrossFade("kneelPhone_00");
							this.SmartPhone.SetActive(true);
						}
						this.WitnessRivalDiePhase++;
						this.RivalDeathTimer = 0f;
					}
				}
				else if (this.WitnessRivalDiePhase == 4)
				{
					this.CharacterAnimation.CrossFade("kneelPhone_00");
					this.RivalDeathTimer += Time.deltaTime;
					if (this.Phoneless)
					{
						this.RivalDeathTimer += 100f;
					}
					if (this.RivalDeathTimer > this.Subtitle.SenpaiRivalDeathReactionClips[3].length)
					{
						this.Subtitle.UpdateLabel(SubtitleType.SenpaiRivalDeathReaction, 4, 10f);
						this.CharacterAnimation.CrossFade("senpaiRivalCorpseCry_00");
						this.SmartPhone.SetActive(false);
						this.WitnessRivalDiePhase++;
						if (!this.StudentManager.Jammed && !this.Phoneless)
						{
							Debug.Log(this.Name + " just called the cops.");
							this.Police.Called = true;
							this.Police.Show = true;
						}
					}
				}
				if ((this.Yandere.Lifting || this.Yandere.Dragging) && this.Yandere.TargetStudent == this.StudentManager.Students[this.StudentManager.RivalID])
				{
					this.Alarm = 200f;
				}
				if (this.StudentManager.Students[this.StudentManager.RivalID].Ragdoll.Concealed)
				{
					Debug.Log("A corpse that was being mourned has been concealed in a trash bag. " + this.Name + " should now react as if they know the player is a murderer.");
					if (!this.Yandere.Noticed)
					{
						this.Alarm = 200f;
						this.Alarmed = false;
						this.MurdersWitnessed = 1;
						this.WitnessedMurder = true;
						this.Yandere.PotentiallyMurderousTimer = 1f;
						this.Witnessed = StudentWitnessType.Murder;
						this.BecomeAlarmed();
						this.SenpaiWitnessingRivalDie = false;
					}
				}
			}
			if (this.SpecialRivalDeathReaction)
			{
				if (this.Corpse == null)
				{
					this.Corpse = this.StudentManager.Students[this.CorpseID].Ragdoll;
				}
				this.CurrentDestination = this.Corpse.Student.Hips;
				this.Pathfinding.target = this.Corpse.Student.Hips;
				if (this.WitnessRivalDiePhase == 1)
				{
					if (this.DistanceToDestination <= 1f)
					{
						if (!this.Male)
						{
							this.CharacterAnimation.CrossFade("f02_friendCorpseReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade("senpaiRivalCorpseReaction_00");
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						if (this.CorpseHead == null)
						{
							this.CorpseHead = this.Corpse.Student.Head;
						}
						base.transform.LookAt(this.CorpseHead.position);
						base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y - 90f, 0f);
					}
					else
					{
						this.CharacterAnimation.CrossFade(this.RunAnim);
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.Pathfinding.speed = 4f;
					}
					if (this.RivalDeathTimer == 0f)
					{
						this.Subtitle.PreviousSubtitle = SubtitleType.AcceptFood;
						if (this.StudentID == this.StudentManager.ObstacleID)
						{
							Debug.Log("Raibaru is reacting to Osana's corpse.");
							this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 2, 15f);
							this.StudentToMourn = this.StudentManager.Students[11];
						}
						else if (this.StudentID == this.StudentManager.RivalID)
						{
							this.Subtitle.UpdateLabel(SubtitleType.OsanaObstacleDeathReaction, 2, 15f);
						}
						else if (!this.StudentManager.Eighties && this.StudentID > 1 && this.StudentID < 4)
						{
							this.Subtitle.CustomText = "Sister!! Sister, answer me!! Wake up, please, wake up!! Don't do this!! Oh, no, this can't be happening!! NO!! ...no...";
							this.Subtitle.UpdateLabel(SubtitleType.Custom, 2, 15f);
						}
					}
					this.RivalDeathTimer += Time.deltaTime;
					if ((!this.Male && this.CharacterAnimation["f02_friendCorpseReaction_00"].time >= this.CharacterAnimation["f02_friendCorpseReaction_00"].length) || (this.Male && this.CharacterAnimation["senpaiRivalCorpseReaction_00"].time >= this.CharacterAnimation["senpaiRivalCorpseReaction_00"].length))
					{
						if (!this.Phoneless)
						{
							if (this.StudentID == this.StudentManager.ObstacleID)
							{
								this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 3, 10f);
							}
							else if (this.StudentID == this.StudentManager.RivalID)
							{
								this.Subtitle.UpdateLabel(SubtitleType.OsanaObstacleDeathReaction, 3, 10f);
							}
							else if (!this.StudentManager.Eighties && this.StudentID > 1 && this.StudentID < 4)
							{
								this.Subtitle.CustomText = "...hello...police ? ...I'm...a student at Akademi...my sister is...dead...I'm not sure what happened...please...send someone...anyone...";
								this.Subtitle.UpdateLabel(SubtitleType.Custom, 3, 10f);
							}
							if (this.Male)
							{
								this.CharacterAnimation.CrossFade("kneelPhone_00");
							}
							else
							{
								this.CharacterAnimation.CrossFade("f02_kneelPhone_00");
							}
							this.SmartPhone.SetActive(true);
						}
						else
						{
							if (this.StudentID == this.StudentManager.ObstacleID)
							{
								this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 4, 10f);
							}
							else if (this.StudentID == this.StudentManager.RivalID)
							{
								this.Subtitle.UpdateLabel(SubtitleType.OsanaObstacleDeathReaction, 4, 10f);
							}
							else if (!this.StudentManager.Eighties && this.StudentID > 1 && this.StudentID < 4)
							{
								this.Subtitle.CustomText = "(Sobbing)";
								this.Subtitle.UpdateLabel(SubtitleType.Custom, 4, 10f);
							}
							if (!this.Male)
							{
								this.CharacterAnimation.CrossFade("f02_friendCorpseCry_00");
							}
							else
							{
								this.CharacterAnimation.CrossFade("senpaiRivalCorpseCry_00");
							}
							this.WitnessRivalDiePhase++;
						}
						this.WitnessRivalDiePhase++;
						this.RivalDeathTimer = 0f;
					}
				}
				else if (this.WitnessRivalDiePhase == 2)
				{
					this.RivalDeathTimer += Time.deltaTime;
					if (this.RivalDeathTimer > this.Subtitle.OsanaObstacleDeathReactionClips[3].length)
					{
						if (this.StudentID == this.StudentManager.ObstacleID)
						{
							this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 4, 10f);
						}
						else if (this.StudentID == this.StudentManager.RivalID)
						{
							this.Subtitle.UpdateLabel(SubtitleType.OsanaObstacleDeathReaction, 4, 10f);
						}
						else
						{
							this.Subtitle.CustomText = "(Sobbing)";
							this.Subtitle.UpdateLabel(SubtitleType.Custom, 4, 10f);
						}
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiRivalCorpseCry_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade("f02_friendCorpseCry_00");
						}
						this.SmartPhone.SetActive(false);
						this.WitnessRivalDiePhase++;
						if (!this.StudentManager.Jammed)
						{
							Debug.Log(this.Name + " just called the cops.");
							this.Police.Called = true;
							this.Police.Show = true;
						}
					}
				}
				if ((this.Yandere.Lifting || this.Yandere.Dragging) && this.Yandere.TargetStudent == this.StudentToMourn)
				{
					this.Alarm = 200f;
				}
				if (this.StudentToMourn.Ragdoll.Concealed)
				{
					Debug.Log("A corpse that was being mourned has been concealed in a trash bag. " + this.Name + " should now react as if they know the player is a murderer.");
					if (!this.Yandere.Noticed)
					{
						this.Alarm = 200f;
						this.Alarmed = false;
						this.MurdersWitnessed = 1;
						this.WitnessedMurder = true;
						this.Yandere.PotentiallyMurderousTimer = 1f;
						this.Witnessed = StudentWitnessType.Murder;
						this.WitnessMurder();
						this.SpecialRivalDeathReaction = false;
					}
				}
			}
			if (this.SolvingPuzzle)
			{
				this.PuzzleTimer += Time.deltaTime;
				this.CharacterAnimation.CrossFade(this.PuzzleAnim);
				this.PuzzleCube.transform.Rotate(new Vector3(360f, 360f, 360f), Time.deltaTime * 100f);
				if (this.PuzzleTimer > 30f)
				{
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.PuzzleTimer = 0f;
					this.Routine = true;
					this.DropPuzzle();
				}
			}
			if (this.GoAway)
			{
				this.GoAwayTimer += Time.deltaTime;
				if (this.GoAwayTimer > this.GoAwayLimit)
				{
					Debug.Log("A student's GoAwayTimer has run out.");
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.GoAwayLimit = 10f;
					this.GoAwayTimer = 0f;
					this.Hurry = this.WasHurrying;
					this.Distracted = false;
					this.GoAway = false;
					this.Routine = true;
					if (!this.Hurry)
					{
						this.Pathfinding.speed = 1f;
					}
				}
			}
			if (this.TakingOutTrash)
			{
				if (this.TrashPhase == 0)
				{
					this.CurrentDestination = this.TrashDestination;
					this.Pathfinding.target = this.TrashDestination;
					this.EmptyHands();
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.CharacterAnimation.CrossFade(this.WalkAnim);
					this.TrashPhase++;
				}
				else if (this.TrashPhase == 1)
				{
					if (Mathf.Abs(base.transform.position.y - this.TrashDestination.position.y) <= 2.5f && Vector3.Distance(base.transform.position, new Vector3(this.TrashDestination.position.x, base.transform.position.y, this.TrashDestination.position.z)) < 1f)
					{
						this.TrashDestination.parent = this.ItemParent;
						this.TrashDestination.localEulerAngles = new Vector3(90f, 0f, 0f);
						this.TrashDestination.localPosition = new Vector3(0f, 0.05f, -0.45f);
						this.CurrentDestination = this.Yandere.Incinerator.transform;
						this.Pathfinding.target = this.Yandere.Incinerator.transform;
						this.TrashPhase++;
					}
				}
				else if (this.TrashPhase == 2 && this.DistanceToDestination < 2.5f)
				{
					this.Yandere.Incinerator.DumpGarbageBag(this.TrashDestination.GetComponent<PickUpScript>());
					this.TakingOutTrash = false;
					this.Routine = true;
					this.GetDestinations();
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.CurrentDestination = this.Destinations[this.Phase];
				}
			}
			if (this.FocusOnYandere)
			{
				this.LookAtYandere();
			}
			else if (this.FocusOnStudent)
			{
				if (!this.InEvent)
				{
					this.CharacterAnimation.CrossFade(this.LeanAnim);
				}
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.WeirdStudent.position.x, base.transform.position.y, this.WeirdStudent.position.z) - base.transform.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
			}
		}
		if (this.InstantNoticeTimer > 0f)
		{
			this.InstantNoticeTimer = Mathf.MoveTowards(this.InstantNoticeTimer, 0f, Time.deltaTime);
		}
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x001E7540 File Offset: 0x001E5740
	private void LookAtYandere()
	{
		this.CharacterAnimation.CrossFade(this.LeanAnim);
		this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
		if (this.FollowTarget != null && !this.AwareOfMurder && Vector3.Distance(base.transform.position, this.FollowTarget.transform.position) < 4f && this.FollowTarget.Attacked && this.FollowTarget.Alive && !this.FollowTarget.Tranquil && !this.Blind)
		{
			Debug.Log("Raibaru should be aware that Osana is being attacked.");
			this.AwareOfMurder = true;
			this.Alarm = 200f;
		}
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x001E766C File Offset: 0x001E586C
	private void UpdateVisibleCorpses()
	{
		this.VisibleCorpses.Clear();
		this.ID = 0;
		while (this.ID < this.Police.Corpses)
		{
			RagdollScript ragdollScript = this.Police.CorpseList[this.ID];
			if (ragdollScript != null && !ragdollScript.Hidden && !ragdollScript.Concealed)
			{
				Collider collider = ragdollScript.AllColliders[0];
				bool flag = false;
				if (collider.transform.position.y < base.transform.position.y + 4f)
				{
					flag = true;
				}
				if (flag && this.CanSeeObject(collider.gameObject, collider.transform.position, this.CorpseLayers, this.Mask))
				{
					RagdollScript component = collider.gameObject.transform.parent.parent.parent.GetComponent<RagdollScript>();
					if (!component.Concealed)
					{
					}
					if (component != null && !component.Concealed)
					{
						this.VisibleCorpses.Add(component.StudentID);
						this.Corpse = component;
						if ((this.Club == ClubType.Delinquent && this.Corpse.Student.Club == ClubType.Bully) || (this.Persona == PersonaType.Fragile && this.Corpse.Student.Club == ClubType.Bully))
						{
							Debug.Log("At this moment, a delinquent is reacting to the corpse of a bully. 1");
							this.ScaredAnim = this.EvilWitnessAnim;
							this.Persona = PersonaType.Evil;
						}
						if (this.Persona == PersonaType.TeachersPet && this.StudentManager.Reporter == null && !this.Police.Called)
						{
							this.StudentManager.CorpseLocation.position = this.Corpse.AllColliders[0].transform.position;
							this.StudentManager.CorpseLocation.LookAt(base.transform.position);
							this.StudentManager.CorpseLocation.Translate(this.StudentManager.CorpseLocation.forward * -1f);
							this.StudentManager.LowerCorpsePosition();
							this.StudentManager.Reporter = this;
							this.ReportingMurder = true;
							this.DetermineCorpseLocation();
							this.Pathfinding.canSearch = false;
							this.Pathfinding.canMove = false;
							this.Pathfinding.speed = 0f;
							this.Fleeing = false;
						}
					}
				}
			}
			this.ID++;
		}
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x001E78F8 File Offset: 0x001E5AF8
	private void UpdateVisibleBlood()
	{
		this.ID = 0;
		while (this.ID < this.StudentManager.Blood.Length && this.BloodPool == null)
		{
			Collider collider = this.StudentManager.Blood[this.ID];
			if (collider != null && Vector3.Distance(base.transform.position, collider.transform.position) < this.VisionDistance && this.CanSeeObject(collider.gameObject, collider.transform.position))
			{
				this.BloodPool = collider.transform;
				this.OriginalBloodPoolLocation = this.BloodPool.transform.position;
				this.WitnessedBloodPool = true;
				if (this.Club != ClubType.Delinquent && this.StudentManager.BloodReporter == null && !this.Police.Called && !this.LostTeacherTrust)
				{
					this.StudentManager.BloodLocation.position = this.BloodPool.position;
					this.StudentManager.BloodLocation.LookAt(new Vector3(base.transform.position.x, this.StudentManager.BloodLocation.position.y, base.transform.position.z));
					this.StudentManager.BloodLocation.Translate(this.StudentManager.BloodLocation.forward * -1f);
					this.StudentManager.LowerBloodPosition();
					this.StudentManager.BloodReporter = this;
					this.ReportingBlood = true;
					Debug.Log(this.Name + " has just appointed themself as a BloodReporter.");
					if (this.BloodPool.gameObject.GetComponent<BloodPoolScript>() != null)
					{
						Debug.Log("Attempting to mark the blood pool as ''under investigation''.");
						this.BloodPool.gameObject.GetComponent<BloodPoolScript>().UnderInvestigation = true;
					}
					this.DetermineBloodLocation();
				}
			}
			this.ID++;
		}
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x001E7B10 File Offset: 0x001E5D10
	private void UpdateVisibleLimbs()
	{
		this.ID = 0;
		while (this.ID < this.StudentManager.Limbs.Length && this.BloodPool == null)
		{
			Collider collider = this.StudentManager.Limbs[this.ID];
			if (collider != null && this.CanSeeObject(collider.gameObject, collider.transform.position))
			{
				this.BloodPool = collider.transform;
				this.OriginalBloodPoolLocation = this.BloodPool.transform.position;
				this.WitnessedLimb = true;
				if (this.Club != ClubType.Delinquent && this.StudentManager.BloodReporter == null && !this.Police.Called && !this.LostTeacherTrust)
				{
					this.StudentManager.BloodLocation.position = this.BloodPool.position;
					this.StudentManager.BloodLocation.LookAt(new Vector3(base.transform.position.x, this.StudentManager.BloodLocation.position.y, base.transform.position.z));
					this.StudentManager.BloodLocation.Translate(this.StudentManager.BloodLocation.forward * -1f);
					this.StudentManager.LowerBloodPosition();
					this.StudentManager.BloodReporter = this;
					this.ReportingBlood = true;
					this.DetermineBloodLocation();
				}
			}
			this.ID++;
		}
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x001E7CB8 File Offset: 0x001E5EB8
	private void UpdateVisibleWeapons()
	{
		this.ID = 0;
		while (this.ID < this.Yandere.WeaponManager.Weapons.Length)
		{
			if (this.Yandere.WeaponManager.Weapons[this.ID] != null && (this.Yandere.WeaponManager.Weapons[this.ID].Blood.enabled || (this.Yandere.WeaponManager.Weapons[this.ID].Misplaced && this.Yandere.WeaponManager.Weapons[this.ID].transform.parent == null)))
			{
				if (!(this.BloodPool == null))
				{
					break;
				}
				if (this.Yandere.WeaponManager.Weapons[this.ID].transform != this.LastSuspiciousObject && this.Yandere.WeaponManager.Weapons[this.ID].transform != this.LastSuspiciousObject2 && this.Yandere.WeaponManager.Weapons[this.ID].enabled)
				{
					Collider myCollider = this.Yandere.WeaponManager.Weapons[this.ID].MyCollider;
					if (myCollider != null && this.CanSeeObject(myCollider.gameObject, myCollider.transform.position))
					{
						if ((!this.StudentManager.Eighties && this.StudentID == 48 && this.Yandere.WeaponManager.Weapons[this.ID].WeaponID == 12) || (!this.StudentManager.Eighties && this.StudentID == 50 && this.Yandere.WeaponManager.Weapons[this.ID].WeaponID == 24))
						{
							break;
						}
						this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
						Debug.Log(this.Name + " has seen a dropped weapon on the ground.");
						this.CheckForBento();
						this.BloodPool = myCollider.transform;
						this.OriginalBloodPoolLocation = this.BloodPool.transform.position;
						if (this.Yandere.WeaponManager.Weapons[this.ID].Blood.enabled)
						{
							this.WitnessedBloodyWeapon = true;
						}
						Debug.Log("WitnessedWeapon SHOULD be set to true here...");
						this.WitnessedWeapon = true;
						if (this.Club != ClubType.Delinquent && this.StudentManager.BloodReporter == null && !this.Police.Called && !this.LostTeacherTrust)
						{
							Debug.Log("Current WalkAnim is: " + this.WalkAnim + ". Saving BeforeReturnAnim.");
							this.StudentManager.BloodLocation.position = this.BloodPool.position;
							this.StudentManager.BloodLocation.LookAt(new Vector3(base.transform.position.x, this.StudentManager.BloodLocation.position.y, base.transform.position.z));
							this.StudentManager.BloodLocation.Translate(this.StudentManager.BloodLocation.forward * -1f);
							this.StudentManager.LowerBloodPosition();
							this.StudentManager.BloodReporter = this;
							this.ReportingBlood = true;
							this.BeforeReturnAnim = this.WalkAnim;
							this.WalkAnim = this.OriginalWalkAnim;
							this.WasHurrying = this.Hurry;
							this.DetermineBloodLocation();
						}
					}
				}
			}
			this.ID++;
		}
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x001E8084 File Offset: 0x001E6284
	private void UpdateVision()
	{
		bool flag = false;
		if (this.Distracted)
		{
			flag = true;
			if (this.StudentManager.BloodReporter == this)
			{
				flag = false;
			}
		}
		if (this.AmnesiaTimer > 0f)
		{
			flag = true;
			this.AmnesiaTimer = Mathf.MoveTowards(this.AmnesiaTimer, 0f, Time.deltaTime);
			if (this.AmnesiaTimer == 0f)
			{
				Debug.Log("Student should now be returning to their previous routine.");
			}
		}
		if (!flag)
		{
			bool flag2 = true;
			if (this.Yandere.Pursuer == null && this.Yandere.Pursuer == this)
			{
				flag2 = false;
			}
			if (this.WitnessedMurder || this.CheckingNote || this.Shoving || this.Slave || this.Struggling || !flag2 || this.Drownable || this.Fighting)
			{
				this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
				return;
			}
			if (this.Police.Corpses > 0)
			{
				if (!this.Blind && !this.AwareOfCorpse)
				{
					this.UpdateVisibleCorpses();
				}
				if (this.VisibleCorpses.Count > 0)
				{
					if (!this.WitnessedCorpse)
					{
						Debug.Log(this.Name + " discovered the corpse of " + this.Corpse.Student.Name);
						if (this.ReturningMisplacedWeapon)
						{
							this.DropMisplacedWeapon();
						}
						if (this.BloodPool != null)
						{
							Debug.Log("ForgetAboutBloodPool() was called from this place in the code.");
							this.ForgetAboutBloodPool();
						}
						this.Fleeing = false;
						this.Police.StudentFoundCorpse = true;
						this.SawCorpseThisFrame = true;
						if ((this.Club == ClubType.Delinquent || this.Persona == PersonaType.Fragile) && this.Corpse.Student.Club == ClubType.Bully)
						{
							Debug.Log(this.Name + " witnessed the corpse of a bully, specifically.");
							this.FoundEnemyCorpse = true;
						}
						if (this.Persona == PersonaType.Sleuth)
						{
							if (this.Sleuthing)
							{
								this.Persona = PersonaType.PhoneAddict;
								this.SmartPhone.SetActive(true);
							}
							else if (this.StudentManager.Eighties)
							{
								this.Persona = PersonaType.LandlineUser;
							}
							else
							{
								this.Persona = PersonaType.SocialButterfly;
							}
						}
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						if (!this.Male)
						{
							this.CharacterAnimation["f02_smile_00"].weight = 0f;
						}
						Debug.Log("Manually setting Alarm to 200.");
						this.AlarmTimer = 0f;
						this.Alarm = 200f;
						this.LastKnownCorpse = this.Corpse.AllColliders[0].transform.position;
						this.WitnessedBloodyWeapon = false;
						this.WitnessedBloodPool = false;
						this.WitnessedSomething = false;
						this.WitnessedWeapon = false;
						this.WitnessedCorpse = true;
						this.WitnessedLimb = false;
						this.Yandere.NotificationManager.CustomText = this.Name + " found a corpse!";
						this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
						this.SetOutlineColor(new Color(1f, 1f, 0f, 1f));
						this.SummonWitnessCamera();
						if (this.StudentManager.BloodReporter == this)
						{
							this.StudentManager.BloodReporter = null;
							this.ReportingBlood = false;
							this.Fleeing = false;
						}
						if (this.Distracting || this.ResumeDistracting)
						{
							Debug.Log(this.Name + " should be forgetting about ''Distracting'' right now.");
							if (this.DistractionTarget != null)
							{
								this.DistractionTarget.TargetedForDistraction = false;
							}
							this.ResumeDistracting = false;
							this.Distracting = false;
						}
						this.InvestigatingBloodPool = false;
						this.Investigating = false;
						this.EatingSnack = false;
						this.Threatened = false;
						this.SentHome = false;
						this.Routine = false;
						this.CheckingNote = false;
						this.SentToLocker = false;
						this.Meeting = false;
						this.MeetTimer = 0f;
						if (this.Persona == PersonaType.Spiteful && ((this.Bullied && this.Corpse.Student.Club == ClubType.Bully) || this.Corpse.Student.Bullied))
						{
							Debug.Log("At this moment, a delinquent is reacting to the corpse of a bully. 2");
							this.ScaredAnim = this.EvilWitnessAnim;
							this.Persona = PersonaType.Evil;
						}
						this.ForgetRadio();
						if (this.Wet)
						{
							this.Persona = PersonaType.Loner;
						}
						if (this.Corpse.Disturbing)
						{
							if (Vector3.Distance(base.transform.position, this.Corpse.transform.position) < 5f)
							{
								this.WalkBackTimer = 5f;
								this.WalkBack = true;
								this.Routine = false;
							}
							if (this.Corpse.BurningAnimation || this.Corpse.ElectrocutionAnimation)
							{
								this.Hesitation = 0.5f;
							}
							if (this.Corpse.ChokingAnimation)
							{
								this.Hesitation = 0.6f;
							}
							if (this.Corpse.RobotDeath)
							{
								this.Hesitation = 0.66666f;
							}
							if (this.Corpse.MurderSuicideAnimation)
							{
								this.WitnessedMindBrokenMurder = true;
								this.Hesitation = 1f;
							}
						}
						if (this.Corpse.Student.Electrified)
						{
							Debug.Log(this.Name + " is witnessing a person being electrocuted.");
							this.ElectrocutionVictim = this.Corpse.Student.StudentID;
							Vector3 headPosition = this.Yandere.HeadPosition;
							if (this.CanSeeObject(this.Yandere.gameObject, headPosition))
							{
								Debug.Log("Yandere-chan is in the vicinity.");
								if (this.Yandere.PotentiallyMurderousTimer > 0f)
								{
									Debug.Log("Yandere-chan just threw a car battery, which means she just deliberately killed someone!");
									this.WitnessMurder();
								}
							}
						}
						if (this.Corpse.Student.Burning)
						{
							Debug.Log(this.Name + " is witnessing a person burn to death.");
							Vector3 headPosition2 = this.Yandere.HeadPosition;
							if (this.CanSeeObject(this.Yandere.gameObject, headPosition2))
							{
								Debug.Log("Yandere-chan is in the vicinity.");
								if (this.Yandere.PotentiallyMurderousTimer > 0f)
								{
									Debug.Log("Yandere-chan just ran into the victim while holding a flame, which means she just deliberately killed someone!");
									this.WitnessMurder();
								}
							}
						}
						this.StudentManager.UpdateMe(this.StudentID);
						if (!this.Teacher)
						{
							this.SpawnAlarmDisc();
						}
						else
						{
							this.AlarmTimer = 3f;
						}
						ParticleSystem.EmissionModule emission = this.Hearts.emission;
						if (this.Talking)
						{
							this.DialogueWheel.End();
							emission.enabled = false;
							this.Pathfinding.canSearch = true;
							this.Pathfinding.canMove = true;
							this.Obstacle.enabled = false;
							this.Talk.enabled = false;
							this.Talking = false;
							this.Waiting = false;
							this.StudentManager.EnablePrompts();
						}
						if (this.Following)
						{
							emission.enabled = false;
							this.FollowCountdown.gameObject.SetActive(false);
							this.Yandere.Follower = null;
							this.Yandere.Followers--;
							this.Following = false;
						}
					}
					if (this.Corpse.Dragged || this.Corpse.Dumped)
					{
						if (this.Teacher)
						{
							this.Subtitle.UpdateLabel(SubtitleType.TeacherMurderReaction, UnityEngine.Random.Range(1, 3), 3f);
						}
						if (!this.Yandere.Egg && !this.Yandere.Invisible)
						{
							this.WitnessMurder();
						}
					}
				}
			}
			if (this.VisibleCorpses.Count == 0 && !this.WitnessedCorpse)
			{
				if (this.WitnessCooldownTimer > 0f)
				{
					this.WitnessCooldownTimer = Mathf.MoveTowards(this.WitnessCooldownTimer, 0f, Time.deltaTime);
				}
				else if ((this.StudentID == this.StudentManager.CurrentID || (this.Persona == PersonaType.Strict && this.Fleeing)) && !this.Wet && !this.Guarding && !this.IgnoreBlood && !this.InvestigatingPossibleDeath && !this.Spraying && !this.Emetic && !this.Threatened && !this.Sedated && !this.Headache && !this.SentHome && !this.Slave && !this.Talking && !this.Confessing && !this.SentToLocker && !this.Meeting && !this.IgnoringThingsOnGround && !this.RetreivingMedicine && !this.StudentManager.KokonaTutorial)
				{
					if (this.BloodPool == null && this.StudentManager.Police.LimbParent.childCount > 0)
					{
						this.UpdateVisibleLimbs();
					}
					if (this.BloodPool == null && (this.Police.BloodyWeapons > 0 || this.Yandere.WeaponManager.MisplacedWeapons > 0) && !this.InvestigatingPossibleLimb && !this.TakingOutTrash && !this.Alarmed && !this.InEvent && !this.Distracted && !this.InvestigatingPossibleBlood && !this.ChangingShoes && !this.EatingSnack && this.Persona != PersonaType.Violent && (this.MyPlate == null || (this.MyPlate != null && this.MyPlate.parent != this.RightHand)))
					{
						this.UpdateVisibleWeapons();
					}
					if (this.BloodPool == null)
					{
						if (this.StudentManager.Police.BloodParent.childCount > 0 && !this.InvestigatingPossibleLimb)
						{
							this.UpdateVisibleBlood();
						}
					}
					else if (!this.WitnessedSomething)
					{
						Debug.Log(this.Name + " saw something suspicious on the ground.");
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						if (!this.Male)
						{
							this.CharacterAnimation["f02_smile_00"].weight = 0f;
						}
						Debug.Log("Manually setting Alarm to 200.");
						this.AlarmTimer = 0f;
						this.Alarm = 200f;
						this.LastKnownBlood = this.BloodPool.transform.position;
						this.NotAlarmedByYandereChan = true;
						this.WitnessedSomething = true;
						this.Investigating = false;
						this.EatingSnack = false;
						this.Threatened = false;
						this.SentHome = false;
						this.Routine = false;
						this.ForgetRadio();
						this.StudentManager.UpdateMe(this.StudentID);
						if (this.Teacher)
						{
							this.AlarmTimer = 3f;
						}
						ParticleSystem.EmissionModule emission2 = this.Hearts.emission;
						if (this.Talking)
						{
							this.DialogueWheel.End();
							emission2.enabled = false;
							this.Pathfinding.canSearch = true;
							this.Pathfinding.canMove = true;
							this.Obstacle.enabled = false;
							this.Talk.enabled = false;
							this.Talking = false;
							this.Waiting = false;
							this.StudentManager.EnablePrompts();
						}
						if (this.Following)
						{
							emission2.enabled = false;
							this.FollowCountdown.gameObject.SetActive(false);
							this.Yandere.Follower = null;
							this.Yandere.Followers--;
							this.Following = false;
						}
					}
				}
			}
			this.PreviousAlarm = this.Alarm;
			if (this.DistanceToPlayer < this.VisionDistance + this.VisionBonus - this.ChameleonBonus)
			{
				if (!this.Talking && !this.Spraying && !this.SentHome && !this.Slave && !this.Attacked)
				{
					if (!this.Yandere.Noticed && !this.Yandere.Invisible)
					{
						bool flag3 = false;
						if (this.Guarding || this.Fleeing || (this.InvestigatingBloodPool && this.WitnessedBloodPool))
						{
							flag3 = true;
						}
						bool flag4 = false;
						if (this.Yandere.Club == ClubType.Occult && this.Yandere.OccultRobe)
						{
							flag4 = true;
						}
						if ((this.Yandere.Armed && this.Yandere.EquippedWeapon.Suspicious) || (this.Yandere.Armed && this.Yandere.EquippedWeapon.Bloody) || (!this.IgnoringPettyActions && this.StudentID > 1 && this.Yandere.PickUp != null && this.Yandere.PickUp.Suspicious) || (!this.IgnoringPettyActions && this.StudentID > 1 && this.Yandere.PickUp != null && this.Yandere.PickUp.CleaningProduct && this.Clock.Period != 5 && this.Yandere.CleaningNotSuspicious == 0f) || (this.Guarding && this.Yandere.Mopping && this.Yandere.Mop.Bloodiness > 0f) || (this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && !this.Yandere.Paint) || (this.Yandere.Sanity < 33.333f || this.Yandere.Pickpocketing || this.Yandere.Lockpicking || this.Yandere.Attacking || this.Yandere.Cauterizing || this.Yandere.Struggling || this.Yandere.WrappingCorpse || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed)) || (this.Yandere.Dragging && this.Yandere.CurrentRagdoll.Concealed && this.Clock.Period != 5) || (!this.IgnoringPettyActions && this.Yandere.Lewd) || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Carrying && this.Yandere.CurrentRagdoll.Concealed && this.Clock.Period != 5) || (this.Yandere.Yakuza || this.Yandere.Medusa || this.Yandere.Poisoning || this.Yandere.WeaponTimer > 0f || (this.Yandere.WearingRaincoat && !flag4)) || (this.Yandere.MurderousActionTimer > 0f || (!this.IgnoringPettyActions && this.Yandere.Schoolwear == 2 && this.Yandere.transform.position.z < 30f)) || (!this.IgnoringPettyActions && this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart != null && !this.Yandere.PickUp.Garbage) || (!this.IgnoringPettyActions && this.Yandere.SuspiciousActionTimer > 0f) || (!this.IgnoringPettyActions && this.Yandere.Laughing && this.Yandere.LaughIntensity > 15f) || (!this.IgnoringPettyActions && this.Yandere.Stance.Current == StanceType.Crouching) || (!this.IgnoringPettyActions && this.Yandere.Stance.Current == StanceType.Crawling) || (!this.IgnoringPettyActions && this.Yandere.Trespassing) || (this.Private && this.Yandere.Eavesdropping && !this.Yandere.Talking) || (this.Teacher && !this.WitnessedCorpse && this.Yandere.Trespassing) || (this.Teacher && !this.IgnoringPettyActions && this.Yandere.Rummaging) || (!this.IgnoringPettyActions && this.Yandere.TheftTimer > 0f) || (!this.IgnoringPettyActions && this.StudentID == 1 && this.Yandere.NearSenpai && !this.Yandere.Talking) || (!this.IgnoringPettyActions && !this.StudentManager.CombatMinigame.Practice && this.Yandere.DelinquentFighting && this.StudentID != 10 && this.StudentManager.CombatMinigame.Path < 4 && !this.StudentManager.CombatMinigame.Practice && !this.Yandere.SeenByAuthority) || (flag3 && this.Yandere.PickUp != null && this.Yandere.PickUp.Mop != null && this.Yandere.PickUp.Mop.Bloodiness > 50f) || (!this.IgnoringPettyActions && flag3 && this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart != null && !this.Yandere.PickUp.Garbage) || (this.Yandere.PickUp != null && this.Yandere.PickUp.Clothing && this.Yandere.PickUp.Evidence && !this.Yandere.PickUp.BloodMistakenForPaint) || (!this.IgnoringPettyActions && this.AnnoyedByRadio > 1 && this.Yandere.PotentiallyAnnoyingTimer > 0f) || (!this.IgnoringPettyActions && this.AnnoyedByGiggles > 4 && this.Yandere.AnnoyingGiggleTimer > 0f) || (!this.IgnoringPettyActions && this.Yandere.PreparingThrow && this.Yandere.Obvious))
						{
							bool flag5 = false;
							if (this.Yandere.transform.position.y < base.transform.position.y + 4f)
							{
								flag5 = true;
							}
							Vector3 headPosition3 = this.Yandere.HeadPosition;
							if ((flag5 && this.CanSeeObject(this.Yandere.gameObject, headPosition3)) || this.AwareOfMurder)
							{
								this.YandereVisible = true;
								if (this.Yandere.Attacking || this.Yandere.Cauterizing || this.Yandere.Struggling || this.Yandere.WrappingCorpse || (this.WitnessedCorpse && this.Yandere.NearBodies > 0 && this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && !this.Yandere.Paint) || (this.WitnessedCorpse && this.Yandere.NearBodies > 0 && this.Yandere.Armed) || (this.WitnessedCorpse && this.Yandere.NearBodies > 0 && this.Yandere.Sanity < 66.66666f) || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.MurderousActionTimer > 0f || (this.Guarding && this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && !this.Yandere.Paint)) || (this.Guarding && this.Yandere.Armed && this.Yandere.EquippedWeapon.Dangerous) || (this.Guarding && this.Yandere.Armed && this.Yandere.EquippedWeapon.Suspicious) || (this.Guarding && this.Yandere.Sanity < 66.66666f) || (this.Guarding && this.Yandere.WearingRaincoat && !flag4) || (!this.IgnoringPettyActions && !this.StudentManager.CombatMinigame.Practice && this.Club == ClubType.Council && this.Yandere.DelinquentFighting && this.StudentManager.CombatMinigame.Path < 4 && !this.Yandere.SeenByAuthority) || (!this.StudentManager.CombatMinigame.Practice && this.Teacher && this.Yandere.DelinquentFighting && this.StudentManager.CombatMinigame.Path < 4 && !this.Yandere.SeenByAuthority) || (flag3 && this.Yandere.PickUp != null && this.Yandere.PickUp.Mop != null && this.Yandere.PickUp.Mop.Bloodiness > 0f) || (flag3 && this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart != null && !this.Yandere.PickUp.Garbage) || (this.Yandere.PickUp != null && this.Teacher && this.Yandere.PickUp.Clothing && this.Yandere.PickUp.Evidence) || (this.StudentManager.Atmosphere < 0.33333f && this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && this.Yandere.Armed) || (this.Fleeing && this.Yandere.Yakuza))
								{
									Debug.Log(this.Name + " is aware that Yandere-chan is doing something murderous.");
									if (this.Yandere.Hungry || !this.Yandere.Egg)
									{
										Debug.Log(this.Name + " has just witnessed a murder!");
										if (this.Yandere.PickUp != null)
										{
											if (flag3)
											{
												if (this.Yandere.PickUp.Mop != null)
												{
													if (this.Yandere.PickUp.Mop.Bloodiness > 0f)
													{
														Debug.Log("This character witnessed Yandere-chan trying to cover up a crime.");
														this.WitnessedCoverUp = true;
													}
												}
												else if (this.Yandere.PickUp.BodyPart != null && !this.Yandere.PickUp.Garbage)
												{
													Debug.Log("This character witnessed Yandere-chan trying to cover up a crime.");
													this.WitnessedCoverUp = true;
												}
											}
											if (this.Teacher && this.Yandere.PickUp.Clothing && this.Yandere.PickUp.Evidence)
											{
												Debug.Log("This character witnessed Yandere-chan trying to cover up a crime.");
												this.WitnessedCoverUp = true;
											}
										}
										if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
										{
											Debug.Log(this.Name + ", a Phone Addict, is deciding what to do.");
											this.Countdown.gameObject.SetActive(false);
											this.Countdown.Sprite.fillAmount = 1f;
											this.WitnessedMurder = false;
											this.Fleeing = false;
											if (this.CrimeReported)
											{
												Debug.Log(this.Name + "'s ''CrimeReported'' was ''true'', but we're seeing it to ''false''.");
												this.CrimeReported = false;
											}
										}
										if (!this.Yandere.DelinquentFighting)
										{
											this.NoBreakUp = true;
										}
										else if (this.Teacher || this.Club == ClubType.Council)
										{
											this.Yandere.SeenByAuthority = true;
										}
										this.WitnessMurder();
									}
								}
								else if (!this.Fleeing && (!this.Alarmed || this.CanStillNotice))
								{
									if (this.Yandere.Rummaging || this.Yandere.TheftTimer > 0f)
									{
										this.Alarm = 200f;
									}
									if (this.Yandere.WeaponTimer > 0f)
									{
										this.Alarm = 200f;
									}
									if (this.IgnoreTimer == 0f || this.CanStillNotice)
									{
										if (this.Teacher)
										{
											this.StudentManager.TutorialWindow.ShowTeacherMessage = true;
										}
										int num = 1;
										if (this.Yandere.Armed && this.Yandere.EquippedWeapon.Suspicious && (this.Yandere.EquippedWeapon.Type == WeaponType.Bat || this.Yandere.EquippedWeapon.Type == WeaponType.Katana || this.Yandere.EquippedWeapon.Type == WeaponType.Saw || this.Yandere.EquippedWeapon.Type == WeaponType.Weight))
										{
											num = 5;
										}
										if (this.Yandere.Carrying)
										{
											num = 5;
										}
										if (!this.Yandere.Chased && this.Yandere.Chasers == 0 && !this.Yandere.StruggleIminent)
										{
											if (this.InstantNoticeTimer > 0f)
											{
												this.Alarm = 100f;
											}
											this.Alarm += Time.deltaTime * (100f / this.DistanceToPlayer) * this.Paranoia * this.Perception * (float)num;
											if (this.Yandere.SneakingShot)
											{
												this.Alarm += Time.deltaTime * (100f / this.DistanceToPlayer) * this.Paranoia * this.Perception * (float)num * 2f;
											}
											if (this.Yandere.SuspiciousActionTimer > 0f || this.Yandere.PotentiallyAnnoyingTimer > 0f || this.Yandere.AnnoyingGiggleTimer > 0f)
											{
												Debug.Log(this.Name + " witnessed something suspicious or annoying.");
												this.Alarm += Time.deltaTime * (100f / this.DistanceToPlayer) * this.Paranoia * this.Perception * (float)num * 9f;
												if (this.Yandere.CreatingBucketTrap)
												{
													Debug.Log(this.Name + " just witnessed the player creating a bucket trap.");
													this.WillRemoveBucket = true;
												}
												if (this.Yandere.CreatingTripwireTrap)
												{
													this.WillRemoveTripwire = true;
												}
											}
										}
										else
										{
											this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
										}
										if (this.StudentID == 1 && this.Yandere.TimeSkipping)
										{
											this.Clock.EndTimeSkip();
										}
									}
								}
							}
							else
							{
								this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
							}
						}
						else
						{
							this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
						}
					}
					else
					{
						this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
					}
				}
				else
				{
					this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
				}
			}
			else
			{
				this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
			}
			if (this.PreviousAlarm > this.Alarm && this.Alarm < 100f)
			{
				this.YandereVisible = false;
			}
			if (this.Teacher && !this.Yandere.Medusa && this.Yandere.Egg)
			{
				this.Alarm = 0f;
			}
			if (this.Alarm > 100f)
			{
				if (this.Yandere.Yakuza && this.YandereVisible)
				{
					this.SpottedYakuza = true;
					this.WitnessMurder();
					return;
				}
				this.BecomeAlarmed();
				return;
			}
		}
		else if (this.Distraction != null)
		{
			this.targetRotation = Quaternion.LookRotation(new Vector3(this.Distraction.position.x, base.transform.position.y, this.Distraction.position.z) - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
			if (this.Distractor != null)
			{
				if (this.Distractor.Club == ClubType.Cooking && this.Distractor.ClubActivityPhase > 0 && this.Distractor.Actions[this.Distractor.Phase] == StudentActionType.ClubAction)
				{
					this.CharacterAnimation.CrossFade(this.PlateEatAnim);
					if ((double)this.CharacterAnimation[this.PlateEatAnim].time > 6.83333)
					{
						this.Fingerfood[this.Distractor.StudentID].SetActive(false);
						return;
					}
					if ((double)this.CharacterAnimation[this.PlateEatAnim].time > 2.66666)
					{
						this.Fingerfood[this.Distractor.StudentID].SetActive(true);
						return;
					}
				}
				else
				{
					this.CharacterAnimation.CrossFade(this.RandomAnim);
					if (this.CharacterAnimation[this.RandomAnim].time >= this.CharacterAnimation[this.RandomAnim].length)
					{
						this.PickRandomAnim();
					}
				}
			}
		}
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x001E9F00 File Offset: 0x001E8100
	public void BecomeAlarmed()
	{
		Debug.Log(this.Name + " just fired the BecomeAlarmed() function.");
		if (this.Teacher && this.Persona == PersonaType.Violent)
		{
			this.Persona = PersonaType.Strict;
		}
		if (this.Yandere.Medusa && this.YandereVisible)
		{
			this.TurnToStone();
			return;
		}
		if (this.Investigating && !this.HeardScream)
		{
			Debug.Log(this.Name + " was investigating prior to being alarmed, so they are now ending their investigation.");
			this.StopInvestigating();
		}
		if (!this.Alarmed || this.DiscCheck)
		{
			bool flag = false;
			if (this.CurrentAction == StudentActionType.Sunbathe && this.SunbathePhase > 2)
			{
				this.SunbathePhase = 2;
				flag = true;
			}
			if (this.ReturningMisplacedWeapon)
			{
				this.DropMisplacedWeapon();
				this.ReturnToRoutineAfter = true;
			}
			if (this.DistractionTarget != null)
			{
				this.DistractionTarget.TargetedForDistraction = false;
			}
			if (this.SolvingPuzzle)
			{
				this.DropPuzzle();
			}
			Debug.Log(this.Name + " is now having their DiscCheck set to false...");
			this.CharacterAnimation.CrossFade(this.LeanAnim);
			this.Pathfinding.canSearch = false;
			this.Pathfinding.canMove = false;
			this.CameraReacting = false;
			this.CanStillNotice = false;
			this.Distracting = false;
			this.Distracted = false;
			this.DiscCheck = false;
			this.Reacted = false;
			this.Routine = false;
			this.Alarmed = true;
			if (this.HeardScream)
			{
				Debug.Log("But HeardScream was true, so we're setting DiscCheck to true...");
				this.DiscCheck = true;
			}
			this.PuzzleTimer = 0f;
			this.ReadPhase = 0;
			this.BountyCollider.SetActive(false);
			if (this.YandereVisible && this.Yandere.Mask == null)
			{
				this.Witness = true;
			}
			this.EmptyHands();
			if ((this.Persona == PersonaType.PhoneAddict && !this.Phoneless && !flag) || this.Persona == PersonaType.Sleuth || this.StudentID == 20)
			{
				this.SmartPhone.SetActive(true);
				this.Scrubber.SetActive(false);
				this.Eraser.SetActive(false);
			}
			if (this.Club == ClubType.Cooking && this.Actions[this.Phase] == StudentActionType.ClubAction && this.ClubActivityPhase == 1 && !this.WitnessedCorpse)
			{
				Debug.Log("The game believes that " + this.Name + " did not witness a corpse; ''ResumeDistracting'' is being set to ''true''.");
				this.ResumeDistracting = true;
				this.MyPlate.gameObject.SetActive(true);
			}
			if (this.Actions[this.Phase] == StudentActionType.BakeSale && this.BakeSalePhase == 1)
			{
				this.MyPlate.gameObject.SetActive(true);
			}
			if (this.TakingOutTrash && !this.WitnessedCorpse)
			{
				this.ResumeTakingOutTrash = true;
			}
			this.SpeechLines.Stop();
			this.StopPairing();
			if (this.Witnessed != StudentWitnessType.Weapon && this.Witnessed != StudentWitnessType.Eavesdropping)
			{
				this.PreviouslyWitnessed = this.Witnessed;
			}
			if (this.DistanceToDestination < 5f && (this.Actions[this.Phase] == StudentActionType.Graffiti || this.Actions[this.Phase] == StudentActionType.Bully))
			{
				this.StudentManager.NoBully[this.BullyID] = true;
				this.KilledMood = true;
			}
			if (this.WitnessedCorpse && !this.WitnessedMurder)
			{
				this.Witnessed = StudentWitnessType.Corpse;
				this.EyeShrink = 0.9f;
			}
			else if (this.WitnessedLimb)
			{
				this.Witnessed = StudentWitnessType.SeveredLimb;
			}
			else if (this.WitnessedBloodyWeapon)
			{
				this.Witnessed = StudentWitnessType.BloodyWeapon;
			}
			else if (this.WitnessedBloodPool)
			{
				this.Witnessed = StudentWitnessType.BloodPool;
			}
			else if (this.WitnessedWeapon)
			{
				this.Witnessed = StudentWitnessType.DroppedWeapon;
			}
			else if (this.StudentManager.TutorialActive)
			{
				this.Witnessed = StudentWitnessType.Tutorial;
			}
			if (this.SawCorpseThisFrame)
			{
				this.YandereVisible = false;
			}
			if (this.TemporarilyBlind)
			{
				this.YandereVisible = false;
			}
			if ((this.SenpaiWitnessingRivalDie && this.StudentManager.Students[this.StudentManager.RivalID].Ragdoll.Concealed) || (this.SpecialRivalDeathReaction && this.StudentToMourn.Ragdoll.Concealed))
			{
				this.NotAlarmedByYandereChan = false;
				this.YandereVisible = true;
			}
			if (this.YandereVisible && !this.NotAlarmedByYandereChan)
			{
				if (this.Yandere.Mask == null)
				{
					this.TimesAlarmed++;
				}
				if ((!this.Injured && this.Persona == PersonaType.Violent && this.Yandere.Armed && !this.WitnessedCorpse && !this.RespectEarned) || (this.Persona == PersonaType.Violent && this.Yandere.DelinquentFighting))
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentWeaponReaction, 0, 3f);
					this.ThreatDistance = this.DistanceToPlayer;
					this.CheerTimer = UnityEngine.Random.Range(1f, 2f);
					this.SmartPhone.SetActive(false);
					this.Threatened = true;
					this.ThreatPhase = 1;
					this.ForgetGiggle();
				}
				Debug.Log(this.Name + " saw Yandere-chan doing something bad.");
				if (this.Yandere.CreatingBucketTrap)
				{
					Debug.Log(this.Name + " just witnessed the player creating a bucket trap.");
					this.WillRemoveBucket = true;
				}
				if (this.Yandere.CreatingTripwireTrap)
				{
					Debug.Log(this.Name + " just witnessed the player creating a tripwire trap.");
					this.WillRemoveTripwire = true;
				}
				this.FocusOnYandere = true;
				if (this.StudentID > 1 && this.Yandere.Mask == null)
				{
					Debug.Log("Alerts was incremented.");
					this.Yandere.Alerts++;
				}
				else
				{
					Debug.Log("Alerts was not incremented.");
				}
				if (this.Yandere.Attacking || this.Yandere.Struggling || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart && !this.Yandere.PickUp.Garbage))
				{
					if (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed)
					{
						this.Corpse = this.Yandere.CurrentRagdoll;
					}
					if (!this.Yandere.Egg)
					{
						this.WitnessMurder();
					}
				}
				else if (this.Witnessed != StudentWitnessType.Corpse)
				{
					this.DetermineWhatWasWitnessed();
				}
				if (this.Teacher && this.WitnessedCorpse)
				{
					this.Concern = 1;
				}
				if (this.StudentID == 1 && this.Yandere.Mask == null && !this.Yandere.Egg && this.Persona != PersonaType.Evil)
				{
					if (this.Concern == 5)
					{
						Debug.Log("Senpai noticed stalking or lewdness.");
						this.SenpaiNoticed();
						if (this.Witnessed == StudentWitnessType.Stalking || this.Witnessed == StudentWitnessType.Lewd)
						{
							this.CharacterAnimation.CrossFade(this.IdleAnim);
							this.CharacterAnimation[this.AngryFaceAnim].weight = 1f;
						}
						else
						{
							Debug.Log("Senpai entered his scared animation.");
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
							if (this.Male)
							{
								this.CharacterAnimation["scaredFace_00"].weight = 1f;
							}
						}
						this.CameraEffects.MurderWitnessed();
					}
					else
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("suspicious_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.LeanAnim);
						}
						this.CameraEffects.Alarm();
					}
				}
				else if (!this.Teacher)
				{
					this.CameraEffects.Alarm();
				}
				else
				{
					Debug.Log(this.Name + ", using the Teacher Persona, has just witnessed Yandere-chan doing something bad.");
					if (!this.Fleeing)
					{
						if (this.AnnoyedByGiggles > 4 && this.Yandere.AnnoyingGiggleTimer > 0f)
						{
							Debug.Log("It seems to be a giggle, specifically, that annoyed the teacher.");
							this.Concern = 5;
						}
						if (this.Concern < 5)
						{
							this.CameraEffects.Alarm();
						}
						else if (!this.Yandere.Struggling && !this.Yandere.StruggleIminent && !this.StudentManager.PinningDown)
						{
							this.SenpaiNoticed();
							this.CameraEffects.MurderWitnessed();
						}
					}
					else
					{
						this.PersonaReaction();
						this.AlarmTimer = 0f;
						if (this.Concern < 5)
						{
							this.CameraEffects.Alarm();
						}
						else
						{
							this.CameraEffects.MurderWitnessed();
						}
					}
				}
				if (!this.Teacher)
				{
					if (this.Club != ClubType.Delinquent && this.Witnessed == this.PreviouslyWitnessed)
					{
						this.RepeatReaction = true;
					}
					if (this.Yandere.Mask == null)
					{
						this.RepDeduction = 0f;
						this.CalculateReputationPenalty();
						if (this.RepDeduction >= 0f)
						{
							this.RepLoss -= this.RepDeduction;
						}
						this.Reputation.PendingRep -= this.RepLoss * this.Paranoia;
						this.PendingRep -= this.RepLoss * this.Paranoia;
					}
				}
				if (this.ToiletEvent != null && this.ToiletEvent.EventDay == DayOfWeek.Monday)
				{
					this.ToiletEvent.EndEvent();
				}
			}
			else if (!this.WitnessedCorpse)
			{
				if (this.Yandere.Caught)
				{
					if (this.Yandere.Mask == null)
					{
						if (this.Yandere.Pickpocketing)
						{
							this.Witnessed = StudentWitnessType.Pickpocketing;
							this.RepLoss += 10f;
						}
						else
						{
							this.Witnessed = StudentWitnessType.Theft;
						}
						this.RepDeduction = 0f;
						Debug.Log("Calculating reputation penalty from here. 1");
						this.CalculateReputationPenalty();
						if (this.RepDeduction >= 0f)
						{
							this.RepLoss -= this.RepDeduction;
						}
						this.Reputation.PendingRep -= this.RepLoss * this.Paranoia;
						this.PendingRep -= this.RepLoss * this.Paranoia;
					}
				}
				else if (this.WitnessedLimb)
				{
					this.Witnessed = StudentWitnessType.SeveredLimb;
				}
				else if (this.WitnessedBloodyWeapon)
				{
					this.Witnessed = StudentWitnessType.BloodyWeapon;
				}
				else if (this.WitnessedBloodPool)
				{
					this.Witnessed = StudentWitnessType.BloodPool;
				}
				else if (this.WitnessedWeapon)
				{
					this.Witnessed = StudentWitnessType.DroppedWeapon;
				}
				else
				{
					Debug.Log(this.Name + " was alarmed by something, but didn't see what it was. DiscCheck is being set to true.");
					if (this.WitnessedSlave)
					{
						Debug.Log("Specifically, " + this.Name + " saw a mind-broken slave walking around...");
					}
					this.Witnessed = StudentWitnessType.None;
					this.DiscCheck = true;
					this.Witness = false;
					this.AlarmTimer = 0f;
				}
			}
			else
			{
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
			}
		}
		this.NotAlarmedByYandereChan = false;
		this.SawCorpseThisFrame = false;
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x001EAA18 File Offset: 0x001E8C18
	private void UpdateDetectionMarker()
	{
		if (this.Alarm < 0f)
		{
			this.Alarm = 0f;
			if (this.Club == ClubType.Council && !this.Yandere.Noticed)
			{
				this.CanStillNotice = true;
			}
		}
		if (this.DetectionMarker != null)
		{
			if (this.Alarm > 0f)
			{
				if (!this.DetectionMarker.Tex.enabled)
				{
					this.DetectionMarker.Tex.enabled = true;
				}
				this.DetectionMarker.Tex.transform.localScale = new Vector3(this.DetectionMarker.Tex.transform.localScale.x, (this.Alarm <= 100f) ? (this.Alarm / 100f) : 1f, this.DetectionMarker.Tex.transform.localScale.z);
				this.DetectionMarker.Tex.color = new Color(this.DetectionMarker.Tex.color.r, this.DetectionMarker.Tex.color.g, this.DetectionMarker.Tex.color.b, this.Alarm / 100f);
				return;
			}
			if (this.DetectionMarker.Tex.color.a != 0f)
			{
				this.DetectionMarker.Tex.enabled = false;
				this.DetectionMarker.Tex.color = new Color(this.DetectionMarker.Tex.color.r, this.DetectionMarker.Tex.color.g, this.DetectionMarker.Tex.color.b, 0f);
				return;
			}
		}
		else
		{
			this.SpawnDetectionMarker();
		}
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x001EABFC File Offset: 0x001E8DFC
	private void UpdateTalkInput()
	{
		if (this.Prompt.Circle[0].fillAmount == 0f)
		{
			Debug.Log("The player is attempting to speak to a student.");
			if (!this.Alarmed)
			{
				this.AlarmTimer = 0f;
			}
			bool flag = false;
			if (!this.StudentManager.EmptyDemon)
			{
				if (!this.StudentManager.Eighties && this.StudentID == 10 && this.StudentManager.TaskManager.TaskStatus[46] == 1 && !this.NoMentor && !this.StudentManager.TaskManager.Mentored)
				{
					Debug.Log("Nothing should be stopping her, but what time is it?");
					if (this.Clock.Period == 3 || this.Clock.Period == 5)
					{
						Debug.Log("It's you-can't-mentor-the-martial-arts-club-o-clock.");
						this.Yandere.NotificationManager.CustomText = "Martial Arts Club is not training now";
						this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				bool flag2 = false;
				if (this.StudentManager.Pose)
				{
					flag2 = true;
				}
				if (this.FocusOnYandere)
				{
					Debug.Log("''FocusOnYandere'' was true at the time.");
					if (!this.StudentManager.KokonaTutorial)
					{
						this.EndAlarm();
					}
				}
				if (this.Alarm > 0f || this.AlarmTimer > 0f || this.Yandere.Armed || this.Yandere.Shoved || this.Stripping || this.Waiting || this.InEvent || this.SentHome || this.Threatened || this.Guarding || this.VisitSenpaiDesk || (this.Distracted && !this.Drownable) || (this.StudentID == 1 && !flag2) || this.Yandere.YandereVision || this.TakingOutTrash)
				{
					if (this.Alarm > 0f)
					{
						Debug.Log("Alarm was above zero.");
					}
					if (this.AlarmTimer > 0f)
					{
						Debug.Log("AlarmTimer was above zero.");
					}
					if (this.Stripping)
					{
						Debug.Log("Stripping was true.");
					}
					if ((!this.Slave && !this.BadTime && !this.Yandere.Gazing && !this.FightingSlave) || this.Yandere.YandereVision || this.Stripping)
					{
						Debug.Log("Met criteria for not being allowed to talk to a student.");
						if (this.InEvent || this.VisitSenpaiDesk)
						{
							string str = "She";
							if (this.Male)
							{
								str = "He";
							}
							this.Yandere.NotificationManager.CustomText = str + " is busy with an event right now!";
							this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
						}
						else if (this.Guarding)
						{
							string str2 = "She";
							if (this.Male)
							{
								str2 = "He";
							}
							if (!this.Teacher)
							{
								this.Yandere.NotificationManager.CustomText = str2 + " is too scared to talk right now!";
								this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
							}
							else
							{
								this.Yandere.NotificationManager.CustomText = str2 + " is already aware of a murder!";
								this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
							}
						}
						this.Prompt.Circle[0].fillAmount = 1f;
					}
				}
			}
			if (this.Yandere.Yakuza)
			{
				this.Prompt.Circle[0].fillAmount = 1f;
				this.FocusOnYandere = true;
				this.Alarm += 200f;
			}
			if (this.Prompt.Circle[0].fillAmount == 0f)
			{
				Debug.Log("The player is allowed to speak to a student right now.");
				bool flag3 = false;
				if (this.StudentID < 86 && this.Armband.activeInHierarchy)
				{
					Debug.Log("The player is speaking to a Club Leader.");
					Debug.Log("The Club Leader's current action is: " + this.Actions[this.Phase].ToString());
					if (this.StudentManager.CustomMode)
					{
						flag3 = true;
					}
					if (this.Actions[this.Phase] == StudentActionType.ClubAction || this.Actions[this.Phase] == StudentActionType.SitAndSocialize || this.Actions[this.Phase] == StudentActionType.Socializing || this.Actions[this.Phase] == StudentActionType.Sleuth || this.Actions[this.Phase] == StudentActionType.Lyrics || this.Actions[this.Phase] == StudentActionType.Patrol || this.Actions[this.Phase] == StudentActionType.Rehearse || this.Actions[this.Phase] == StudentActionType.SitAndEatBento || this.Actions[this.Phase] == StudentActionType.BakeSale || this.Actions[this.Phase] == StudentActionType.Clean || this.Actions[this.Phase] == StudentActionType.Paint)
					{
						Debug.Log("This Club Leader is " + Vector3.Distance(base.transform.position, this.StudentManager.ClubZones[(int)this.Club].position).ToString() + " meters from the center of his Club Zone.");
						Debug.Log(string.Concat(new string[]
						{
							"This Club Leader's DistanceToDestination is ",
							this.DistanceToDestination.ToString(),
							" but he is actually ",
							Vector3.Distance(base.transform.position, this.CurrentDestination.position).ToString(),
							" meters from his destination."
						}));
						if (this.DistanceToDestination < 10f || (base.transform.position.y > this.StudentManager.ClubZones[(int)this.Club].position.y - 2.5f && base.transform.position.y < this.StudentManager.ClubZones[(int)this.Club].position.y + 2.5f && Vector3.Distance(base.transform.position, this.StudentManager.ClubZones[(int)this.Club].position) < this.ClubThreshold) || (this.Club == ClubType.Drama && Vector3.Distance(base.transform.position, this.StudentManager.DramaSpots[1].position) < 12f) || (this.Club == ClubType.MartialArts && base.transform.position.y < 1f && Vector3.Distance(base.transform.position, this.StudentManager.Clubs.List[this.StudentID].position) < 12f))
						{
							Debug.Log("Criteria for talking to this Club Leader was met.");
							flag3 = true;
							this.Warned = false;
						}
					}
				}
				if (this.StudentID == 76)
				{
					Debug.Log(string.Concat(new string[]
					{
						"BlondeHair is: ",
						GameGlobals.BlondeHair.ToString(),
						". Yandere's Persona is: ",
						this.Yandere.Persona.ToString(),
						". Friendships are: ",
						PlayerGlobals.GetStudentFriend(76).ToString(),
						", ",
						PlayerGlobals.GetStudentFriend(77).ToString(),
						", ",
						PlayerGlobals.GetStudentFriend(78).ToString(),
						", ",
						PlayerGlobals.GetStudentFriend(79).ToString(),
						", ",
						PlayerGlobals.GetStudentFriend(80).ToString()
					}));
					bool flag4 = false;
					if (this.Yandere.Persona == YanderePersonaType.Tough || this.Yandere.Persona == YanderePersonaType.Edgy)
					{
						flag4 = true;
					}
					bool flag5 = this.StudentManager.Students[76] != null && this.StudentManager.Students[76].Friend;
					bool flag6 = this.StudentManager.Students[77] != null && this.StudentManager.Students[77].Friend;
					bool flag7 = this.StudentManager.Students[78] != null && this.StudentManager.Students[78].Friend;
					bool flag8 = this.StudentManager.Students[79] != null && this.StudentManager.Students[79].Friend;
					bool flag9 = this.StudentManager.Students[80] != null && this.StudentManager.Students[80].Friend;
					Debug.Log("Yandere.PersonaID is: " + this.Yandere.PersonaID.ToString());
					Debug.Log("Yandere.Persona is: " + this.Yandere.Persona.ToString());
					if ((GameGlobals.BlondeHair && this.Reputation.Reputation < -33.33333f && flag4 && flag5 && flag6 && flag7 && flag8 && flag9) || this.Yandere.Club == ClubType.Delinquent)
					{
						Debug.Log("Yandere-chan meets the criteria to talk to the delinquent leader about joining.");
						flag3 = true;
						this.Warned = false;
					}
					else
					{
						Debug.Log("Yandere-chan does not meet the criteria to talk to the delinquent leader about joining.");
					}
				}
				bool flag10 = false;
				if (this.Yandere.PickUp != null && this.Yandere.PickUp.Salty && !this.Indoors)
				{
					flag10 = true;
				}
				bool flag11 = false;
				if (this.Teacher && this.StudentManager.CanSelfReport)
				{
					flag11 = true;
				}
				if (ClubGlobals.GetClubKicked(this.Club) && this.ExplainedKick)
				{
					flag3 = false;
				}
				if (this.StudentManager.Pose)
				{
					this.MyController.enabled = false;
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.Stop = true;
					this.Pose();
				}
				else if (this.BadTime)
				{
					this.Yandere.EmptyHands();
					this.BecomeRagdoll();
					this.Yandere.RagdollPK.connectedBody = this.Ragdoll.AllRigidbodies[5];
					this.Yandere.RagdollPK.connectedAnchor = this.Ragdoll.LimbAnchor[4];
					this.DialogueWheel.PromptBar.ClearButtons();
					this.DialogueWheel.PromptBar.Label[1].text = "Back";
					this.DialogueWheel.PromptBar.UpdateButtons();
					this.DialogueWheel.PromptBar.Show = true;
					this.Yandere.Ragdoll = this.Ragdoll.gameObject;
					this.Yandere.SansEyes[0].SetActive(true);
					this.Yandere.SansEyes[1].SetActive(true);
					this.Yandere.GlowEffect.Play();
					this.Yandere.CanMove = false;
					this.Yandere.PK = true;
					this.DeathType = DeathType.EasterEgg;
				}
				else if (this.StudentManager.Six)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.AlarmDisc, base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<AlarmDiscScript>().Originator = this;
					AudioSource.PlayClipAtPoint(this.Yandere.SixTakedown, base.transform.position);
					AudioSource.PlayClipAtPoint(this.Yandere.Snarls[UnityEngine.Random.Range(0, this.Yandere.Snarls.Length)], base.transform.position);
					this.Yandere.CharacterAnimation.CrossFade("f02_sixEat_00");
					this.Yandere.TargetStudent = this;
					this.Yandere.FollowHips = true;
					this.Yandere.Attacking = true;
					this.Yandere.CanMove = false;
					this.Yandere.Eating = true;
					this.CharacterAnimation.CrossFade(this.EatVictimAnim);
					this.CharacterAnimation[this.WetAnim].weight = 0f;
					this.Pathfinding.enabled = false;
					this.Routine = false;
					this.Dying = true;
					this.Eaten = true;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.EmptyGameObject, base.transform.position, Quaternion.identity);
					this.Yandere.SixTarget = gameObject.transform;
					this.Yandere.SixTarget.LookAt(this.Yandere.transform.position);
					this.Yandere.SixTarget.Translate(this.Yandere.SixTarget.forward);
				}
				else if (this.Yandere.SpiderGrow)
				{
					if (!this.Eaten && !this.Cosmetic.Empty)
					{
						AudioSource.PlayClipAtPoint(this.Yandere.SixTakedown, base.transform.position);
						AudioSource.PlayClipAtPoint(this.Yandere.Snarls[UnityEngine.Random.Range(0, this.Yandere.Snarls.Length)], base.transform.position);
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.Yandere.EmptyHusk, base.transform.position + base.transform.forward * 0.5f, Quaternion.identity);
						gameObject2.GetComponent<EmptyHuskScript>().TargetStudent = this;
						gameObject2.transform.LookAt(base.transform.position);
						this.CharacterAnimation.CrossFade(this.EatVictimAnim);
						this.CharacterAnimation[this.WetAnim].weight = 0f;
						this.Pathfinding.enabled = false;
						this.Distracted = false;
						this.Routine = false;
						this.Blind = true;
						this.Dying = true;
						this.Eaten = true;
						if (this.Investigating)
						{
							this.StopInvestigating();
						}
						if (this.Following)
						{
							this.FollowCountdown.gameObject.SetActive(false);
							this.Yandere.Follower = null;
							this.Yandere.Followers--;
							this.Following = false;
						}
						UnityEngine.Object.Instantiate<GameObject>(this.EmptyGameObject, base.transform.position, Quaternion.identity);
					}
				}
				else if (this.StudentManager.Gaze)
				{
					this.Yandere.CharacterAnimation.CrossFade("f02_gazerPoint_00");
					this.Yandere.GazerEyes.Attacking = true;
					this.Yandere.TargetStudent = this;
					this.Yandere.GazeAttacking = true;
					this.Yandere.CanMove = false;
					this.Routine = false;
				}
				else if (this.Yandere.Succubus)
				{
					this.Prompt.Circle[0].fillAmount = 1f;
					if (this.Male)
					{
						if (this.Following)
						{
							this.StopFollowing();
						}
						else
						{
							this.Hearts.emission.enabled = true;
							this.OriginalIdleAnim = this.AdmireAnims[0];
							this.IdleAnim = this.AdmireAnims[0];
							this.CurrentDestination = this.Yandere.transform;
							this.Pathfinding.target = this.Yandere.transform;
							this.Prompt.Label[0].text = "     Stop";
							this.Yandere.FollowerList[this.Yandere.Followers] = this;
							this.Yandere.Followers++;
							this.Following = true;
							this.Routine = false;
							this.Hurry = false;
							this.Blind = true;
							this.StudentManager.UpdateStudents(0);
						}
					}
					else if (this.Yandere.Followers > 0)
					{
						StudentScript studentScript = null;
						for (int i = 0; i < 101; i++)
						{
							if (studentScript == null && this.Yandere.FollowerList[i] != null)
							{
								studentScript = this.Yandere.FollowerList[i];
								this.Yandere.FollowerList[i] = null;
							}
						}
						if (studentScript != null)
						{
							studentScript.MurderSuicidePhase = 1;
							studentScript.HuntTarget = this;
							studentScript.Following = false;
							studentScript.Blind = true;
							studentScript.GoCommitMurder();
						}
					}
				}
				else if (this.Slave)
				{
					this.Prompt.Circle[0].fillAmount = 1f;
					this.Yandere.TargetStudent = this;
					this.Yandere.PauseScreen.StudentInfoMenu.Targeting = true;
					this.Yandere.PauseScreen.StudentInfoMenu.gameObject.SetActive(true);
					this.Yandere.PauseScreen.StudentInfoMenu.Column = 0;
					this.Yandere.PauseScreen.StudentInfoMenu.Row = 0;
					this.Yandere.PauseScreen.StudentInfoMenu.UpdateHighlight();
					base.StartCoroutine(this.Yandere.PauseScreen.StudentInfoMenu.UpdatePortraits());
					this.Yandere.PauseScreen.MainMenu.SetActive(false);
					this.Yandere.PauseScreen.Panel.enabled = true;
					this.Yandere.PauseScreen.Sideways = true;
					this.Yandere.PauseScreen.Show = true;
					Time.timeScale = 0.0001f;
					this.Yandere.PromptBar.ClearButtons();
					this.Yandere.PromptBar.Label[1].text = "Cancel";
					this.Yandere.PromptBar.UpdateButtons();
					this.Yandere.PromptBar.Show = true;
				}
				else if (this.FightingSlave)
				{
					this.Yandere.CharacterAnimation.CrossFade("f02_subtleStab_00");
					this.Yandere.SubtleStabbing = true;
					this.Yandere.TargetStudent = this;
					this.Yandere.CanMove = false;
				}
				else if (this.Following)
				{
					this.StopFollowing();
				}
				else if ((this.Clock.Period == 2 && !flag11) || (this.Clock.Period == 4 && !flag11))
				{
					Debug.Log("This character won't talk because Class is in session, or because their destination is ''seat''.");
					if (this.Club != ClubType.Delinquent)
					{
						this.Subtitle.UpdateLabel(SubtitleType.ClassApology, 0, 3f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentAnnoy, UnityEngine.Random.Range(0, this.Subtitle.DelinquentAnnoyClips.Length), 3f);
					}
					this.Prompt.Circle[0].fillAmount = 1f;
				}
				else if (this.InEvent || !this.CanTalk || this.GoAway || this.Fleeing || (this.Meeting && !this.Drownable) || this.Wet || this.TurnOffRadio || this.InvestigatingBloodPool || (this.MyPlate != null && this.MyPlate.parent == this.RightHand) || flag10 || this.ReturningMisplacedWeapon || this.Actions[this.Phase] == StudentActionType.Bully || this.Actions[this.Phase] == StudentActionType.Graffiti || (this.CanTakeSnack && this.IgnoreFoodTimer > 0f) || this.MustTrip || (this.FollowTarget != null && this.FollowTarget.InEvent))
				{
					if (this.InEvent)
					{
						Debug.Log("Can't talk because InEvent is true.");
					}
					if (!this.CanTalk)
					{
						Debug.Log("Can't talk because CanTalk is false.");
					}
					if (this.GoAway)
					{
						Debug.Log("Can't talk because GoAway is true.");
					}
					if (this.Fleeing)
					{
						Debug.Log("Can't talk because Fleeing is true.");
					}
					if (this.Meeting)
					{
						Debug.Log("Can't talk because Meeting is true.");
					}
					if (this.Wet)
					{
						Debug.Log("Can't talk because Wet is true.");
					}
					if (this.TurnOffRadio)
					{
						Debug.Log("Can't talk because TurnOffRadio is true.");
					}
					if (this.InvestigatingBloodPool)
					{
						Debug.Log("Can't talk because InvestigatingBloodPool is true.");
					}
					if (this.MyPlate != null)
					{
						Debug.Log("Can't talk because MyPlate is not null.");
					}
					if (flag10)
					{
						Debug.Log("Can't talk because CannotEat is true.");
					}
					if (this.ReturningMisplacedWeapon)
					{
						Debug.Log("Can't talk because ReturningMisplacedWeapon is true.");
					}
					if (this.NoMentor)
					{
						Debug.Log("Can't talk because NoMentor is true.");
					}
					if (this.Actions[this.Phase] == StudentActionType.Bully)
					{
						Debug.Log("Can't talk because Bully is true.");
					}
					if (this.Actions[this.Phase] == StudentActionType.Graffiti)
					{
						Debug.Log("Can't talk because Graffiti is true.");
					}
					if (this.IgnoreFoodTimer > 0f)
					{
						Debug.Log("Can't talk because IgnoreFoodTimer is greater than 0.");
					}
					this.Subtitle.UpdateLabel(SubtitleType.EventApology, 1, 3f);
					this.Prompt.Circle[0].fillAmount = 1f;
					this.StudentManager.UpdateMe(this.StudentID);
				}
				else if (this.Clock.Period == 3 && this.BusyAtLunch)
				{
					this.Subtitle.UpdateLabel(SubtitleType.SadApology, 1, 3f);
					this.Prompt.Circle[0].fillAmount = 1f;
				}
				else if (this.Warned)
				{
					Debug.Log("This character refuses to speak to Yandere-chan because of a grudge.");
					if (this.Persona == PersonaType.Coward)
					{
						this.Subtitle.UpdateLabel(SubtitleType.CowardRefusal, 0, 3f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.GrudgeRefusal, 0, 3f);
					}
					this.Prompt.Circle[0].fillAmount = 1f;
					if (!this.Male)
					{
						this.CharacterAnimation["f02_smile_00"].weight = 0f;
					}
				}
				else if (this.Ignoring)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PhotoAnnoyance, 0, 3f);
					this.Prompt.Circle[0].fillAmount = 1f;
				}
				else if (this.Yandere.PickUp != null && this.Yandere.PickUp.PuzzleCube)
				{
					if (this.Investigating)
					{
						this.StopInvestigating();
					}
					this.EmptyHands();
					this.Prompt.Circle[0].fillAmount = 1f;
					this.PuzzleCube = this.Yandere.PickUp;
					this.Yandere.EmptyHands();
					this.PuzzleCube.enabled = false;
					this.PuzzleCube.Prompt.Hide();
					this.PuzzleCube.Prompt.enabled = false;
					this.PuzzleCube.MyRigidbody.useGravity = false;
					this.PuzzleCube.MyRigidbody.isKinematic = true;
					this.PuzzleCube.gameObject.transform.parent = this.RightHand;
					this.PuzzleCube.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					this.PuzzleCube.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					if (this.Male)
					{
						this.PuzzleCube.gameObject.transform.localPosition = new Vector3(0f, -0.0466666f, 0f);
						this.PuzzleCube.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					}
					else
					{
						this.PuzzleCube.gameObject.transform.localPosition = new Vector3(0f, -0.0266666f, 0f);
						this.PuzzleCube.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
					}
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.SolvingPuzzle = true;
					this.Distracted = true;
					this.Routine = false;
				}
				else if (this.Actions[this.Phase] == StudentActionType.LightFire && this.DistanceToDestination < 1f)
				{
					this.Yandere.NotificationManager.CustomText = "She doesn't seem to notice you...";
					this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
					this.Prompt.Circle[0].fillAmount = 1f;
				}
				else if (this.Teacher && !this.StudentManager.CanSelfReport)
				{
					this.StudentManager.CheckSelfReportStatus(this);
				}
				else
				{
					bool flag12 = false;
					if (this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && !this.Yandere.Paint)
					{
						flag12 = true;
					}
					if (this.Yandere.Club == ClubType.Art && this.Yandere.ClubAttire)
					{
						flag12 = false;
					}
					if (!this.Witness && flag12)
					{
						this.Prompt.Circle[0].fillAmount = 1f;
						this.YandereVisible = true;
						this.Alarm = 200f;
					}
					else
					{
						if (this.Subtitle.CurrentClip != null)
						{
							UnityEngine.Object.Destroy(this.Subtitle.CurrentClip);
						}
						Debug.Log("Setting Speaker to " + this.Name);
						this.Subtitle.Speaker = this;
						this.SpeechLines.Stop();
						this.Yandere.TargetStudent = this;
						if (!this.Grudge)
						{
							if (!this.Yandere.StudentManager.TutorialActive)
							{
								this.ClubManager.CheckGrudge(this.Club);
							}
							if (this.StudentID > 89 && this.StudentManager.CanSelfReport)
							{
								Debug.Log("The player has just reported blood/murder to the faculty.");
								this.StudentManager.Reputation.UpdateRep();
								this.Police.SelfReported = true;
								this.StudentManager.Reputation.Portal.EndDay();
							}
							else if (ClubGlobals.GetClubKicked(this.Club) && flag3 && !this.ExplainedKick)
							{
								Debug.Log("Player was kicked out of this club.");
								if (this.ClubManager.ClubGrudge)
								{
									Debug.Log("Someone in the club hates the player.");
								}
								else
								{
									Debug.Log("Player never showed up for club activities, got kicked.");
								}
								this.Interaction = StudentInteractionType.ClubGrudge;
								this.ExplainedKick = true;
								this.TalkTimer = 5f;
								this.Warned = true;
							}
							else if (this.Yandere.Club == this.Club && flag3 && this.ClubManager.ClubGrudge)
							{
								this.Interaction = StudentInteractionType.ClubKick;
								this.ClubManager.ClubsKickedFrom[(int)this.Club] = true;
								this.TalkTimer = 5f;
								this.Warned = true;
							}
							else if (this.CanBeFed)
							{
								this.Interaction = StudentInteractionType.Feeding;
								this.TalkTimer = 10f;
							}
							else if (this.CanTakeSnack)
							{
								this.Yandere.Interaction = YandereInteractionType.GivingSnack;
								this.Yandere.TalkTimer = 3f;
								this.Interaction = StudentInteractionType.Idle;
							}
							else if (this.CanGiveHelp)
							{
								this.Yandere.Interaction = YandereInteractionType.AskingForHelp;
								this.Yandere.TalkTimer = 5f;
								this.Interaction = StudentInteractionType.Idle;
							}
							else if (!this.StudentManager.Eighties && this.StudentID == 10 && this.StudentManager.TaskManager.TaskStatus[46] == 1 && this.Yandere.PickUp == null && !flag)
							{
								Debug.Log("The status of Budo's Task is: " + this.StudentManager.TaskManager.TaskStatus[46].ToString());
								Debug.Log("The game thinks that the current period is: " + this.Clock.Period.ToString());
								if (this.Clock.Period == 3 || this.Clock.Period == 5)
								{
									this.Yandere.NotificationManager.CustomText = "Martial Arts Club is not training now!";
									this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
									Debug.Log("HOW THE FUCK DID THE CODE GET HERE?????");
								}
								else
								{
									if (this.FollowTarget != null)
									{
										this.StudentManager.LastKnownOsana.position = this.FollowTarget.transform.position;
									}
									this.Interaction = StudentInteractionType.Idle;
									this.Yandere.Interaction = YandereInteractionType.TaskInquiry;
									this.Yandere.TalkTimer = 5f;
								}
							}
							else if (this.StudentID == 79 && this.DistanceToDestination < 1f && this.Actions[this.Phase] == StudentActionType.Wait)
							{
								this.Interaction = StudentInteractionType.WaitingForBeatEmUpResult;
							}
							else
							{
								if (this.Destinations[this.Phase] == null)
								{
									this.DistanceToDestination = 100f;
								}
								else
								{
									this.DistanceToDestination = Vector3.Distance(base.transform.position, this.Destinations[this.Phase].position);
								}
								if (this.Sleuthing && this.SleuthTarget != null)
								{
									this.DistanceToDestination = Vector3.Distance(base.transform.position, this.SleuthTarget.position);
								}
								if (flag3)
								{
									int num;
									if (this.Club == ClubType.Photography && this.Sleuthing)
									{
										num = 5;
									}
									else
									{
										num = 0;
									}
									if (this.StudentManager.EmptyDemon)
									{
										num = (int)(this.Club * (ClubType)(-1));
									}
									this.Subtitle.UpdateLabel(SubtitleType.ClubGreeting, (int)(this.Club + num), 4f);
									this.DialogueWheel.ClubLeader = true;
									this.Yandere.Jukebox.ClubTheme.clip = this.Yandere.Jukebox.ClubThemes[(int)this.Club];
									this.Yandere.Jukebox.ClubTheme.Play();
								}
								else
								{
									this.Subtitle.UpdateLabel(SubtitleType.Greeting, 0, 3f);
								}
								if (this.Club != ClubType.Council && this.Club != ClubType.Delinquent && ((this.Male && this.Yandere.Class.Seduction + this.Yandere.Class.SeductionBonus > 0) || this.Yandere.Class.Seduction + this.Yandere.Class.SeductionBonus > 4))
								{
									ParticleSystem.EmissionModule emission = this.Hearts.emission;
									emission.rateOverTime = (float)(this.Yandere.Class.Seduction + this.Yandere.Class.SeductionBonus);
									emission.enabled = true;
									this.Hearts.Play();
								}
								this.StudentManager.DisablePrompts();
								this.StudentManager.VolumeDown();
								this.DialogueWheel.HideShadows();
								this.DialogueWheel.Show = true;
								this.DialogueWheel.Panel.enabled = true;
								this.TalkTimer = 0f;
								if (this.Rival)
								{
									int num2 = this.DialogueWheel.Social.StudentFriendships[this.StudentID];
									float num3 = this.DialogueWheel.Social.StudentThresholds[this.StudentID] * 100f;
									this.Yandere.NotificationManager.CustomText = "Friendship: " + num2.ToString() + "/" + num3.ToString();
									this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
								}
							}
						}
						else if (flag3)
						{
							this.Interaction = StudentInteractionType.ClubUnwelcome;
							this.TalkTimer = 5f;
							this.Warned = true;
						}
						else
						{
							this.Interaction = StudentInteractionType.PersonalGrudge;
							this.TalkTimer = 5f;
							this.Warned = true;
						}
						this.Yandere.ShoulderCamera.OverShoulder = true;
						this.Pathfinding.canSearch = false;
						this.Pathfinding.canMove = false;
						this.Obstacle.enabled = true;
						this.Giggle = null;
						this.Yandere.WeaponMenu.KeyboardShow = false;
						this.Yandere.WeaponMenu.Show = false;
						this.Yandere.YandereVision = false;
						this.Yandere.CanMove = false;
						this.Yandere.Talking = true;
						this.Investigating = false;
						this.Talk.enabled = true;
						this.Reacted = false;
						this.Routine = false;
						this.Talking = true;
						this.TargetDistance = 0.5f;
						this.CuriosityPhase = 0;
						this.ReadPhase = 0;
						this.EmptyHands();
						bool flag13 = false;
						if (this.CurrentAction == StudentActionType.Sunbathe && this.SunbathePhase > 2)
						{
							this.SunbathePhase = 2;
							flag13 = true;
						}
						if (this.Phoneless)
						{
							this.SmartPhone.SetActive(false);
						}
						else if (this.Sleuthing)
						{
							if (!this.Scrubber.activeInHierarchy)
							{
								this.SmartPhone.SetActive(true);
							}
							else
							{
								this.SmartPhone.SetActive(false);
							}
						}
						else if (this.Persona != PersonaType.PhoneAddict)
						{
							this.SmartPhone.SetActive(false);
						}
						else if (!this.Scrubber.activeInHierarchy && !flag13)
						{
							this.SmartPhone.SetActive(true);
						}
						this.ChalkDust.Stop();
						this.StopPairing();
					}
				}
			}
		}
		if (this.Prompt.Circle[2].fillAmount == 0f || (this.Yandere.Sanity < 33.33333f && this.Yandere.CanMove && !this.Prompt.HideButton[2] && this.Prompt.InSight && this.Club != ClubType.Council && !this.Struggling && !this.Chasing && this.DistanceToPlayer < 1.4f && this.SeenByYandere() && this.StudentID > 1 && this.Yandere.EquippedWeapon != null && !this.Yandere.EquippedWeapon.Broken))
		{
			if (!this.Yandere.Armed && this.Drownable)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.AlarmDisc, base.transform.position + Vector3.up, Quaternion.identity);
				gameObject3.GetComponent<AlarmDiscScript>().Originator = this;
				gameObject3.GetComponent<AlarmDiscScript>().Silent = true;
				Debug.Log("Just began to drown someone.");
				if (this.VomitDoor != null)
				{
					this.VomitDoor.Prompt.enabled = true;
					this.VomitDoor.enabled = true;
				}
				this.Yandere.EmptyHands();
				this.Prompt.Hide();
				this.Prompt.enabled = false;
				this.Prompt.Circle[2].fillAmount = 1f;
				this.VomitEmitter.gameObject.SetActive(false);
				this.Police.DrownedStudentName = this.Name;
				this.MyController.enabled = false;
				this.SmartPhone.SetActive(false);
				this.Police.DrownVictims++;
				this.Distracted = true;
				this.Routine = false;
				this.Drowned = true;
				if (this.Male)
				{
					this.Subtitle.UpdateLabel(SubtitleType.DrownReaction, 1, 3f);
				}
				else
				{
					this.Subtitle.UpdateLabel(SubtitleType.DrownReaction, 0, 3f);
				}
				this.Yandere.TargetStudent = this;
				this.Yandere.Attacking = true;
				this.Yandere.CanMove = false;
				this.Yandere.Drown = true;
				this.Yandere.DrownAnim = "f02_fountainDrownA_00";
				if (this.Male)
				{
					if (Vector3.Distance(base.transform.position, this.StudentManager.transform.position) < 5f)
					{
						this.DrownAnim = "fountainDrown_00_B";
					}
					else
					{
						this.DrownAnim = "toiletDrown_00_B";
					}
				}
				else if (Vector3.Distance(base.transform.position, this.StudentManager.transform.position) < 5f)
				{
					this.DrownAnim = "f02_fountainDrownB_00";
				}
				else
				{
					this.DrownAnim = "f02_toiletDrownB_00";
				}
				this.CharacterAnimation.CrossFade(this.DrownAnim);
				return;
			}
			if (!this.Yandere.Armed && this.Pushable)
			{
				Debug.Log("The player is now pushing someone off of a rooftop.");
				this.Prompt.Circle[0].fillAmount = 1f;
				this.Prompt.Circle[2].fillAmount = 1f;
				if (!this.FocusOnYandere)
				{
					this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
					if (!this.Male)
					{
						this.Subtitle.UpdateLabel(SubtitleType.NoteReaction, 5, 3f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.NoteReactionMale, 5, 3f);
					}
					this.Prompt.Label[0].text = "     Talk";
					this.Yandere.TargetStudent = this;
					this.Yandere.FollowHips = true;
					this.Yandere.Attacking = true;
					this.Yandere.RoofPush = true;
					this.Yandere.CanMove = false;
					this.Yandere.EmptyHands();
					this.EmptyHands();
					this.Distracted = true;
					this.Routine = false;
					this.Pushed = true;
					this.CharacterAnimation.CrossFade(this.PushedAnim);
					this.RemoveOfferHelpPrompt();
					this.Yandere.PromptBar.ClearButtons();
					this.Yandere.PromptBar.Show = true;
					this.Yandere.PromptBar.Label[1].text = "Death Cam";
					this.Yandere.PromptBar.UpdateButtons();
					return;
				}
				this.Yandere.NotificationManager.CustomText = "Can't push someone who can see you!";
				this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
				return;
			}
			else if (this.Yandere.EquippedWeapon != null && this.Yandere.EquippedWeapon.Broken)
			{
				if (Input.GetButtonDown(InputNames.Xbox_X))
				{
					this.Yandere.NotificationManager.CustomText = "Can't attack! Weapon is broken!";
					this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
					return;
				}
			}
			else
			{
				Debug.Log(this.Name + " was just attacked, either because the player pressed the X button, or because Yandere-chan had low sanity.");
				if (this.Investigating)
				{
					this.StopInvestigating();
				}
				float f = Vector3.Angle(-base.transform.forward, this.Yandere.transform.position - base.transform.position);
				this.Yandere.AttackManager.Stealth = (Mathf.Abs(f) <= 45f);
				if (this.Yandere.EquippedWeapon.Type == WeaponType.Garrote && !this.Yandere.AttackManager.Stealth)
				{
					if (Input.GetButtonDown(InputNames.Xbox_X))
					{
						this.Yandere.NotificationManager.CustomText = "To strangle, approach from behind.";
						this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
					}
					return;
				}
				bool flag14 = false;
				if (this.Yandere.Armed && this.Yandere.AttackManager.Stealth && (this.Yandere.EquippedWeapon.Type == WeaponType.Bat || this.Yandere.EquippedWeapon.Type == WeaponType.Weight))
				{
					flag14 = true;
				}
				if (this.Yandere.Bloodiness > 0f)
				{
					flag14 = true;
				}
				if (flag14 || this.Yandere.Schoolwear == 2 || this.StudentManager.OriginalUniforms + this.StudentManager.NewUniforms > 1)
				{
					if (this.ClubActivityPhase < 16)
					{
						bool flag15 = false;
						if (this.Club == ClubType.Delinquent && !this.Injured && !this.Yandere.AttackManager.Stealth && !this.RespectEarned && !this.SolvingPuzzle && !this.Wet && this.MyWeapon != null && !this.Blind && !this.Yandere.Invisible && !this.EatingSnack)
						{
							Debug.Log(this.Name + " knows that Yandere-chan is trying to attack them, and will now shove (and spawn alarm disc).");
							this.Persona = PersonaType.Violent;
							flag15 = true;
							this.RespectEarned = false;
							this.Fleeing = false;
							this.Patience = 1;
							this.Shove();
							this.SpawnAlarmDisc();
						}
						if (this.Yandere.AttackManager.Stealth)
						{
							this.SpawnSmallAlarmDisc();
						}
						if (!flag15 && !this.Yandere.NearSenpai && !this.Yandere.Attacking && this.Yandere.Stance.Current != StanceType.Crouching)
						{
							if ((this.Yandere.Armed && this.Yandere.EquippedWeapon.Flaming) || this.Yandere.CyborgParts[1].activeInHierarchy)
							{
								this.Yandere.SanityBased = false;
							}
							if (this.Strength == 9 && !this.Emetic && !this.Lethal && !this.Sedated && !this.Headache)
							{
								Debug.Log("Opponent has a Strength of 9 - ''Invincible.''");
								if (this.Following)
								{
									this.StopFollowing();
								}
								if (!this.Male)
								{
									if (!this.Yandere.AttackManager.Stealth)
									{
										this.CharacterAnimation.CrossFade("f02_dramaticFrontal_00");
									}
									else
									{
										this.CharacterAnimation.CrossFade("f02_dramaticStealth_00");
									}
								}
								else if (!this.Yandere.AttackManager.Stealth)
								{
									this.CharacterAnimation.CrossFade("dramaticFrontal_00");
								}
								else
								{
									Debug.Log("This character should be playing the dramaticStealth_00 animation right now...");
									this.CharacterAnimation.CrossFade("dramaticStealth_00");
								}
								this.Yandere.CharacterAnimation.CrossFade("f02_readyToFight_00");
								this.Yandere.StruggleIminent = true;
								this.Yandere.CanMove = false;
								if (!this.StudentManager.ChallengeManager.InvincibleRaibaru && this.Yandere.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0)
								{
									Debug.Log("The player meets the criteria for being able to fight back against Raibaru.");
								}
								else
								{
									Debug.Log("The player does not meet the criteria for being able to fight back against Raibaru.");
									Debug.Log("The player is being set to Invisible here.");
									this.Yandere.Invisible = true;
								}
								this.DramaticCamera.enabled = true;
								this.DramaticCamera.rect = new Rect(0f, 0.5f, 1f, 0f);
								this.DramaticCamera.gameObject.SetActive(true);
								this.DramaticCamera.gameObject.GetComponent<AudioSource>().Play();
								this.DramaticReaction = true;
								this.EatingSnack = false;
								this.GoAway = false;
								this.Pathfinding.canSearch = false;
								this.Pathfinding.canMove = false;
								this.Routine = false;
								return;
							}
							if (this.Yandere.Armed)
							{
								if (this.Yandere.EquippedWeapon.WeaponID != 27 || (this.Yandere.EquippedWeapon.WeaponID == 27 && this.Yandere.AttackManager.Stealth))
								{
									this.AttackReaction();
									return;
								}
								if (this.Yandere.NotificationManager.Timer == 0f)
								{
									this.Yandere.NotificationManager.CustomText = "Approach victim from behind to strangle";
									this.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
									this.Yandere.NotificationManager.Timer = 5f;
									return;
								}
							}
						}
					}
				}
				else if (!this.Yandere.ClothingWarning)
				{
					this.Yandere.NotificationManager.DisplayNotification(NotificationType.Clothing);
					this.StudentManager.TutorialWindow.ShowClothingMessage = true;
					this.Yandere.ClothingWarning = true;
				}
			}
		}
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x001ED7B4 File Offset: 0x001EB9B4
	private void UpdateDying()
	{
		this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
		this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
		if (this.Attacked)
		{
			if (!this.Teacher)
			{
				if (this.Strength == 9 && !this.Emetic && !this.Lethal && !this.Sedated && !this.Headache)
				{
					if (!this.StudentManager.Stop)
					{
						this.StudentManager.StopMoving();
						this.Yandere.RPGCamera.enabled = false;
						this.SmartPhone.SetActive(false);
						this.Police.Show = false;
					}
					if (!this.Male)
					{
						this.CharacterAnimation.CrossFade("f02_moCounterB_00");
					}
					else
					{
						this.CharacterAnimation.CrossFade("moCounterB_00");
					}
					if (!this.WitnessedMurder && !this.Male && this.CharacterAnimation["f02_moLipSync_00"].weight == 0f)
					{
						this.CharacterAnimation["f02_moLipSync_00"].weight = 1f;
						this.CharacterAnimation["f02_moLipSync_00"].time = 0f;
						this.CharacterAnimation.Play("f02_moLipSync_00");
					}
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, Time.deltaTime * 10f);
					this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward);
					return;
				}
				this.EyeShrink = Mathf.Lerp(this.EyeShrink, 1f, Time.deltaTime * 10f);
				if (this.Alive && !this.Tranquil)
				{
					if (this.Yandere.SanityBased)
					{
						Vector3 vector = this.Yandere.transform.position + this.Yandere.transform.forward * this.Yandere.AttackManager.Distance;
						if (Vector3.Distance(vector, base.transform.position) > 0.5f)
						{
							base.transform.position = vector;
						}
						this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward * this.Yandere.AttackManager.Distance);
						if (!this.Yandere.AttackManager.Stealth)
						{
							this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
						}
						else
						{
							this.targetRotation = Quaternion.LookRotation(base.transform.position - new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z));
						}
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, Time.deltaTime * 10f);
						return;
					}
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, Time.deltaTime * 10f);
					if (this.Yandere.EquippedWeapon.WeaponID == 11)
					{
						this.CharacterAnimation.CrossFade(this.CyborgDeathAnim);
						this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward);
						if (this.CharacterAnimation[this.CyborgDeathAnim].time >= this.CharacterAnimation[this.CyborgDeathAnim].length - 0.25f && this.Yandere.EquippedWeapon.WeaponID == 11)
						{
							UnityEngine.Object.Instantiate<GameObject>(this.BloodyScream, base.transform.position + Vector3.up, Quaternion.identity);
							this.DeathType = DeathType.EasterEgg;
							this.BecomeRagdoll();
							this.Ragdoll.Dismember();
							return;
						}
					}
					else
					{
						if (this.Yandere.EquippedWeapon.WeaponID == 7)
						{
							this.CharacterAnimation.CrossFade(this.BuzzSawDeathAnim);
							this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward);
							return;
						}
						if (!this.Yandere.EquippedWeapon.Concealable)
						{
							this.CharacterAnimation.CrossFade(this.SwingDeathAnim);
							this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward);
							return;
						}
						this.CharacterAnimation.CrossFade(this.DefendAnim);
						this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward * 0.1f);
						return;
					}
				}
				else
				{
					this.CharacterAnimation.CrossFade(this.DeathAnim);
					if (this.CharacterAnimation[this.DeathAnim].time < 1f)
					{
						base.transform.Translate(Vector3.back * Time.deltaTime);
						return;
					}
					this.BecomeRagdoll();
					return;
				}
			}
			else
			{
				if (!this.StudentManager.Stop)
				{
					this.StudentManager.StopMoving();
					this.Yandere.Laughing = false;
					this.Yandere.Dipping = false;
					this.Yandere.RPGCamera.enabled = false;
					this.SmartPhone.SetActive(false);
					this.Police.Show = false;
				}
				this.CharacterAnimation.CrossFade(this.CounterAnim);
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, Time.deltaTime * 10f);
				this.MoveTowardsTarget(this.Yandere.transform.position + this.Yandere.transform.forward);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 10f);
			}
		}
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x001EDF9C File Offset: 0x001EC19C
	private void UpdatePushed()
	{
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.Pathfinding.target.rotation, Time.deltaTime * 10f);
		this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
		this.EyeShrink = Mathf.Lerp(this.EyeShrink, 1f, Time.deltaTime * 10f);
		if (this.CharacterAnimation[this.PushedAnim].time >= this.CharacterAnimation[this.PushedAnim].length)
		{
			this.BecomeRagdoll();
		}
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x001EE05C File Offset: 0x001EC25C
	private void UpdateDrowned()
	{
		this.SplashTimer += Time.deltaTime;
		if (this.SplashTimer > 3f && this.SplashTimer < 100f)
		{
			this.DrowningSplashes.Play();
			this.SplashTimer += 100f;
		}
		this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
		this.EyeShrink = Mathf.Lerp(this.EyeShrink, 1f, Time.deltaTime * 10f);
		if (this.CharacterAnimation[this.DrownAnim].time >= this.CharacterAnimation[this.DrownAnim].length)
		{
			this.BecomeRagdoll();
		}
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x001EE12C File Offset: 0x001EC32C
	private void UpdateWitnessedMurder()
	{
		if (this.Threatened)
		{
			this.UpdateAlarmed();
			return;
		}
		if (!this.Fleeing && !this.Shoving)
		{
			if (this.StudentID > 1 && this.Persona != PersonaType.Evil)
			{
				this.EyeShrink += Time.deltaTime * 0.2f;
			}
			Debug.Log(this.Name + " is now checking if Yandere-chan killed their loved one...");
			if ((this.Yandere.Attacking && this.Yandere.TargetStudent != null && this.LovedOneIsTargeted(this.Yandere.TargetStudent.StudentID)) || (this.Yandere.Struggling && this.Yandere.TargetStudent != null && this.LovedOneIsTargeted(this.Yandere.TargetStudent.StudentID)) || (this.ElectrocutionVictim > 0 && this.LovedOneIsTargeted(this.ElectrocutionVictim)) || (this.Yandere.Carrying && this.LovedOneIsTargeted(this.Yandere.CurrentRagdoll.StudentID)) || (this.Yandere.Dragging && this.LovedOneIsTargeted(this.Yandere.CurrentRagdoll.StudentID)) || (this.PartnerID > 0 && this.Yandere.Bloodiness > 0f && this.Yandere.NearestCorpseID == this.PartnerID))
			{
				Debug.Log(this.Name + " realized that the protagonist killed their loved one!");
				this.Strength = 5;
				this.Persona = PersonaType.Heroic;
				this.SmartPhone.SetActive(false);
				this.SprintAnim = this.OriginalSprintAnim;
				this.DoNotMourn = true;
				this.StudentToMourn = this.Yandere.TargetStudent;
				this.Corpse = this.Yandere.TargetStudent.Ragdoll;
			}
			if ((this.Club != ClubType.Delinquent || (this.Club == ClubType.Delinquent && this.Injured)) && this.Yandere.TargetStudent == null && this.LovedOneIsTargeted(this.Yandere.NearestCorpseID))
			{
				this.Strength = 5;
				if (this.Injured)
				{
					this.Strength = 1;
				}
				this.Persona = PersonaType.Heroic;
			}
			if (this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart != null && this.Yandere.PickUp.BodyPart.Type == 1 && this.LovedOneIsTargeted(this.Yandere.PickUp.BodyPart.StudentID))
			{
				this.Strength = 5;
				this.Persona = PersonaType.Heroic;
				this.SmartPhone.SetActive(false);
				this.SprintAnim = this.OriginalSprintAnim;
			}
			if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
			{
				if (!this.Attacked)
				{
					Debug.Log("Calling PhoneAddictCameraUpdate() from here, specifically.");
					this.PhoneAddictCameraUpdate();
				}
			}
			else
			{
				this.CharacterAnimation.CrossFade(this.ScaredAnim);
			}
			this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.position.x, base.transform.position.y, this.Yandere.Hips.position.z) - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
			if (!this.Yandere.Struggling)
			{
				if (this.Persona != PersonaType.Heroic && this.Persona != PersonaType.Dangerous && this.Persona != PersonaType.Violent)
				{
					this.AlarmTimer += Time.deltaTime * (float)this.MurdersWitnessed;
					if (this.Urgent && this.Yandere.CanMove)
					{
						if (this.StudentID == 1 && this.Persona != PersonaType.Evil)
						{
							this.SenpaiNoticed();
						}
						this.AlarmTimer += 5f;
					}
				}
				else
				{
					this.AlarmTimer += Time.deltaTime * ((float)this.MurdersWitnessed * 5f);
				}
			}
			else if (this.Yandere.Won)
			{
				this.Urgent = true;
			}
			if (this.AlarmTimer > 5f)
			{
				this.PersonaReaction();
				this.AlarmTimer = 0f;
			}
			else if (this.AlarmTimer > 1f && !this.Reacted)
			{
				if (this.StudentID == 1 && this.Persona != PersonaType.Evil && this.Yandere.Mask == null)
				{
					Debug.Log("Senpai witnessed murder, and entered a specific murder reaction animation.");
					this.MurderReaction = UnityEngine.Random.Range(1, 6);
					if (this.Male)
					{
						this.CharacterAnimation.CrossFade("senpaiMurderReaction_0" + this.MurderReaction.ToString());
						this.CharacterAnimation["scaredFace_00"].weight = 0f;
					}
					this.GameOverCause = GameOverType.Murder;
					if (!this.Yandere.Egg)
					{
						this.SenpaiNoticed();
					}
					this.CharacterAnimation[this.AngryFaceAnim].weight = 0f;
					this.Yandere.ShoulderCamera.enabled = true;
					this.Yandere.ShoulderCamera.Noticed = true;
					this.Yandere.RPGCamera.enabled = false;
					this.Stop = true;
				}
				else
				{
					if (this.StudentID == 1 && this.Persona != PersonaType.Evil)
					{
						Debug.Log("Senpai saw a mask.");
						this.Persona = PersonaType.Heroic;
						this.PersonaReaction();
					}
					if (!this.Teacher)
					{
						if (this.Persona != PersonaType.Evil)
						{
							if (this.Club == ClubType.Delinquent)
							{
								this.SmartPhone.SetActive(false);
							}
							else if (!this.StudentManager.Eighties && this.StudentID == 10)
							{
								Debug.Log("This is the exact moment that Raibaru witnesses Yandere-chan commit murder.");
								this.Subtitle.UpdateLabel(SubtitleType.ObstacleMurderReaction, 1, 3f);
								this.Yandere.Chasers++;
								this.Urgent = true;
							}
							else
							{
								this.Subtitle.UpdateLabel(SubtitleType.MurderReaction, 1, 3f);
							}
						}
					}
					else if (this.WitnessedCoverUp)
					{
						this.Subtitle.UpdateLabel(SubtitleType.TeacherCoverUpHostile, 1, 5f);
					}
					else
					{
						this.DetermineWhatWasWitnessed();
						this.DetermineTeacherSubtitle();
					}
				}
				this.Reacted = true;
			}
			if (!this.Male)
			{
				this.CharacterAnimation[this.ShyAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.ShyAnim].weight, 0f, Time.deltaTime);
			}
		}
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x001EE7E4 File Offset: 0x001EC9E4
	private void UpdateAlarmed()
	{
		if (this.ID == 30)
		{
			Debug.Log(this.Name + " is calling UpdateAlarmed()");
		}
		if (!this.Threatened)
		{
			if (this.Yandere.Medusa && this.YandereVisible)
			{
				this.TurnToStone();
				return;
			}
			if (this.Persona != PersonaType.PhoneAddict && !this.Sleuthing)
			{
				this.SmartPhone.SetActive(false);
			}
			this.OccultBook.SetActive(false);
			this.Drawing.SetActive(false);
			this.Pen.SetActive(false);
			this.SpeechLines.Stop();
			this.ReadPhase = 0;
			if (this.WitnessedCorpse)
			{
				if (!this.WalkBack)
				{
					int studentID = this.StudentID;
					if (this.Persona != PersonaType.PhoneAddict)
					{
						this.CharacterAnimation.CrossFade(this.ScaredAnim);
					}
					else if (!this.Phoneless && !this.Attacked)
					{
						if (this.Corpse.MurderSuicideAnimation)
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						else
						{
							this.PhoneAddictCameraUpdate();
						}
					}
				}
				else
				{
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.MyController.Move(base.transform.forward * (-0.5f * Time.deltaTime));
					this.MyController.Move(Physics.gravity * 0.1f);
					this.CharacterAnimation.CrossFade(this.WalkBackAnim);
					this.WalkBackTimer -= Time.deltaTime;
					if (this.WalkBackTimer <= 0f)
					{
						this.WalkBack = false;
					}
				}
			}
			else if (this.StudentID > 1)
			{
				if (this.Witness)
				{
					this.CharacterAnimation.CrossFade(this.LeanAnim);
				}
				else
				{
					if (!this.Investigating)
					{
						if (this.FocusOnStudent)
						{
							this.CharacterAnimation.CrossFade(this.LeanAnim);
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.LeanAnim);
						}
					}
					if (this.FocusOnYandere)
					{
						if (this.DistanceToPlayer < 1f && !this.Injured && ((this.Club == ClubType.Council && !this.DoNotShove) || (this.Club == ClubType.Delinquent && !this.Injured) || this.Shovey))
						{
							this.AlarmTimer = 0f;
							if (this.Yandere.CanMove)
							{
								this.ThreatTimer += Time.deltaTime;
							}
							if (this.ThreatTimer > 5f && !this.Yandere.Struggling && !this.Yandere.DelinquentFighting && !this.Yandere.Chased && this.Yandere.Chasers == 0 && this.Prompt.InSight)
							{
								this.ThreatTimer = 0f;
								this.Shove();
							}
						}
						this.DistractionSpot = new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z);
					}
				}
			}
			else
			{
				this.CharacterAnimation.CrossFade(this.LeanAnim);
			}
			if (this.WitnessedMurder)
			{
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.transform.position.x, base.transform.position.y, this.Yandere.transform.position.z) - base.transform.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
			}
			else if (this.WitnessedCorpse)
			{
				if (this.Corpse != null && this.Corpse.AllColliders[0] != null)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.Corpse.AllColliders[0].transform.position.x, base.transform.position.y, this.Corpse.AllColliders[0].transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				}
			}
			else if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
			{
				if (this.BloodPool != null)
				{
					this.targetRotation = Quaternion.LookRotation(new Vector3(this.BloodPool.transform.position.x, base.transform.position.y, this.BloodPool.transform.position.z) - base.transform.position);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				}
			}
			else if (!this.Investigating)
			{
				if (!this.FocusOnYandere)
				{
					if (this.DiscCheck)
					{
						this.targetRotation = Quaternion.LookRotation(new Vector3(this.DistractionSpot.x, base.transform.position.y, this.DistractionSpot.z) - base.transform.position);
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
					}
				}
				else
				{
					this.LookAtYandere();
				}
			}
			if (!this.Fleeing && !this.Yandere.DelinquentFighting)
			{
				this.AlarmTimer += Time.deltaTime * (1f - this.Hesitation);
			}
			if (!this.CanStillNotice)
			{
				this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia) * 5f;
			}
			if (this.AlarmTimer < 5f && this.BloodPool != null && this.CanSeeObject(this.Yandere.gameObject, this.Yandere.HeadPosition) && this.BloodPool.parent == this.Yandere.RightHand)
			{
				Debug.Log("ForgetAboutBloodPool() was called from this place in the code. 3");
				this.ForgetAboutBloodPool();
			}
			if (this.AlarmTimer > 5f)
			{
				if (!this.DramaticReaction)
				{
					this.EndAlarm();
				}
			}
			else if (this.AlarmTimer > 1f && !this.Reacted)
			{
				if (this.Teacher)
				{
					if (!this.WitnessedCorpse)
					{
						Debug.Log(this.Name + " witnessed: " + this.Witnessed.ToString());
						if (this.Concern > 4 || this.Witnessed == StudentWitnessType.Trespassing)
						{
							Debug.Log("A teacher's subtitle is now being determined.");
							this.CharacterAnimation.CrossFade(this.IdleAnim);
							switch (this.Witnessed)
							{
							case StudentWitnessType.Blood:
								this.Subtitle.UpdateLabel(SubtitleType.TeacherBloodReaction, 1, 6f);
								this.GameOverCause = GameOverType.Blood;
								break;
							case StudentWitnessType.BloodAndInsanity:
							case StudentWitnessType.Insanity:
							case StudentWitnessType.CleaningItem:
							case StudentWitnessType.Poisoning:
							case StudentWitnessType.WeaponAndBloodAndInsanity:
							case StudentWitnessType.WeaponAndInsanity:
								this.Subtitle.UpdateLabel(SubtitleType.TeacherInsanityReaction, 1, 6f);
								this.GameOverCause = GameOverType.Insanity;
								break;
							case StudentWitnessType.Lewd:
								Debug.Log(this.Name + ", using the Teacher Persona, witnessed lewd behavior.");
								this.Subtitle.UpdateLabel(SubtitleType.TeacherLewdReaction, 1, 6f);
								this.GameOverCause = GameOverType.Lewd;
								break;
							case StudentWitnessType.Pickpocketing:
							case StudentWitnessType.Theft:
								this.Subtitle.UpdateLabel(SubtitleType.TeacherTheftReaction, 1, 6f);
								break;
							case StudentWitnessType.Trespassing:
								Debug.Log("Witnessed was Trespassing, so we're using TeacherTrespassingReaction.");
								this.Subtitle.UpdateLabel(SubtitleType.TeacherTrespassingReaction, this.Concern, 5f);
								break;
							case StudentWitnessType.Violence:
								Debug.Log(this.Name + ", using the Teacher Persona, witnessed violence.");
								this.Subtitle.UpdateLabel(SubtitleType.TeacherTrespassingReaction, 1, 6f);
								this.GameOverCause = GameOverType.Violence;
								this.Concern = 5;
								break;
							case StudentWitnessType.Weapon:
							case StudentWitnessType.WeaponAndBlood:
								this.Subtitle.UpdateLabel(SubtitleType.TeacherWeaponReaction, 1, 6f);
								this.GameOverCause = GameOverType.Weapon;
								break;
							}
						}
						else
						{
							Debug.Log("Concerned was below 5 and player was not trespassing, so we're using HmmReaction.");
							if (this.WitnessedSlave)
							{
								this.Subtitle.UpdateLabel(SubtitleType.SlaveReaction, 1, 3f);
							}
							else
							{
								this.Subtitle.UpdateLabel(SubtitleType.HmmReaction, 1, 3f);
							}
						}
						if (this.Club == ClubType.Council)
						{
							UnityEngine.Object.Destroy(this.Subtitle.CurrentClip);
							this.Subtitle.UpdateLabel(SubtitleType.CouncilToCounselor, this.ClubMemberID, 6f);
						}
						if (!this.Yandere.Noticed && this.BloodPool != null)
						{
							Debug.Log(this.Name + ", using the Teacher Persona, was alarmed because she saw something weird on the ground - a " + this.BloodPool.name);
							UnityEngine.Object.Destroy(this.Subtitle.CurrentClip);
							this.Subtitle.UpdateLabel(SubtitleType.BloodPoolReaction, 2, 5f);
							PromptScript component = this.BloodPool.GetComponent<PromptScript>();
							if (component != null)
							{
								WeaponScript component2 = this.BloodPool.GetComponent<WeaponScript>();
								bool flag = false;
								if (component2 != null)
								{
									if (component2.BroughtFromHome)
									{
										Debug.Log("This weapon was brought from home!");
										flag = true;
									}
									else
									{
										Debug.Log("This weapon was not brought from home.");
									}
								}
								if (!flag)
								{
									Debug.Log("Disabling a bloody object's prompt because " + this.Name + ", using the Teacher Persona, is heading for it.");
									component.Hide();
									component.enabled = false;
								}
							}
							if (this.ReportPhase == 2)
							{
								Debug.Log("This teacher noticed the suspicious object while on her way to investigate something, and is now updating her pathfinding target.");
								this.DetermineBloodLocation();
							}
						}
					}
					else
					{
						Debug.Log("A teacher found a corpse.");
						this.Concern = 1;
						this.DetermineWhatWasWitnessed();
						this.DetermineTeacherSubtitle();
						if (this.WitnessedMurder)
						{
							this.MurdersWitnessed++;
							if (!this.Yandere.Chased)
							{
								Debug.Log("A teacher has reached ChaseYandere() through UpdateAlarm().");
								this.ChaseYandere();
							}
						}
					}
					if (!this.Chasing && !this.Attacked && ((this.YandereVisible && this.Concern == 5) || this.Yandere.Noticed))
					{
						Debug.Log("Yandere-chan is getting sent to the guidance counselor.");
						if (this.Witnessed == StudentWitnessType.Theft && this.Yandere.StolenObject != null)
						{
							this.Yandere.StolenObject.SetActive(true);
							this.Yandere.StolenObject = null;
							this.Yandere.Inventory.IDCard = false;
						}
						this.StudentManager.CombatMinigame.Stop();
						this.CharacterAnimation[this.AngryFaceAnim].weight = 1f;
						this.Yandere.ShoulderCamera.enabled = true;
						this.Yandere.ShoulderCamera.Noticed = true;
						this.Yandere.RPGCamera.enabled = false;
						this.Stop = true;
					}
				}
				else if (this.StudentID == 1 && this.Persona != PersonaType.Evil && this.Yandere.Mask == null && !this.WitnessedWeapon)
				{
					Debug.Log("We are now determining what Senpai saw...");
					if (this.Witnessed == StudentWitnessType.WeaponAndBloodAndInsanity)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiInsanityReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Insanity;
					}
					else if (this.Witnessed == StudentWitnessType.WeaponAndBlood)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiWeaponReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Weapon;
					}
					else if (this.Witnessed == StudentWitnessType.WeaponAndInsanity)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiInsanityReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Insanity;
					}
					else if (this.Witnessed == StudentWitnessType.BloodAndInsanity)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiInsanityReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Insanity;
					}
					else if (this.Witnessed == StudentWitnessType.Weapon)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiWeaponReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Weapon;
					}
					else if (this.Witnessed == StudentWitnessType.Blood)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiBloodReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Blood;
					}
					else if (this.Witnessed == StudentWitnessType.Insanity)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiInsanityReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Insanity;
					}
					else if (this.Witnessed == StudentWitnessType.Lewd || this.Witnessed == StudentWitnessType.Poisoning)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiLewdReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Lewd;
					}
					else if (this.Witnessed == StudentWitnessType.Stalking)
					{
						if (this.Concern < 5)
						{
							this.Subtitle.UpdateLabel(SubtitleType.SenpaiStalkingReaction, this.Concern, 4.5f);
						}
						else
						{
							if (this.Male)
							{
								this.CharacterAnimation.CrossFade("senpaiCreepyReaction_00");
							}
							else
							{
								this.CharacterAnimation.CrossFade(this.ScaredAnim);
							}
							this.GameOverCause = GameOverType.Stalking;
						}
						this.Witnessed = StudentWitnessType.None;
					}
					else if (this.Witnessed == StudentWitnessType.Corpse)
					{
						if (this.Corpse.Student.Rival)
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.UpdateLabel(SubtitleType.SenpaiRivalDeathReaction, 1, 5f);
							Debug.Log("Senpai is reacting to Osana's corpse with a unique subtitle.");
						}
						else
						{
							this.Subtitle.UpdateLabel(SubtitleType.SenpaiCorpseReaction, 1, 5f);
						}
					}
					else if (this.Witnessed == StudentWitnessType.Violence)
					{
						if (this.Male)
						{
							this.CharacterAnimation.CrossFade("senpaiFightReaction_00");
						}
						else
						{
							this.CharacterAnimation.CrossFade(this.ScaredAnim);
						}
						this.GameOverCause = GameOverType.Violence;
						this.Concern = 5;
					}
					else
					{
						Debug.Log("Senpai witnessed...nothing?!");
					}
					if (this.Concern == 5)
					{
						if (this.Male)
						{
							this.CharacterAnimation["scaredFace_00"].weight = 0f;
						}
						this.CharacterAnimation[this.AngryFaceAnim].weight = 0f;
						this.Yandere.ShoulderCamera.enabled = true;
						this.Yandere.ShoulderCamera.Noticed = true;
						this.Yandere.RPGCamera.enabled = false;
						this.Stop = true;
					}
				}
				else if (this.StudentID == 41 && !this.StudentManager.Eighties)
				{
					this.Subtitle.UpdateLabel(SubtitleType.Impatience, 6, 5f);
				}
				else if (this.RepeatReaction)
				{
					if (!this.StudentManager.Eighties && this.StudentID == 48 && this.TaskPhase == 4 && this.Yandere.Armed && this.Yandere.EquippedWeapon.WeaponID == 12)
					{
						this.Subtitle.CustomText = "Is that dumbbell for me? Drop it over here!";
						this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					}
					else if (!this.StudentManager.Eighties && this.StudentID == 50 && this.TaskPhase == 4 && this.Yandere.Armed && this.Yandere.EquippedWeapon.WeaponID == 24)
					{
						this.Subtitle.CustomText = "Are you going to use that pipe wrench to fix the training dummy?";
						this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.RepeatReaction, 1, 3f);
						this.RepeatReaction = false;
					}
				}
				else if (this.Club != ClubType.Delinquent)
				{
					if (this.Witnessed == StudentWitnessType.WeaponAndBloodAndInsanity)
					{
						this.Subtitle.UpdateLabel(SubtitleType.WeaponAndBloodAndInsanityReaction, 1, 3f);
					}
					else if (this.Witnessed == StudentWitnessType.WeaponAndBlood)
					{
						this.Subtitle.UpdateLabel(SubtitleType.WeaponAndBloodReaction, 1, 3f);
					}
					else if (this.Witnessed == StudentWitnessType.WeaponAndInsanity)
					{
						this.Subtitle.UpdateLabel(SubtitleType.WeaponAndInsanityReaction, 1, 3f);
					}
					else if (this.Witnessed == StudentWitnessType.BloodAndInsanity)
					{
						this.Subtitle.UpdateLabel(SubtitleType.BloodAndInsanityReaction, 1, 3f);
					}
					else if (this.Witnessed == StudentWitnessType.Weapon)
					{
						Debug.Log("Witnessed a weapon. That weapon's ID is: " + this.WeaponWitnessed.ToString());
						this.Subtitle.StudentID = this.StudentID;
						if (this.PlayerHeldBloodyWeapon)
						{
							this.Subtitle.CustomText = "Why is that thing covered in blood?! Did you hurt someone?!";
							this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							this.PlayerHeldBloodyWeapon = false;
						}
						else
						{
							this.Subtitle.UpdateLabel(SubtitleType.WeaponReaction, this.WeaponWitnessed, 3f);
						}
					}
					else if (this.Witnessed == StudentWitnessType.Blood)
					{
						if (!this.Bloody)
						{
							this.Subtitle.UpdateLabel(SubtitleType.BloodReaction, 1, 3f);
						}
						else
						{
							this.Subtitle.UpdateLabel(SubtitleType.WetBloodReaction, 1, 3f);
							this.Witnessed = StudentWitnessType.None;
							this.Witness = false;
						}
					}
					else if (this.Witnessed == StudentWitnessType.Insanity)
					{
						this.Subtitle.UpdateLabel(SubtitleType.InsanityReaction, 1, 3f);
					}
					else if (this.Witnessed == StudentWitnessType.Lewd)
					{
						this.Subtitle.UpdateLabel(SubtitleType.LewdReaction, 1, 3f);
					}
					else if (this.Witnessed == StudentWitnessType.CleaningItem)
					{
						this.Subtitle.UpdateLabel(SubtitleType.SuspiciousReaction, 0, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.Suspicious)
					{
						this.Subtitle.UpdateLabel(SubtitleType.SuspiciousReaction, 1, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.Corpse)
					{
						Debug.Log(this.Name + " is currently reacting to the corpse of " + this.Corpse.Student.Name + " and is deciding what subtitle to use.");
						if (!this.StudentManager.Eighties && this.StudentID == this.StudentManager.ObstacleID && this.Corpse.Student.Rival)
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 1, 5f);
							Debug.Log("Raibaru is reacting to Osana's corpse with a unique subtitle.");
						}
						else if (!this.StudentManager.Eighties && this.StudentID == 11 && this.Corpse.StudentID == this.StudentManager.ObstacleID)
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.UpdateLabel(SubtitleType.OsanaObstacleDeathReaction, 1, 5f);
							Debug.Log("Osana is reacting to Raibaru's corpse with a unique subtitle.");
						}
						else if (!this.StudentManager.Eighties && this.StudentID > 1 && this.StudentID < 4 && this.Corpse.StudentID > 1 && this.Corpse.StudentID < 4)
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.CustomText = "Sister?! What's wrong?! Are you hurt?!";
							this.Subtitle.UpdateLabel(SubtitleType.Custom, 1, 5f);
							Debug.Log("A Basu sister is reacting to her sister's death with a unique subtitle.");
						}
						else if (this.Club == ClubType.Council)
						{
							if (this.StudentID == 86)
							{
								this.Subtitle.UpdateLabel(SubtitleType.CouncilCorpseReaction, 1, 5f);
							}
							else if (this.StudentID == 87)
							{
								this.Subtitle.UpdateLabel(SubtitleType.CouncilCorpseReaction, 2, 5f);
							}
							else if (this.StudentID == 88)
							{
								this.Subtitle.UpdateLabel(SubtitleType.CouncilCorpseReaction, 3, 5f);
							}
							else if (this.StudentID == 89)
							{
								this.Subtitle.UpdateLabel(SubtitleType.CouncilCorpseReaction, 4, 5f);
							}
						}
						else if (this.Persona == PersonaType.Evil)
						{
							this.Subtitle.UpdateLabel(SubtitleType.EvilCorpseReaction, 1, 5f);
						}
						else if (this.WitnessedMindBrokenMurder)
						{
							this.Subtitle.CustomText = "This can't be happening...";
							this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
						}
						else if (this.InCouple && this.Corpse.StudentID == this.PartnerID)
						{
							this.Subtitle.CustomText = "No...NO...!";
							this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
						}
						else if (!this.Corpse.Choking)
						{
							Debug.Log("Corpse Reaction Subtitle #1");
							this.Subtitle.UpdateLabel(SubtitleType.CorpseReaction, 0, 5f);
						}
						else
						{
							Debug.Log("Corpse Reaction Subtitle #2");
							this.Subtitle.UpdateLabel(SubtitleType.CorpseReaction, 1, 5f);
						}
					}
					else if (this.Witnessed == StudentWitnessType.Interruption)
					{
						if (this.StudentID == 11)
						{
							this.Subtitle.UpdateLabel(SubtitleType.InterruptionReaction, 1, 5f);
						}
						else
						{
							this.Subtitle.UpdateLabel(SubtitleType.InterruptionReaction, 2, 5f);
						}
					}
					else if (this.Witnessed == StudentWitnessType.Eavesdropping)
					{
						if (this.StudentID == 10 || (this.StudentManager.Eighties && this.Rival))
						{
							this.Subtitle.UpdateLabel(SubtitleType.RivalEavesdropReaction, 0, 9f);
							this.Hesitation = 0.6f;
						}
						else if (this.Rival)
						{
							this.Subtitle.UpdateLabel(SubtitleType.RivalEavesdropReaction, DateGlobals.Week, 9f);
							this.Hesitation = 0.6f;
						}
						else if (this.EventInterrupted)
						{
							this.Subtitle.UpdateLabel(SubtitleType.EventEavesdropReaction, 1, 5f);
							this.EventInterrupted = false;
						}
						else
						{
							this.Subtitle.UpdateLabel(SubtitleType.EavesdropReaction, 1, 5f);
						}
					}
					else if (this.Witnessed == StudentWitnessType.Pickpocketing)
					{
						this.Subtitle.UpdateLabel(this.PickpocketSubtitleType, 1, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.Violence)
					{
						this.Subtitle.UpdateLabel(SubtitleType.ViolenceReaction, 5, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.Poisoning)
					{
						if (this.Yandere.TargetBento != null)
						{
							if (this.Yandere.TargetBento.StudentID != this.StudentID)
							{
								this.Subtitle.UpdateLabel(SubtitleType.PoisonReaction, 1, 5f);
							}
							else
							{
								Debug.Log(this.Name + " witnessed their own bento being poisoned.");
								this.Subtitle.UpdateLabel(SubtitleType.PoisonReaction, 2, 5f);
								this.NotEating = true;
								if (this.Clock.Period == 3)
								{
									this.Phase++;
									this.Pathfinding.target = this.Destinations[this.Phase];
									this.CurrentDestination = this.Destinations[this.Phase];
								}
							}
						}
						else
						{
							Debug.Log("Player was caught poisoning a bento that is a part of an event.");
							if (this.StudentID == 11)
							{
								Debug.Log("Osana witnessed it.");
								if (this.StudentManager.Portal.GetComponent<PortalScript>().OsanaEvent.Bentos[1].GetComponent<BentoScript>().BeingPoisoned)
								{
									Debug.Log("Osana witnessed Senpai's bento being poisoned.");
									this.StudentManager.Portal.GetComponent<PortalScript>().OsanaMondayLunchEvent.enabled = false;
									this.Subtitle.CustomText = "What are you doing to Senpai's bento?! Ugh, now I can't give it to him...";
									this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
									this.StudentManager.Students[1].MyBento.Tampered = false;
									this.StudentManager.Students[1].MyBento.Emetic = false;
									this.StudentManager.Students[1].Emetic = false;
								}
								else if (this.StudentManager.Portal.GetComponent<PortalScript>().OsanaEvent.Bentos[2].GetComponent<BentoScript>().BeingPoisoned)
								{
									Debug.Log("Osana witnessed her own bento being poisoned.");
									this.StudentManager.Portal.GetComponent<PortalScript>().OsanaMondayLunchEvent.enabled = false;
									this.Subtitle.CustomText = "What are you doing to my bento?! Well, now I'm not going to eat it...";
									this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
									Debug.Log("Osana will spend her lunchtime looking sad.");
									this.ScheduleBlocks[4].action = "Shamed";
									this.GetDestinations();
								}
							}
							else
							{
								this.Subtitle.UpdateLabel(SubtitleType.PoisonReaction, 1, 5f);
							}
						}
					}
					else if (this.Witnessed == StudentWitnessType.SeveredLimb)
					{
						this.Subtitle.UpdateLabel(SubtitleType.LimbReaction, 0, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.BloodyWeapon)
					{
						this.Subtitle.UpdateLabel(SubtitleType.BloodyWeaponReaction, 0, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.DroppedWeapon)
					{
						this.Subtitle.UpdateLabel(SubtitleType.BloodyWeaponReaction, 0, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.BloodPool)
					{
						this.Subtitle.UpdateLabel(SubtitleType.BloodPoolReaction, 0, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.HoldingBloodyClothing)
					{
						this.Subtitle.UpdateLabel(SubtitleType.HoldingBloodyClothingReaction, 0, 5f);
					}
					else if (this.Witnessed == StudentWitnessType.Theft)
					{
						if (this.StudentID == 2 && this.RingReact)
						{
							this.Subtitle.UpdateLabel(SubtitleType.TheftReaction, 1, 5f);
						}
						else
						{
							this.Subtitle.UpdateLabel(SubtitleType.TheftReaction, 0, 5f);
						}
					}
					else if (this.Witnessed == StudentWitnessType.Tutorial)
					{
						this.Subtitle.UpdateLabel(SubtitleType.TutorialReaction, 0, 10f);
					}
					else if (this.Witnessed == StudentWitnessType.Trespassing)
					{
						this.Subtitle.UpdateLabel(SubtitleType.TrespassReaction, 0, 10f);
					}
					else if (this.WitnessedSlave)
					{
						this.Subtitle.UpdateLabel(SubtitleType.SlaveReaction, 1, 3f);
					}
					else if (this.Club == ClubType.Council)
					{
						this.Subtitle.UpdateLabel(SubtitleType.HmmReaction, this.ClubMemberID, 3f);
						this.TemporarilyBlind = false;
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.HmmReaction, 0, 3f);
					}
				}
				else if (this.Witnessed == StudentWitnessType.None)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentHmm, 0, 5f);
				}
				else if (this.Witnessed == StudentWitnessType.Corpse)
				{
					if (this.FoundEnemyCorpse)
					{
						this.Subtitle.UpdateLabel(SubtitleType.EvilDelinquentCorpseReaction, 1, 5f);
					}
					else if (this.Corpse.Student.Club == ClubType.Delinquent)
					{
						this.Subtitle.Speaker = this;
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentFriendCorpseReaction, 1, 5f);
						this.FoundFriendCorpse = true;
					}
					else
					{
						this.Subtitle.Speaker = this;
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentCorpseReaction, 1, 5f);
					}
				}
				else if (this.Witnessed == StudentWitnessType.Weapon && !this.Injured)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentWeaponReaction, 0, 3f);
				}
				else
				{
					this.Subtitle.Speaker = this;
					if (this.WitnessedLimb || this.WitnessedWeapon || this.WitnessedBloodPool || this.WitnessedBloodyWeapon)
					{
						this.Subtitle.UpdateLabel(SubtitleType.LimbReaction, 0, 5f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentReaction, 0, 5f);
						Debug.Log("A delinquent is reacting to Yandere-chan's behavior.");
					}
				}
				this.Reacted = true;
			}
			if (((this.Club == ClubType.Council && !this.DoNotShove) || this.Shovey) && this.DistanceToPlayer < 1.1f && !this.Yandere.Invisible && !this.EatingSnack && (this.Yandere.Armed || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed)) && this.Prompt.InSight)
			{
				if (this.Yandere.Armed && !this.Yandere.EquippedWeapon.Suspicious && !this.WitnessedMurder && !this.WitnessedCorpse && !this.Yandere.Chased && this.Yandere.Chasers == 0)
				{
					Debug.Log(this.Name + " is shoving the player from this place in the code. 1");
					this.Shove();
					return;
				}
				Debug.Log("Calling ''Spray()'' from this part of the code. 3");
				this.Spray();
				return;
			}
		}
		else
		{
			this.Alarm -= Time.deltaTime * 100f * (1f / this.Paranoia);
			if (this.StudentManager.CombatMinigame.Delinquent == null || this.StudentManager.CombatMinigame.Delinquent == this)
			{
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
			}
			else
			{
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.StudentManager.CombatMinigame.Midpoint.position.x, base.transform.position.y, this.StudentManager.CombatMinigame.Midpoint.position.z) - base.transform.position);
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
			if (this.Yandere.DelinquentFighting && this.StudentManager.CombatMinigame.Delinquent != this)
			{
				if (this.StudentManager.CombatMinigame.Path >= 4)
				{
					this.CharacterAnimation.CrossFade(this.IdleAnim, 5f);
					this.NoTalk = true;
					return;
				}
				if (this.DistanceToPlayer < 1f)
				{
					this.MyController.Move(base.transform.forward * Time.deltaTime * -1f);
				}
				if (Vector3.Distance(base.transform.position, this.StudentManager.CombatMinigame.Delinquent.transform.position) < 1f)
				{
					this.MyController.Move(base.transform.forward * Time.deltaTime * -1f);
				}
				if (this.Yandere.enabled)
				{
					this.CheerTimer = Mathf.MoveTowards(this.CheerTimer, 0f, Time.deltaTime);
					if (this.CheerTimer == 0f)
					{
						this.Subtitle.Speaker = this;
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentCheer, 0, 5f);
						this.CheerTimer = UnityEngine.Random.Range(2f, 3f);
					}
				}
				this.CharacterAnimation.CrossFade(this.RandomCheerAnim);
				if (this.CharacterAnimation[this.RandomCheerAnim].time >= this.CharacterAnimation[this.RandomCheerAnim].length)
				{
					this.RandomCheerAnim = this.CheerAnims[UnityEngine.Random.Range(0, this.CheerAnims.Length)];
				}
				this.ThreatPhase = 3;
				this.ThreatTimer = 0f;
				if (this.WitnessedMurder)
				{
					this.Injured = true;
					return;
				}
			}
			else if (!this.Injured)
			{
				if (this.DistanceToPlayer > 5f + this.ThreatDistance && this.ThreatPhase < 4)
				{
					this.ThreatPhase = 3;
					this.ThreatTimer = 0f;
				}
				if (!this.Yandere.Dumping && !this.Yandere.SneakingShot)
				{
					if (this.DistanceToPlayer > 1f && this.Patience > 0)
					{
						if (this.ThreatPhase == 1)
						{
							this.CharacterAnimation.CrossFade("delinquentShock_00");
							if (this.CharacterAnimation["delinquentShock_00"].time >= this.CharacterAnimation["delinquentShock_00"].length)
							{
								this.Subtitle.Speaker = this;
								this.Subtitle.UpdateLabel(SubtitleType.DelinquentThreatened, 0, 3f);
								this.CharacterAnimation.CrossFade("delinquentCombatIdle_00");
								this.ThreatTimer = 5f;
								this.ThreatPhase++;
								return;
							}
						}
						else if (this.ThreatPhase == 2)
						{
							this.ThreatTimer -= Time.deltaTime;
							if (this.ThreatTimer < 0f)
							{
								this.Subtitle.Speaker = this;
								this.Subtitle.UpdateLabel(SubtitleType.DelinquentTaunt, 0, 5f);
								this.ThreatTimer = 5f;
								this.ThreatPhase++;
								return;
							}
						}
						else if (this.ThreatPhase == 3)
						{
							this.ThreatTimer -= Time.deltaTime;
							if (this.ThreatTimer < 0f)
							{
								if (!this.NoTalk)
								{
									this.Subtitle.Speaker = this;
									this.Subtitle.UpdateLabel(SubtitleType.DelinquentCalm, 0, 5f);
								}
								this.CharacterAnimation.CrossFade(this.IdleAnim, 5f);
								this.ThreatTimer = 5f;
								this.ThreatPhase++;
								return;
							}
						}
						else if (this.ThreatPhase == 4)
						{
							this.ThreatTimer -= Time.deltaTime;
							if (this.ThreatTimer < 0f)
							{
								if (this.CurrentDestination != this.Destinations[this.Phase])
								{
									this.StopInvestigating();
								}
								this.FocusOnStudent = false;
								this.FocusOnYandere = false;
								this.Distracted = false;
								this.Threatened = false;
								this.Alarmed = false;
								this.Routine = true;
								this.NoTalk = false;
								this.IgnoreTimer = 5f;
								this.AlarmTimer = 0f;
								return;
							}
						}
					}
					else if (!this.NoTalk)
					{
						this.Yandere.Shoved = false;
						Debug.Log("The combat minigame is now beginning.");
						this.Yandere.CustomThreshold = 5f;
						this.Yandere.WallToRight = false;
						this.Yandere.Direction = 2;
						this.Yandere.CheckForWall();
						if (this.Yandere.WallToRight)
						{
							Debug.Log("Trying to move Yandere-chan to the left.");
							this.Yandere.MyController.Move(this.Yandere.transform.right * Time.deltaTime * -1f);
						}
						this.Yandere.CustomThreshold = 0f;
						string str = "";
						if (!this.Male)
						{
							str = "Female_";
						}
						if (this.StudentID == 46)
						{
							this.CharacterAnimation.CrossFade("delinquentDrawWeapon_00");
						}
						if (this.StudentManager.CombatMinigame.Delinquent == null)
						{
							this.Yandere.CharacterAnimation.CrossFade("Yandere_CombatIdle");
							if (this.MyWeapon.transform.parent != this.ItemParent)
							{
								string str2 = "The game is trying to tell ";
								StudentScript delinquent = this.StudentManager.CombatMinigame.Delinquent;
								Debug.Log(str2 + ((delinquent != null) ? delinquent.ToString() : null) + "to draw out a weapon.");
								this.CharacterAnimation.CrossFade(str + "delinquentDrawWeapon_00");
							}
							else
							{
								this.CharacterAnimation.CrossFade("delinquentCombatIdle_00");
							}
							if ((this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed))
							{
								this.Yandere.EmptyHands();
							}
							if (this.Yandere.Pickpocketing)
							{
								this.Yandere.Caught = true;
								this.PickPocket.PickpocketMinigame.End();
							}
							this.Yandere.StopLaughing();
							this.Yandere.StopAiming();
							this.Yandere.Unequip();
							if (this.Yandere.YandereVision)
							{
								this.Yandere.YandereVision = false;
								this.Yandere.ResetYandereEffects();
							}
							if (this.Yandere.PickUp != null)
							{
								this.Yandere.EmptyHands();
							}
							this.Yandere.DelinquentFighting = true;
							this.Yandere.NearSenpai = false;
							this.Yandere.Degloving = false;
							this.Yandere.CanMove = false;
							this.Yandere.GloveTimer = 0f;
							this.FocusOnStudent = false;
							this.FocusOnYandere = false;
							this.Distracted = true;
							this.Fighting = true;
							this.ThreatTimer = 0f;
							this.StudentManager.CombatMinigame.Delinquent = this;
							this.StudentManager.CombatMinigame.enabled = true;
							this.StudentManager.CombatMinigame.StartCombat();
							this.SpeechLines.Stop();
							this.SpawnAlarmDisc();
							if (this.WitnessedMurder || this.WitnessedCorpse)
							{
								this.Subtitle.Speaker = this;
								this.Subtitle.UpdateLabel(SubtitleType.DelinquentAvenge, 0, 5f);
							}
							else if (!this.StudentManager.CombatMinigame.Practice)
							{
								this.Subtitle.Speaker = this;
								this.Subtitle.UpdateLabel(SubtitleType.DelinquentFight, 0, 5f);
							}
						}
						this.Yandere.transform.rotation = Quaternion.LookRotation(new Vector3(this.Hips.transform.position.x, this.Yandere.transform.position.y, this.Hips.transform.position.z) - this.Yandere.transform.position);
						if (this.CharacterAnimation[str + "delinquentDrawWeapon_00"].time >= 0.5f)
						{
							this.MyWeapon.transform.parent = this.ItemParent;
							this.MyWeapon.transform.localEulerAngles = new Vector3(0f, 15f, 0f);
							this.MyWeapon.transform.localPosition = new Vector3(0.01f, 0f, 0f);
						}
						if (this.CharacterAnimation[str + "delinquentDrawWeapon_00"].time >= this.CharacterAnimation[str + "delinquentDrawWeapon_00"].length)
						{
							this.Threatened = false;
							this.Alarmed = false;
							base.enabled = false;
							return;
						}
					}
					else
					{
						this.ThreatTimer -= Time.deltaTime;
						if (this.ThreatTimer < 0f)
						{
							if (this.CurrentDestination != this.Destinations[this.Phase])
							{
								this.StopInvestigating();
							}
							this.Distracted = false;
							this.Threatened = false;
							this.Alarmed = false;
							this.Routine = true;
							this.NoTalk = false;
							this.IgnoreTimer = 5f;
							this.AlarmTimer = 0f;
							return;
						}
					}
				}
			}
			else
			{
				this.ThreatTimer += Time.deltaTime;
				if (this.ThreatTimer > 5f)
				{
					this.DistanceToDestination = 100f;
					if (this.Yandere.CanMove)
					{
						if (!this.WitnessedMurder && !this.WitnessedCorpse)
						{
							this.Distracted = false;
							this.Threatened = false;
							this.EndAlarm();
							return;
						}
						this.Threatened = false;
						this.Alarmed = false;
						this.Injured = false;
						this.PersonaReaction();
					}
				}
			}
		}
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x001F10C0 File Offset: 0x001EF2C0
	private void UpdateBurning()
	{
		if (this.DistanceToPlayer < 1f && !this.Yandere.Shoved && !this.Yandere.Egg)
		{
			if (this.Yandere.PickUp != null && this.Yandere.PickUp.OpenFlame && !this.Yandere.Invisible)
			{
				this.Yandere.PotentiallyMurderousTimer = 1f;
			}
			this.PushYandereAway();
		}
		if (this.BurnTarget != Vector3.zero)
		{
			this.MoveTowardsTarget(this.BurnTarget);
		}
		if (this.CharacterAnimation[this.BurningAnim].time > this.CharacterAnimation[this.BurningAnim].length)
		{
			this.DeathType = DeathType.Burning;
			this.BecomeRagdoll();
			return;
		}
		if (this.CharacterAnimation[this.BurningAnim].time > 8f)
		{
			this.CheckForWallInFront(2f);
			if (this.TooCloseToWall)
			{
				this.MyController.Move(base.transform.forward * Time.deltaTime * -0.5f);
			}
		}
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x001F11F4 File Offset: 0x001EF3F4
	private void UpdateSplashed()
	{
		this.CharacterAnimation.CrossFade(this.SplashedAnim);
		if (this.Yandere.Tripping)
		{
			this.targetRotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
		}
		this.SplashTimer += Time.deltaTime;
		if (this.SplashTimer > 2f && this.SplashPhase == 1)
		{
			if (this.Gas)
			{
				this.Subtitle.Speaker = this;
				this.Subtitle.UpdateLabel(this.SplashSubtitleType, 5, 5f);
			}
			else if (this.DyedBrown)
			{
				this.Subtitle.Speaker = this;
				this.Subtitle.UpdateLabel(this.SplashSubtitleType, 7, 5f);
			}
			else if (this.Bloody)
			{
				this.Subtitle.Speaker = this;
				this.Subtitle.UpdateLabel(this.SplashSubtitleType, 3, 5f);
			}
			else
			{
				this.Subtitle.Speaker = this;
				this.Subtitle.UpdateLabel(this.SplashSubtitleType, 1, 5f);
			}
			this.CharacterAnimation[this.SplashedAnim].speed = 0.5f;
			this.SplashPhase++;
		}
		if (this.SplashTimer > 12f && this.SplashPhase == 2 && !this.StudentManager.KokonaTutorial)
		{
			if (this.LightSwitch == null)
			{
				if (this.Gas)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(this.SplashSubtitleType, 6, 5f);
				}
				else if (this.Bloody)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(this.SplashSubtitleType, 4, 5f);
				}
				else
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(this.SplashSubtitleType, 2, 5f);
				}
				this.SplashPhase++;
				if (!this.Male)
				{
					this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
					this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
				}
				else
				{
					this.CurrentDestination = this.StudentManager.MaleStripSpot;
					this.Pathfinding.target = this.StudentManager.MaleStripSpot;
				}
			}
			else if (!this.LightSwitch.BathroomLight.activeInHierarchy)
			{
				if (this.LightSwitch.Panel.useGravity)
				{
					this.LightSwitch.Prompt.Hide();
					this.LightSwitch.Prompt.enabled = false;
					this.Prompt.Hide();
					this.Prompt.enabled = false;
				}
				this.Subtitle.UpdateLabel(SubtitleType.LightSwitchReaction, 1, 5f);
				this.CurrentDestination = this.LightSwitch.ElectrocutionSpot;
				this.Pathfinding.target = this.LightSwitch.ElectrocutionSpot;
				this.Pathfinding.speed = this.WalkSpeed;
				this.BathePhase = -1;
				this.InDarkness = true;
			}
			else
			{
				if (!this.Bloody)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(this.SplashSubtitleType, 2, 5f);
				}
				else
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(this.SplashSubtitleType, 4, 5f);
				}
				this.SplashPhase++;
				this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
				this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
			}
			Debug.Log(this.Name + " is now running towards the locker room to wash up.");
			this.CharacterAnimation[this.WetAnim].weight = 1f;
			this.Pathfinding.canSearch = true;
			this.Pathfinding.canMove = true;
			this.Splashed = false;
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x001F1688 File Offset: 0x001EF888
	private void UpdateTurningOffRadio()
	{
		if (this.Radio.On || (this.RadioPhase == 3 && this.Radio.transform.parent == null))
		{
			if (this.RadioPhase == 1)
			{
				Debug.Log("Phase 1 of turning off radio.");
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.Radio.transform.position.x, base.transform.position.y, this.Radio.transform.position.z) - base.transform.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				this.RadioTimer += Time.deltaTime;
				if (this.RadioTimer > 3f)
				{
					if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
					{
						this.SmartPhone.SetActive(true);
					}
					if (this.Persona == PersonaType.Protective || this.Hurry)
					{
						Debug.Log("Sprinting because Protective.");
						this.CharacterAnimation.CrossFade(this.RunAnim);
						this.Pathfinding.speed = 4f;
					}
					else
					{
						this.CharacterAnimation.CrossFade(this.WalkAnim);
					}
					this.CurrentDestination = this.Radio.transform;
					this.Pathfinding.target = this.Radio.transform;
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.RadioTimer = 0f;
					this.RadioPhase++;
				}
			}
			else if (this.RadioPhase == 2)
			{
				Debug.Log("Phase 2 of turning off radio.");
				if (Vector3.Distance(base.transform.position, new Vector3(this.Radio.transform.position.x, base.transform.position.y, this.Radio.transform.position.z)) < 0.5f)
				{
					this.CharacterAnimation.CrossFade(this.RadioAnim);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.SmartPhone.SetActive(false);
					this.RadioTimer = 0f;
					this.RadioPhase++;
				}
				else if (this.Persona == PersonaType.Protective || this.Hurry)
				{
					Debug.Log("Sprinting because Protective.");
					this.CharacterAnimation.CrossFade(this.RunAnim);
					this.Pathfinding.speed = 4f;
				}
				else
				{
					this.CharacterAnimation.CrossFade(this.WalkAnim);
				}
			}
			else if (this.RadioPhase == 3)
			{
				Debug.Log("Phase 3 of turning off radio.");
				this.targetRotation = Quaternion.LookRotation(new Vector3(this.Radio.transform.position.x, base.transform.position.y, this.Radio.transform.position.z) - base.transform.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, 10f * Time.deltaTime);
				this.RadioTimer += Time.deltaTime;
				if (this.RadioTimer > 4f)
				{
					if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
					{
						this.SmartPhone.SetActive(true);
					}
					this.CurrentDestination = this.Destinations[this.Phase];
					this.Pathfinding.target = this.Destinations[this.Phase];
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.ForgetRadio();
				}
				else if (this.RadioTimer > 2f)
				{
					this.Radio.Victim = null;
					this.Radio.TurnOff();
				}
			}
			if (this.StudentID == 9 && this.Clock.GameplayDay == 5 && base.transform.position.x > 17f && base.transform.position.x < 23f && base.transform.position.z > 50f && base.transform.position.z < 54f)
			{
				this.BountyCollider.SetActive(true);
				return;
			}
		}
		else
		{
			if (this.RadioPhase < 100)
			{
				this.CharacterAnimation.CrossFade(this.IdleAnim);
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
				this.RadioPhase = 100;
				this.RadioTimer = 0f;
			}
			this.targetRotation = Quaternion.LookRotation(new Vector3(this.Radio.transform.position.x, base.transform.position.y, this.Radio.transform.position.z) - base.transform.position);
			this.RadioTimer += Time.deltaTime;
			if (this.RadioTimer > 1f || this.Radio.transform.parent != null)
			{
				this.CurrentDestination = this.Destinations[this.Phase];
				this.Pathfinding.target = this.Destinations[this.Phase];
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				this.ForgetRadio();
			}
		}
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x001F1C64 File Offset: 0x001EFE64
	private void UpdateVomiting()
	{
		if (this.VomitPhase != 0 && this.VomitPhase != 4)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
			this.MoveTowardsTarget(this.CurrentDestination.position);
		}
		if (this.VomitPhase == 0)
		{
			if (this.DistanceToDestination < 0.5f)
			{
				if (!this.Yandere.Armed && this.Yandere.PickUp == null)
				{
					this.Drownable = true;
					this.StudentManager.UpdateMe(this.StudentID);
				}
				if (this.VomitDoor != null)
				{
					this.VomitDoor.Prompt.enabled = false;
					this.VomitDoor.Prompt.Hide();
					this.VomitDoor.enabled = false;
				}
				this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
				this.CharacterAnimation.CrossFade(this.VomitAnim);
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
				this.VomitPhase++;
				return;
			}
		}
		else if (this.VomitPhase == 1)
		{
			if (this.CharacterAnimation[this.VomitAnim].time > 1f)
			{
				this.VomitEmitter.gameObject.SetActive(true);
				this.VomitPhase++;
				return;
			}
		}
		else if (this.VomitPhase == 2)
		{
			if (this.StudentManager.KokonaTutorial)
			{
				if (this.CharacterAnimation[this.VomitAnim].time > 13f)
				{
					this.CharacterAnimation[this.VomitAnim].speed = -1f;
					return;
				}
				if (this.CharacterAnimation[this.VomitAnim].time < 1f)
				{
					this.CharacterAnimation[this.VomitAnim].speed = 1f;
					return;
				}
			}
			else if (this.CharacterAnimation[this.VomitAnim].time > 13f)
			{
				this.VomitEmitter.gameObject.SetActive(false);
				this.VomitPhase++;
				return;
			}
		}
		else if (this.VomitPhase == 3)
		{
			if (this.CharacterAnimation[this.VomitAnim].time >= this.CharacterAnimation[this.VomitAnim].length)
			{
				this.Drownable = false;
				this.StudentManager.UpdateMe(this.StudentID);
				this.WalkAnim = this.OriginalWalkAnim;
				this.CharacterAnimation.CrossFade(this.WalkAnim);
				if (this.Male)
				{
					this.StudentManager.GetMaleWashSpot(this);
					this.Pathfinding.target = this.StudentManager.MaleWashSpot;
					this.CurrentDestination = this.StudentManager.MaleWashSpot;
				}
				else
				{
					this.StudentManager.GetFemaleWashSpot(this);
					this.Pathfinding.target = this.StudentManager.FemaleWashSpot;
					this.CurrentDestination = this.StudentManager.FemaleWashSpot;
				}
				if (this.VomitDoor != null)
				{
					this.VomitDoor.Prompt.enabled = true;
					this.VomitDoor.enabled = true;
				}
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				this.Pathfinding.speed = this.WalkSpeed;
				this.DistanceToDestination = 100f;
				this.VomitPhase++;
				return;
			}
		}
		else if (this.VomitPhase == 4)
		{
			if (this.DistanceToDestination < 0.5f)
			{
				this.CharacterAnimation.CrossFade(this.WashFaceAnim);
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
				this.VomitPhase++;
				return;
			}
		}
		else if (this.VomitPhase == 5 && this.CharacterAnimation[this.WashFaceAnim].time > this.CharacterAnimation[this.WashFaceAnim].length)
		{
			this.StopVomitting();
			Debug.Log(this.Name + " just finished vomiting.");
			Debug.Log(this.Name + "'s current Phase is: " + this.Phase.ToString());
			Debug.Log(this.Name + "'s ScheduleBlocks.Length is: " + this.ScheduleBlocks.Length.ToString());
			if (this.Phase < this.ScheduleBlocks.Length && this.Phase < this.ScheduleBlocks.Length - 1)
			{
				ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase + 1];
				scheduleBlock.destination = "Seat";
				scheduleBlock.action = "Sit";
			}
			this.GetDestinations();
			this.Phase++;
			this.Pathfinding.target = this.Destinations[this.Phase];
			this.CurrentDestination = this.Destinations[this.Phase];
			this.CurrentAction = StudentActionType.SitAndTakeNotes;
			this.DistanceToDestination = 100f;
		}
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x001F2184 File Offset: 0x001F0384
	private void StopVomitting()
	{
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		this.VomitEmitter.gameObject.SetActive(false);
		this.Prompt.Label[0].text = "     Talk";
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.Distracted = false;
		this.Drownable = false;
		this.Vomiting = false;
		this.Private = false;
		this.CanTalk = true;
		this.Routine = true;
		this.Emetic = false;
		this.VomitPhase = 0;
		this.StudentManager.UpdateMe(this.StudentID);
		this.WalkAnim = this.OriginalWalkAnim;
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x001F2234 File Offset: 0x001F0434
	private void UpdateConfessing()
	{
		if (this.StudentID > 1 && this.StudentID != this.StudentManager.SuitorID)
		{
			if (this.ConfessPhase == 1)
			{
				if (this.DistanceToDestination < 0.5f)
				{
					this.StudentManager.ChangeSuitorRoutineOnFriday();
					this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
					if (this.Male)
					{
						this.CharacterAnimation.CrossFade("insertNote_01");
						this.Cosmetic.MyRenderer.materials[this.Cosmetic.FaceID].SetFloat("_BlendAmount", 1f);
					}
					else
					{
						this.CharacterAnimation.CrossFade("f02_insertNote_00");
						this.Cosmetic.MyRenderer.materials[2].SetFloat("_BlendAmount", 1f);
					}
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.Note.SetActive(true);
					this.ConfessPhase++;
					return;
				}
				if (this.Pathfinding.speed == 1f)
				{
					this.CharacterAnimation.CrossFade(this.WalkAnim);
				}
				else
				{
					this.CharacterAnimation.CrossFade(this.SprintAnim);
				}
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				return;
			}
			else if (this.ConfessPhase == 2)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
				this.MoveTowardsTarget(this.CurrentDestination.position);
				string name;
				if (this.Male)
				{
					name = "insertNote_01";
				}
				else
				{
					name = "f02_insertNote_00";
				}
				if (this.CharacterAnimation[name].time >= 9f)
				{
					this.Note.SetActive(false);
					this.ConfessPhase++;
					return;
				}
			}
			else if (this.ConfessPhase == 3)
			{
				string name2;
				if (this.Male)
				{
					name2 = "insertNote_01";
				}
				else
				{
					name2 = "f02_insertNote_00";
				}
				if (this.CharacterAnimation[name2].time >= this.CharacterAnimation[name2].length)
				{
					this.CurrentDestination = this.StudentManager.RivalConfessionSpot;
					this.Pathfinding.target = this.StudentManager.RivalConfessionSpot;
					this.Pathfinding.canSearch = true;
					this.Pathfinding.canMove = true;
					this.Pathfinding.speed = 4f;
					this.StudentManager.LoveManager.LeftNote = true;
					this.CharacterAnimation.CrossFade(this.SprintAnim);
					this.ConfessPhase++;
					return;
				}
			}
			else if (this.ConfessPhase == 4)
			{
				if (this.DistanceToDestination < 0.5f)
				{
					if (this.StudentManager.Eighties && this.StudentID == 19)
					{
						this.StudentManager.FollowPrimaryLookSecondary.gameObject.SetActive(true);
					}
					this.CharacterAnimation.CrossFade(this.IdleAnim);
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.ConfessPhase++;
					return;
				}
			}
			else if (this.ConfessPhase == 5)
			{
				if (this.DistanceToPlayer < 5f && (this.Yandere.Attacking || this.Yandere.NearBodies > 0))
				{
					this.Distracted = false;
				}
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
				if (!this.Male && !this.Alarmed)
				{
					this.CharacterAnimation[this.ShyAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.ShyAnim].weight, 1f, Time.deltaTime);
				}
				this.MoveTowardsTarget(this.CurrentDestination.position);
				return;
			}
		}
		else if (this.ConfessPhase == 1)
		{
			string name3;
			if (this.Male)
			{
				name3 = "keepNote_00";
			}
			else
			{
				name3 = "f02_keepNote_00";
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
			this.MoveTowardsTarget(this.CurrentDestination.position);
			if (this.CharacterAnimation[name3].time > 14f)
			{
				this.Note.SetActive(false);
			}
			else if ((double)this.CharacterAnimation[name3].time > 4.5)
			{
				this.Note.SetActive(true);
			}
			if (this.CharacterAnimation[name3].time >= this.CharacterAnimation[name3].length)
			{
				Debug.Log("Sprinting to confession tree.");
				this.CurrentDestination = this.StudentManager.SuitorConfessionSpot;
				this.Pathfinding.target = this.StudentManager.SuitorConfessionSpot;
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				this.Pathfinding.speed = 4f;
				this.Note.SetActive(false);
				this.CharacterAnimation.CrossFade(this.SprintAnim);
				this.ConfessPhase++;
				return;
			}
		}
		else if (this.ConfessPhase == 2)
		{
			if (this.DistanceToDestination < 0.5f)
			{
				if (this.Male)
				{
					this.CharacterAnimation.CrossFade("exhausted_01");
				}
				else
				{
					this.CharacterAnimation.CrossFade("f02_nervousLeftRight_00");
				}
				this.Pathfinding.canSearch = false;
				this.Pathfinding.canMove = false;
				this.ConfessPhase++;
				return;
			}
		}
		else if (this.ConfessPhase == 3)
		{
			if (this.Male)
			{
				this.CharacterAnimation.CrossFade("exhausted_01");
			}
			else
			{
				this.CharacterAnimation.CrossFade("f02_nervousLeftRight_00");
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.CurrentDestination.rotation, Time.deltaTime * 10f);
			this.MoveTowardsTarget(this.CurrentDestination.position);
		}
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x001F2888 File Offset: 0x001F0A88
	private void UpdateMisc()
	{
		if (this.IgnoreTimer > 0f)
		{
			this.IgnoreTimer = Mathf.MoveTowards(this.IgnoreTimer, 0f, Time.deltaTime);
		}
		if (!this.Fleeing)
		{
			if (base.transform.position.z < -100f)
			{
				if (base.transform.position.y < -11f && this.StudentID > 1 && this.Phase > 1)
				{
					base.gameObject.SetActive(false);
					return;
				}
			}
			else
			{
				if (base.transform.position.y < -0.1f)
				{
					base.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				}
				if (!this.Dying && !this.Distracted && !this.WalkBack && !this.Waiting && !this.Guarding && !this.InEvent && !this.WitnessedMurder && !this.WitnessedCorpse && !this.Blind && !this.SentHome && !this.SentToLocker && !this.TurnOffRadio && !this.Wet && !this.InvestigatingBloodPool && !this.ReturningMisplacedWeapon && !this.Yandere.Egg && !this.StudentManager.Pose && !this.ShoeRemoval.enabled && !this.Drownable && !this.Meeting && !this.EatingSnack)
				{
					if (this.StudentManager.MissionMode && (double)this.DistanceToPlayer < 0.5)
					{
						this.Yandere.Shutter.FaceStudent = this;
						this.Yandere.Shutter.Penalize();
					}
					if (((this.Club == ClubType.Council && !this.DoNotShove) || (this.Shovey && !this.Following)) && !this.WitnessedSomething)
					{
						if (this.DistanceToPlayer < 5f)
						{
							if (this.DistanceToPlayer < 2f)
							{
								this.StudentManager.TutorialWindow.ShowCouncilMessage = true;
							}
							if (Mathf.Abs(Vector3.Angle(-base.transform.forward, this.Yandere.transform.position - base.transform.position)) <= 45f && this.Yandere.Stance.Current != StanceType.Crouching && this.Yandere.Stance.Current != StanceType.Crawling && this.Yandere.CanMove && !this.Yandere.Invisible && (this.Yandere.h != 0f || this.Yandere.v != 0f) && (this.Yandere.Running || this.DistanceToPlayer < 2f))
							{
								if (this.Investigating)
								{
									this.StopInvestigating();
								}
								Debug.Log(this.Name + " was alarmed because Yandere-chan was moving nearby.");
								this.DistractionSpot = this.Yandere.transform.position;
								this.Alarm = 200f;
								this.TemporarilyBlind = true;
								this.FocusOnYandere = true;
								this.Routine = false;
								this.Pathfinding.canSearch = false;
								this.Pathfinding.canMove = false;
							}
						}
						if (this.DistanceToPlayer < 1.1f && !this.Yandere.Invisible && !this.EatingSnack && Mathf.Abs(Vector3.Angle(-base.transform.forward, this.Yandere.transform.position - base.transform.position)) > 45f && (this.Yandere.Armed || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed)) && this.Prompt.InSight)
						{
							if (this.Yandere.Armed && !this.Yandere.EquippedWeapon.Suspicious && !this.WitnessedMurder && !this.Yandere.Chased && this.Yandere.Chasers == 0)
							{
								Debug.Log(this.Name + " is shoving the player from this place in the code. 2");
								this.Shove();
							}
							else
							{
								Debug.Log("Calling ''Spray()'' from this part of the code. 4");
								this.Spray();
							}
						}
					}
					if (((this.Club == ClubType.Council && !this.Spraying && !this.DoNotShove) || (this.Club == ClubType.Delinquent && !this.Injured && !this.RespectEarned && !this.Vomiting && !this.Emetic && !this.Headache && !this.Sedated && !this.Lethal) || (this.Shovey && !this.Spraying && !this.Following && !this.Meeting && !this.EatingSnack)) && (double)this.DistanceToPlayer < 0.5 && this.Yandere.CanMove && !this.Yandere.Invisible && (this.Yandere.h != 0f || this.Yandere.v != 0f) && !this.Yandere.Chased && this.Yandere.Chasers == 0 && this.Yandere.NoShoveTimer == 0f)
					{
						if (this.Club == ClubType.Delinquent)
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.UpdateLabel(SubtitleType.DelinquentShove, 0, 3f);
						}
						Debug.Log(this.Name + " is shoving the player from this place in the code. 3");
						this.Shove();
						return;
					}
				}
			}
		}
		else if (((this.Club == ClubType.Council && !this.DoNotShove) || this.Shovey) && this.DistanceToPlayer < 1.1f && !this.Yandere.Invisible && !this.EatingSnack && Mathf.Abs(Vector3.Angle(-base.transform.forward, this.Yandere.transform.position - base.transform.position)) > 45f && ((!this.IgnoringPettyActions && this.Yandere.Armed && this.Yandere.EquippedWeapon.Suspicious) || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed)) && this.Prompt.InSight)
		{
			Debug.Log(this.Name + " will now decide whether to spray or shove the protagonist.");
			if (this.Yandere.Armed && !this.Yandere.EquippedWeapon.Suspicious && !this.WitnessedMurder && !this.Yandere.Chased && this.Yandere.Chasers == 0)
			{
				Debug.Log(this.Name + " is shoving the player from this place in the code. 4");
				this.Shove();
				return;
			}
			Debug.Log("Calling ''Spray()'' from this part of the code. 5");
			this.Spray();
		}
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x001F3070 File Offset: 0x001F1270
	private void LateUpdate()
	{
		if (this.StudentManager.DisableFarAnims && this.DistanceToPlayer >= (float)this.StudentManager.FarAnimThreshold && this.CharacterAnimation.cullingType != AnimationCullingType.AlwaysAnimate && !this.WitnessCamera.Show && !this.ClubActivity)
		{
			this.CharacterAnimation.enabled = false;
		}
		else
		{
			this.CharacterAnimation.enabled = true;
		}
		if (this.EyeShrink > 0f)
		{
			if (this.EyeShrink > 1f)
			{
				this.EyeShrink = 1f;
			}
			this.LeftEye.localPosition = new Vector3(this.LeftEye.localPosition.x, this.LeftEye.localPosition.y, this.LeftEyeOrigin.z - this.EyeShrink * 0.01f);
			this.RightEye.localPosition = new Vector3(this.RightEye.localPosition.x, this.RightEye.localPosition.y, this.RightEyeOrigin.z + this.EyeShrink * 0.01f);
			this.LeftEye.localScale = new Vector3(1f - this.EyeShrink * 0.5f, 1f - this.EyeShrink * 0.5f, this.LeftEye.localScale.z);
			this.RightEye.localScale = new Vector3(1f - this.EyeShrink * 0.5f, 1f - this.EyeShrink * 0.5f, this.RightEye.localScale.z);
			this.PreviousEyeShrink = this.EyeShrink;
		}
		if (!this.Male)
		{
			if (this.Shy)
			{
				if (this.Routine)
				{
					if ((this.Phase == 2 && this.DistanceToDestination < 1f) || (this.Phase == 4 && this.DistanceToDestination < 1f) || (this.Actions[this.Phase] == StudentActionType.SitAndTakeNotes && this.DistanceToDestination < 1f) || (this.Actions[this.Phase] == StudentActionType.Clean && this.DistanceToDestination < 1f) || (this.Actions[this.Phase] == StudentActionType.Read && this.DistanceToDestination < 1f))
					{
						this.CharacterAnimation[this.ShyAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.ShyAnim].weight, 0f, Time.deltaTime);
					}
					else
					{
						this.CharacterAnimation[this.ShyAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.ShyAnim].weight, 1f, Time.deltaTime);
					}
				}
				else if (!this.Headache)
				{
					this.CharacterAnimation[this.ShyAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.ShyAnim].weight, 0f, Time.deltaTime);
				}
			}
			if (this.BreastSize != 1f && (!this.BoobsResized || this.RightBreast.localScale.x != this.BreastSize))
			{
				this.RightBreast.localScale = new Vector3(this.BreastSize, this.BreastSize, this.BreastSize);
				this.LeftBreast.localScale = new Vector3(this.BreastSize, this.BreastSize, this.BreastSize);
				if (!this.Cosmetic.CustomEyes)
				{
					this.RightBreast.gameObject.name = "RightBreastRENAMED";
					this.LeftBreast.gameObject.name = "LeftBreastRENAMED";
				}
				this.BoobsResized = true;
			}
			if (this.StudentManager.Egg && this.SecurityCamera.activeInHierarchy)
			{
				this.Head.localScale = new Vector3(0f, 0f, 0f);
			}
			if (!this.Slave && this.Club == ClubType.Bully)
			{
				for (int i = 0; i < 4; i++)
				{
					if (this.Skirt[i] != null)
					{
						Transform transform = this.Skirt[i].transform;
						transform.localScale = new Vector3(transform.localScale.x, 0.6666667f, transform.localScale.z);
					}
				}
			}
			if (this.Cosmetic.HideEyebrows)
			{
				this.Cosmetic.RightTemple.localScale = new Vector3(0f, 1f, 1f);
				this.Cosmetic.LeftTemple.localScale = new Vector3(0f, 1f, 1f);
			}
		}
		if (this.Club == ClubType.Photography || this.ClubActivity || this.Actions[this.Phase] == StudentActionType.Meeting)
		{
			if (this.Routine && !this.InEvent && !this.Meeting && !this.GoAway)
			{
				if (!this.StudentManager.Eighties)
				{
					if ((this.DistanceToDestination < this.TargetDistance && this.Actions[this.Phase] == StudentActionType.SitAndSocialize) || (this.DistanceToDestination < this.TargetDistance && this.StudentID != 36 && this.Actions[this.Phase] == StudentActionType.Meeting) || (this.DistanceToDestination < this.TargetDistance && this.Actions[this.Phase] == StudentActionType.Sleuth && this.StudentManager.SleuthPhase != 2 && this.StudentManager.SleuthPhase != 3) || (this.Club == ClubType.Photography && this.ClubActivity))
					{
						this.BlendIntoSittingAnim();
					}
					else
					{
						this.BlendOutOfSittingAnim();
					}
				}
				else if (this.DistanceToDestination < this.TargetDistance && this.StudentID != 36 && this.CurrentAction == StudentActionType.Meeting)
				{
					this.BlendIntoSittingAnim();
				}
				else
				{
					this.BlendOutOfSittingAnim();
				}
			}
			else
			{
				this.BlendOutOfSittingAnim();
			}
		}
		if (this.DK)
		{
			this.Arm[0].localScale = new Vector3(2f, 2f, 2f);
			this.Arm[1].localScale = new Vector3(2f, 2f, 2f);
			this.Head.localScale = new Vector3(2f, 2f, 2f);
		}
		if (this.Fate > 0 && this.Clock.HourTime > this.TimeOfDeath)
		{
			this.Yandere.TargetStudent = this;
			this.StudentManager.Shinigami.Effect = this.Fate - 1;
			this.StudentManager.Shinigami.Attack();
			this.Yandere.TargetStudent = null;
			this.Fate = 0;
		}
		if (this.Yandere.BlackHole && this.DistanceToPlayer < 2.5f)
		{
			if (this.DeathScream != null)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.DeathScream, base.transform.position + Vector3.up, Quaternion.identity);
			}
			this.BlackHoleEffect[0].enabled = true;
			this.BlackHoleEffect[1].enabled = true;
			this.BlackHoleEffect[2].enabled = true;
			this.CharacterAnimation[this.WetAnim].weight = 0f;
			this.DeathType = DeathType.EasterEgg;
			this.CharacterAnimation.Stop();
			this.Suck.enabled = true;
			this.BecomeRagdoll();
			this.Dying = true;
		}
		if (this.CameraReacting && (this.StudentManager.NEStairs.bounds.Contains(base.transform.position) || this.StudentManager.NWStairs.bounds.Contains(base.transform.position) || this.StudentManager.SEStairs.bounds.Contains(base.transform.position) || this.StudentManager.SWStairs.bounds.Contains(base.transform.position) || this.StudentManager.PoolStairs.bounds.Contains(base.transform.position)) && this.Spine != null)
		{
			this.Spine.LookAt(this.Yandere.Spine[0]);
			this.Head.LookAt(this.Yandere.Head);
		}
		if (this.DistanceToPlayer < 0.5f && !this.CameraReacting && !this.Struggling && !this.Yandere.Attacking && !this.Distracted && !this.Posing && this.CanSeeObject(this.Yandere.gameObject, this.Yandere.HeadPosition) && this.Yandere.CanMove && this.CurrentDestination != this.Yandere.transform)
		{
			this.PersonalSpaceTimer += Time.deltaTime;
			if (this.PersonalSpaceTimer > 4f)
			{
				Debug.Log(this.Name + " feels that the protagonist is invading their personal space.");
				if (this.WarningsGiven > 4)
				{
					this.Yandere.Shutter.FaceStudent = this;
					this.Yandere.Shutter.Penalize();
				}
				else
				{
					this.Subtitle.UpdateLabel(SubtitleType.HmmReaction, 0, 0f);
					this.Subtitle.UpdateLabel(SubtitleType.ProximityWarning, this.WarningsGiven, 5f);
					this.PersonalSpaceTimer = 0f;
				}
				this.WarningsGiven++;
			}
		}
		if (this.CurrentDestination == null)
		{
			if (this.Phase > 0)
			{
				Debug.Log(this.Name + "'s CurrentDestination became ''null'' for some reason.");
				Debug.Log(this.Name + "'s current Phase is:" + this.Phase.ToString());
			}
			if (this.Destinations[this.Phase] == null)
			{
				if (this.Phase < this.Destinations.Length - 1)
				{
					Debug.Log(this.Name + " is advancing their Phase forward.");
					this.Phase++;
				}
				else
				{
					this.Phase = this.Destinations.Length - 1;
					Debug.Log(this.Name + " is in the final Phase of their routine...");
				}
			}
			this.CurrentDestination = this.Destinations[this.Phase];
			this.Pathfinding.target = this.CurrentDestination;
			if (this.Leaving)
			{
				this.CurrentDestination = this.StudentManager.Exit;
				this.Pathfinding.target = this.StudentManager.Exit;
			}
			if (this.Phase > 0)
			{
				string[] array = new string[6];
				array[0] = "Phase is ";
				array[1] = this.Phase.ToString();
				array[2] = ". Changing ";
				array[3] = this.Name;
				array[4] = "'s CurrentDestination to: ";
				int num = 5;
				Transform transform2 = this.Destinations[this.Phase];
				array[num] = ((transform2 != null) ? transform2.ToString() : null);
				Debug.Log(string.Concat(array));
			}
		}
		if (this.Shoving && this.CharacterAnimation[this.ShoveAnim].time < this.CharacterAnimation[this.ShoveAnim].length * 0.5f)
		{
			float num2 = 1f - this.CharacterAnimation[this.ShoveAnim].time / (this.CharacterAnimation[this.ShoveAnim].length * 0.5f);
			this.MyController.Move(base.transform.forward * (Time.deltaTime * -1f * num2));
		}
		if (base.transform.position.x > 42f && base.transform.position.z > 58f)
		{
			if (base.transform.position.y > 4f)
			{
				this.Obstacle.enabled = false;
				return;
			}
			this.Obstacle.enabled = true;
		}
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x001F3CBC File Offset: 0x001F1EBC
	public void CalculateReputationPenalty()
	{
		Debug.Log("Calculating reputation penalty now.");
		if ((this.Male && this.Yandere.Class.Seduction + this.Yandere.Class.SeductionBonus > 2) || this.Yandere.Class.Seduction + this.Yandere.Class.SeductionBonus > 4)
		{
			this.RepDeduction += this.RepLoss * 0.2f;
		}
		if (PlayerGlobals.Reputation < -33.33333f)
		{
			Debug.Log("Rep is low. Rep loss should be harsher.");
			this.RepDeduction -= this.RepLoss * 0.2f;
		}
		if (PlayerGlobals.Reputation > 33.33333f)
		{
			Debug.Log("Rep is high. Rep loss should be lower.");
			this.RepDeduction += this.RepLoss * 0.2f;
		}
		if (this.Friend)
		{
			this.RepDeduction += this.RepLoss * 0.2f;
		}
		if (PlayerGlobals.PantiesEquipped == 1)
		{
			Debug.Log("Wearing the less-rep-loss panties.");
			this.RepDeduction += this.RepLoss * 0.2f;
		}
		if (this.Yandere.Class.SocialBonus > 0)
		{
			this.RepDeduction += this.RepLoss * 0.2f;
		}
		this.ChameleonCheck();
		if (this.Chameleon)
		{
			Debug.Log("Chopping reputation loss in half!");
			this.RepLoss *= 0.5f;
		}
		if (this.Yandere.Persona == YanderePersonaType.Aggressive)
		{
			this.RepLoss *= 2f;
		}
		if (this.Club == ClubType.Bully)
		{
			this.RepLoss *= 2f;
		}
		if (this.Witnessed == StudentWitnessType.CleaningItem)
		{
			this.RepLoss *= 0.5f;
		}
		if (this.Club == ClubType.Delinquent)
		{
			this.RepDeduction = 0f;
			this.RepLoss = 0f;
		}
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x001F3EB8 File Offset: 0x001F20B8
	public void MoveTowardsTarget(Vector3 target)
	{
		if (Time.timeScale > 0.0001f && this.MyController.enabled)
		{
			Vector3 a = target - base.transform.position;
			if (a.sqrMagnitude > 1E-06f)
			{
				this.MyController.Move(a * (Time.deltaTime * 5f / Time.timeScale));
			}
		}
	}

	// Token: 0x06002179 RID: 8569 RVA: 0x001F3F21 File Offset: 0x001F2121
	private void LookTowardsTarget(Vector3 target)
	{
		float timeScale = Time.timeScale;
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x001F3F30 File Offset: 0x001F2130
	public void AttackReaction()
	{
		Debug.Log(this.Name + " is being attacked.");
		if (this.StudentID != 97 && this.StudentManager.Students[97] != null && !this.StudentManager.Students[97].Hunted && this.StudentManager.Students[97].transform.position.z < -47f && base.transform.position.z < -47f && base.transform.position.x > -14f && base.transform.position.x < 14f)
		{
			Debug.Log("This character's death met the criteria to alarm the gym teacher.");
			this.StudentManager.Students[97].FocusOnYandere = true;
			this.StudentManager.Students[97].AwareOfMurder = true;
			this.StudentManager.Students[97].VisionBonus = 100f;
			this.StudentManager.Students[97].Alarm = 200f;
		}
		this.FocusOnStudent = false;
		this.FocusOnYandere = false;
		this.Blind = true;
		if (this.ReportingMurderToSenpai)
		{
			this.StudentManager.Students[this.LovestruckTarget].Blind = false;
		}
		if (this.SeekingMedicine)
		{
			Debug.Log("Student was seeking medicine at the time of death.");
			StudentScript studentScript = this.StudentManager.Students[90];
			if (studentScript != null && studentScript.RetreivingMedicine)
			{
				Debug.Log("Nurse was retrieving medicine at the time of death.");
				studentScript.RetrieveMedicinePhase = 5;
				studentScript.MedicineTimer = 4f;
				this.StudentManager.UpdateStudents(studentScript.StudentID);
			}
		}
		if (this.TurnOffRadio)
		{
			this.ForgetRadio();
		}
		if (this.SolvingPuzzle)
		{
			this.DropPuzzle();
		}
		this.BountyCollider.SetActive(false);
		if (this.PhotoEvidence)
		{
			this.SmartPhone.GetComponent<SmartphoneScript>().enabled = true;
			this.SmartPhone.GetComponent<PromptScript>().enabled = true;
			this.SmartPhone.GetComponent<Rigidbody>().useGravity = true;
			this.SmartPhone.GetComponent<Collider>().enabled = true;
			this.SmartPhone.transform.parent = null;
			this.SmartPhone.layer = 15;
		}
		else
		{
			this.SmartPhone.SetActive(false);
		}
		if (!this.WitnessedMurder)
		{
			float f = Vector3.Angle(-base.transform.forward, this.Yandere.transform.position - base.transform.position);
			this.Yandere.AttackManager.Stealth = (Mathf.Abs(f) <= 45f);
		}
		if (this.ReturningMisplacedWeapon)
		{
			Debug.Log(this.Name + " was in the process of returning a weapon when they were attacked.");
			this.DropMisplacedWeapon();
		}
		if (this.BloodPool != null)
		{
			Debug.Log(this.Name + "'s BloodPool was not null.");
			if (this.BloodPool.GetComponent<WeaponScript>() != null && this.BloodPool.GetComponent<WeaponScript>().Returner == this)
			{
				this.BloodPool.GetComponent<WeaponScript>().DoNotRelocate = true;
				this.BloodPool.GetComponent<WeaponScript>().Returner = null;
				this.BloodPool.GetComponent<WeaponScript>().Drop();
				this.BloodPool.GetComponent<WeaponScript>().enabled = true;
				this.BloodPool = null;
			}
		}
		if (!this.Male)
		{
			if (this.Following)
			{
				this.StudentManager.TranqDetector.TranqCheck();
			}
			this.CharacterAnimation["f02_smile_00"].weight = 0f;
			if (this.CharacterAnimation[this.ShyAnim] != null)
			{
				this.CharacterAnimation[this.ShyAnim].weight = 0f;
			}
			this.SmartPhone.SetActive(false);
			this.Shy = false;
		}
		if (this.WitnessCamera != null)
		{
			this.WitnessCamera.Show = false;
		}
		this.Pathfinding.canSearch = false;
		this.Pathfinding.canMove = false;
		this.Yandere.CharacterAnimation["f02_idleShort_00"].time = 0f;
		this.Yandere.CharacterAnimation["f02_swingA_00"].time = 0f;
		this.CharacterAnimation[this.WetAnim].weight = 0f;
		this.Yandere.HipCollider.enabled = true;
		this.Yandere.YandereVision = false;
		this.Yandere.Attacking = true;
		this.Yandere.CanMove = false;
		if (this.Yandere.Equipped > 0 && this.Yandere.EquippedWeapon.AnimID == 2)
		{
			this.Yandere.CharacterAnimation[this.Yandere.ArmedAnims[2]].weight = 0f;
		}
		if (this.DetectionMarker != null)
		{
			this.DetectionMarker.Tex.enabled = false;
		}
		this.EmptyHands();
		this.DropPlate();
		this.MyController.radius = 0f;
		if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
		{
			this.Countdown.gameObject.SetActive(false);
			this.ChaseCamera.SetActive(false);
			if (this.StudentManager.ChaseCamera == this.ChaseCamera)
			{
				this.StudentManager.ChaseCamera = null;
			}
		}
		this.VomitEmitter.gameObject.SetActive(false);
		this.SpecialRivalDeathReaction = false;
		this.InvestigatingBloodPool = false;
		this.SeekingMedicine = false;
		this.Investigating = false;
		this.Pen.SetActive(false);
		this.EatingSnack = false;
		this.SpeechLines.Stop();
		this.SentHome = false;
		this.Attacked = true;
		this.Alarmed = false;
		this.Fleeing = false;
		this.Routine = false;
		this.ReadPhase = 0;
		this.Dying = true;
		this.Wet = false;
		this.Prompt.Hide();
		this.Prompt.enabled = false;
		if (this.Following)
		{
			Debug.Log("Yandere-chan's follower is being set to ''null''.");
			this.Hearts.emission.enabled = false;
			this.FollowCountdown.gameObject.SetActive(false);
			this.Yandere.Follower = null;
			this.Yandere.Followers--;
			this.Following = false;
		}
		if (this.Distracting || (this.DistractionTarget != null && this.DistractionTarget.Distracted))
		{
			Debug.Log(string.Concat(new string[]
			{
				"At the time of being attacked, ",
				this.Name,
				" was distracting ",
				this.DistractionTarget.Name,
				"."
			}));
			this.DistractionTarget.TargetedForDistraction = false;
			this.DistractionTarget.Octodog.SetActive(false);
			this.DistractionTarget.Distracted = false;
			this.Distracting = false;
		}
		if (this.Teacher)
		{
			this.CheckForKnifeInInventory();
			bool flag = true;
			if (this.Strength == 9 && this.StudentManager.ChallengeManager.InvincibleRaibaru)
			{
				flag = false;
			}
			if ((flag && this.Yandere.Armed && this.Yandere.Class.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0 && this.Yandere.EquippedWeapon.Type == WeaponType.Knife) || (flag && this.Yandere.Club == ClubType.MartialArts && this.Yandere.Armed && this.Yandere.EquippedWeapon.Type == WeaponType.Knife))
			{
				Debug.Log(this.Name + " has called the ''BeginStruggle'' function.");
				this.Pathfinding.target = this.Yandere.transform;
				this.CurrentDestination = this.Yandere.transform;
				this.Yandere.Attacking = false;
				this.Attacked = false;
				this.Fleeing = true;
				this.Dying = false;
				this.Persona = PersonaType.Heroic;
				this.BeginStruggle();
			}
			else
			{
				Debug.Log(this.Name + " just countered Yandere-chan.");
				this.Yandere.HeartRate.gameObject.SetActive(false);
				this.Yandere.ShoulderCamera.Counter = true;
				this.Yandere.ShoulderCamera.OverShoulder = false;
				this.Yandere.RPGCamera.enabled = false;
				this.Yandere.Senpai = base.transform;
				this.Yandere.Attacking = true;
				this.Yandere.CanMove = false;
				this.Yandere.Talking = false;
				this.Yandere.Noticed = true;
				this.Yandere.HUD.alpha = 0f;
				this.Yandere.TargetStudent = this;
			}
		}
		else if (this.Strength == 9 && !this.Emetic && !this.Lethal && !this.Sedated && !this.Headache)
		{
			Debug.Log("Time to decide how Raibaru should react.");
			if (!this.StudentManager.ChallengeManager.InvincibleRaibaru && this.Yandere.PhysicalGrade + this.Yandere.Class.PhysicalBonus > 0)
			{
				Debug.Log("Player meets the criteria to have a physical struggle with Raibaru.");
				this.Pathfinding.target = this.Yandere.transform;
				this.CurrentDestination = this.Yandere.transform;
				this.Yandere.Attacking = false;
				this.FocusOnYandere = false;
				this.Attacked = false;
				this.Fleeing = true;
				this.Dying = false;
				this.Persona = PersonaType.Heroic;
				this.BeginStruggle();
			}
			else
			{
				Debug.Log("Player does not meet the criteria to have a physical struggle with Raibaru.");
				if (!this.WitnessedMurder)
				{
					this.Subtitle.UpdateLabel(SubtitleType.ObstacleMurderReaction, 3, 11f);
				}
				this.Yandere.CharacterAnimation.CrossFade("f02_moCounterA_00");
				this.Yandere.HeartRate.gameObject.SetActive(false);
				this.Yandere.ShoulderCamera.ObstacleCounter = true;
				this.Yandere.ShoulderCamera.OverShoulder = false;
				this.Yandere.RPGCamera.enabled = false;
				this.Yandere.Senpai = base.transform;
				this.Yandere.Attacking = true;
				this.Yandere.CanMove = false;
				this.Yandere.Talking = false;
				this.Yandere.Noticed = true;
				this.Yandere.HUD.alpha = 0f;
				this.CheckForNearbyWalls();
			}
		}
		else
		{
			this.Yandere.TargetStudent = this;
			if (this.Yandere.Armed)
			{
				if (this.Yandere.EquippedWeapon.Type == WeaponType.Garrote)
				{
					this.StudentManager.TranqDetector.GarroteAttack();
				}
				if (this.Yandere.SanityBased)
				{
					this.Yandere.AttackManager.Attack(this.Character, this.Yandere.EquippedWeapon);
				}
				if (!this.Yandere.AttackManager.Stealth)
				{
					this.Subtitle.UpdateLabel(SubtitleType.Dying, 0, 1f);
					this.SpawnAlarmDisc();
				}
			}
		}
		if (this.StudentManager.Reporter == this)
		{
			this.StudentManager.Reporter = null;
			if (this.ReportPhase == 0)
			{
				Debug.Log("A reporter died before being able to report a corpse. Corpse position reset.");
				this.StudentManager.CorpseLocation.position = Vector3.zero;
			}
		}
		if (this.StudentManager.BloodReporter == this)
		{
			Debug.Log(this.Name + " just set StudentManager.BloodReporter to ''null''.");
			this.StudentManager.BloodReporter = null;
		}
		if (this.Club == ClubType.Delinquent && this.MyWeapon != null && this.MyWeapon.transform.parent == this.ItemParent)
		{
			this.MyWeapon.transform.parent = null;
			this.MyWeapon.MyCollider.enabled = true;
			this.MyWeapon.Prompt.enabled = true;
			Rigidbody component = this.MyWeapon.GetComponent<Rigidbody>();
			component.constraints = RigidbodyConstraints.None;
			component.isKinematic = false;
			component.useGravity = true;
		}
		if (this.PhotoEvidence)
		{
			this.CameraFlash.SetActive(false);
			this.SmartPhone.SetActive(true);
		}
		if (this.BloodPool != null)
		{
			WeaponScript component2 = this.BloodPool.GetComponent<WeaponScript>();
			if (component2 != null && component2.Returner == this)
			{
				component2.Returner = null;
			}
		}
		if (this.StudentManager.AlternateTimeline)
		{
			SceneManager.LoadScene("FunGameOverScene");
		}
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x001F4C08 File Offset: 0x001F2E08
	public void DropPlate()
	{
		if (this.MyPlate != null)
		{
			if (this.MyPlate.parent == this.RightHand)
			{
				this.ClubActivityPhase = 0;
				this.MyPlate.GetComponent<Rigidbody>().isKinematic = false;
				this.MyPlate.GetComponent<Rigidbody>().useGravity = true;
				this.MyPlate.GetComponent<Collider>().enabled = true;
				this.MyPlate.parent = null;
				this.MyPlate.gameObject.SetActive(true);
			}
			if (this.Distracting)
			{
				this.DistractionTarget.TargetedForDistraction = false;
				this.Distracting = false;
				this.IdleAnim = this.OriginalIdleAnim;
				this.WalkAnim = this.OriginalWalkAnim;
			}
		}
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x001F4CC8 File Offset: 0x001F2EC8
	public void SenpaiNoticed()
	{
		Debug.Log("The ''SenpaiNoticed'' function has been called.");
		if (this.InvestigatingBloodPool)
		{
			Debug.Log("Was investigating something suspicious on the ground at the time.");
			if (this.BloodPool.GetComponent<WeaponScript>() != null)
			{
				this.Police.EndOfDay.Counselor.WeaponToReturn = this.BloodPool.GetComponent<WeaponScript>();
			}
			this.ForgetAboutBloodPool();
		}
		if (this.Yandere.Digging)
		{
			this.Yandere.StopDigging();
		}
		if (this.Yandere.Burying)
		{
			this.Yandere.StopBurying();
		}
		if (this.Yandere.Talking)
		{
			this.Yandere.StudentManager.DialogueWheel.End();
		}
		if (this.Yandere.Shutter.Snapping)
		{
			Debug.Log("THE SHUTTER WAS SNAPPING AT THE MOMENT THAT AYANO WAS NOTICED!");
			this.Yandere.Shutter.ResumeGameplay();
			this.Yandere.StopAiming();
			this.Yandere.Shutter.Snapping = false;
			this.Yandere.Shutter.Close = false;
			this.Yandere.Shutter.Timer = 0f;
		}
		this.Yandere.Stance.Current = StanceType.Standing;
		this.Yandere.CrawlTimer = 0f;
		this.Yandere.Uncrouch();
		this.Yandere.Noticed = true;
		if (this.WeaponToTakeAway != null)
		{
			Debug.Log("Yandere-chan was holding a weapon at the time she was caught.");
			if (this.Teacher && !this.Yandere.Attacking)
			{
				if (this.Yandere.EquippedWeapon.WeaponID == 23)
				{
					this.WeaponToTakeAway.transform.parent = null;
					this.WeaponToTakeAway.transform.position = this.WeaponToTakeAway.GetComponent<WeaponScript>().StartingPosition;
					this.WeaponToTakeAway.transform.eulerAngles = this.WeaponToTakeAway.GetComponent<WeaponScript>().StartingRotation;
					this.WeaponToTakeAway.GetComponent<WeaponScript>().Prompt.enabled = true;
					this.WeaponToTakeAway.GetComponent<WeaponScript>().enabled = true;
					this.WeaponToTakeAway.GetComponent<WeaponScript>().DoNotRelocate = true;
					this.WeaponToTakeAway.GetComponent<WeaponScript>().Drop();
					this.WeaponToTakeAway.GetComponent<WeaponScript>().MyRigidbody.useGravity = false;
					this.WeaponToTakeAway.GetComponent<WeaponScript>().MyRigidbody.isKinematic = true;
				}
				else
				{
					Debug.Log("That weapon was splattered with blood!");
					if (this.Yandere.EquippedWeapon.Bloody)
					{
						this.Police.WasHoldingBloodyWeapon = true;
					}
					this.Yandere.EquippedWeapon.Drop();
					this.WeaponToTakeAway.transform.position = this.StudentManager.WeaponBoxSpot.parent.position + new Vector3(0f, 1f, 0f);
					this.WeaponToTakeAway.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					this.WeaponToTakeAway.GetComponent<WeaponScript>().MyRigidbody.useGravity = true;
					this.WeaponToTakeAway.GetComponent<WeaponScript>().MyRigidbody.isKinematic = false;
					this.WeaponToTakeAway.GetComponent<BoxCollider>().isTrigger = false;
				}
				Debug.Log("The weapon has been taken away...");
			}
		}
		if (this.Yandere.StolenObjectID == 1)
		{
			Debug.Log("Yandere-chan was spotted stealing cigarettes.");
			this.Yandere.Inventory.Cigs = false;
		}
		this.WeaponToTakeAway = null;
		if (!this.Yandere.Attacking)
		{
			this.Yandere.EmptyHands();
		}
		this.Yandere.Senpai = base.transform;
		if (this.Yandere.Aiming || this.Yandere.Throwing || this.Yandere.PreparingThrow)
		{
			this.Yandere.StopAiming();
		}
		this.Yandere.PauseScreen.Hint.MyPanel.alpha = 0f;
		this.Yandere.DetectionPanel.alpha = 0f;
		this.Yandere.RPGCamera.mouseSpeed = 0f;
		this.Yandere.LaughIntensity = 0f;
		this.Yandere.HUD.alpha = 0f;
		this.Yandere.EyeShrink = 0f;
		this.Yandere.Sanity = 100f;
		this.Yandere.ProgressBar.transform.parent.gameObject.SetActive(false);
		this.Yandere.StudentManager.CombatMinigame.RedVignette.alpha = 0f;
		this.Yandere.HeartRate.gameObject.SetActive(false);
		this.Yandere.Stance.Current = StanceType.Standing;
		this.Yandere.MainCamera.transform.parent = null;
		this.Yandere.SneakShotPhone.SetActive(false);
		this.Yandere.ShoulderCamera.OverShoulder = false;
		this.Yandere.DelinquentFighting = false;
		this.Yandere.Shutter.Blocked = false;
		this.Yandere.CleaningWeapon = false;
		this.Yandere.FakingReaction = false;
		this.Yandere.BreakingGlass = false;
		this.Yandere.YandereVision = false;
		this.Yandere.CannotRecover = true;
		this.Yandere.SneakingShot = false;
		this.Yandere.Police.Show = false;
		this.Yandere.Cauterizing = false;
		this.Yandere.FollowHips = false;
		this.Yandere.Poisoning = false;
		this.Yandere.Rummaging = false;
		this.Yandere.Laughing = false;
		this.Yandere.CanMove = false;
		this.Yandere.Dipping = false;
		this.Yandere.Mopping = false;
		this.Yandere.Talking = false;
		this.Yandere.Hiding = false;
		this.Yandere.Lewd = false;
		this.Yandere.Jukebox.GameOver();
		this.StudentManager.GameOverIminent = true;
		this.StudentManager.StopMoving();
		if (this.Teacher || this.StudentID == 1)
		{
			if (this.Club != ClubType.Council)
			{
				this.IdleAnim = this.OriginalIdleAnim;
			}
			if (this.Giggle != null)
			{
				this.ForgetGiggle();
			}
			this.AlarmTimer = 0f;
			this.GoAway = false;
			base.enabled = true;
			this.Stop = false;
		}
		if (this.StudentID == 1)
		{
			this.StudentManager.FountainAudio[0].Stop();
			this.StudentManager.FountainAudio[1].Stop();
		}
		if (this.StudentManager.Eighties)
		{
			this.Yandere.LoseGentleEyes();
		}
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x001F5394 File Offset: 0x001F3594
	private void WitnessMurder()
	{
		Debug.Log(this.Name + " just realized that Yandere-chan is responsible for a murder!");
		if (this.Yandere.Mask == null)
		{
			Debug.Log("Alerts was incremented.");
			this.Yandere.Alerts++;
		}
		else
		{
			Debug.Log("Alerts was not incremented.");
		}
		if (this.Corpse == null)
		{
			this.Corpse = this.Yandere.CurrentRagdoll;
		}
		this.RespectEarned = false;
		if ((this.Fleeing && this.WitnessedBloodPool) || this.ReportPhase == 2)
		{
			this.WitnessedBloodyWeapon = false;
			this.WitnessedBloodPool = false;
			this.WitnessedSomething = false;
			this.WitnessedWeapon = false;
			this.WitnessedLimb = false;
			this.Fleeing = false;
			this.ReportPhase = 0;
		}
		this.CharacterAnimation[this.ScaredAnim].time = 0f;
		this.CameraFlash.SetActive(false);
		if (!this.Male)
		{
			this.CharacterAnimation["f02_smile_00"].weight = 0f;
		}
		this.WateringCan.SetActive(false);
		if (this.MyPlate != null && this.MyPlate.parent == this.RightHand)
		{
			this.ClubActivityPhase = 0;
			this.MyPlate.GetComponent<Rigidbody>().isKinematic = false;
			this.MyPlate.GetComponent<Rigidbody>().useGravity = true;
			this.MyPlate.GetComponent<Collider>().enabled = true;
			this.MyPlate.parent = null;
		}
		this.EmptyHands();
		this.MurdersWitnessed++;
		this.SpeechLines.Stop();
		this.WitnessedBloodyWeapon = false;
		this.WitnessedBloodPool = false;
		this.WitnessedSomething = false;
		this.WitnessedWeapon = false;
		this.WitnessedLimb = false;
		if (this.ReturningMisplacedWeapon)
		{
			this.DropMisplacedWeapon();
		}
		this.SpecialRivalDeathReaction = false;
		this.SenpaiWitnessingRivalDie = false;
		this.ReturningMisplacedWeapon = false;
		this.InvestigatingBloodPool = false;
		this.CameraReacting = false;
		this.FocusOnStudent = false;
		this.FocusOnYandere = false;
		this.TakingOutTrash = false;
		this.WitnessedMurder = true;
		this.Investigating = false;
		this.Distracting = false;
		this.EatingSnack = false;
		this.Threatened = false;
		this.Distracted = false;
		this.Reacted = false;
		this.Routine = false;
		this.Alarmed = true;
		this.NoTalk = false;
		this.Shy = false;
		this.Wet = false;
		if (this.OriginalPersona != PersonaType.Violent && this.Persona != this.OriginalPersona)
		{
			Debug.Log(this.Name + " is reverting back into their original Persona: " + this.OriginalPersona.ToString());
			this.Persona = this.OriginalPersona;
			this.SwitchBack = false;
			if (this.Persona == PersonaType.Heroic || this.Persona == PersonaType.Dangerous || this.Persona == PersonaType.Protective)
			{
				this.PersonaReaction();
			}
		}
		if (this.Persona == PersonaType.Spiteful && this.Yandere.TargetStudent != null)
		{
			Debug.Log("A Spiteful student witnessed a murder.");
			if ((this.Bullied && this.Yandere.TargetStudent.Club == ClubType.Bully) || this.Yandere.TargetStudent.Bullied)
			{
				Debug.Log("At this moment, a delinquent is reacting to the murder of a bully. 1");
				this.ScaredAnim = this.EvilWitnessAnim;
				this.Persona = PersonaType.Evil;
			}
		}
		if (this.Club == ClubType.Delinquent || this.Persona == PersonaType.Fragile)
		{
			Debug.Log("A Delinquent witnessed murderous behavior, and now has to determine if he saw a bully's murder.");
			if (this.Yandere.Attacking && this.Yandere.TargetStudent != null && this.Yandere.TargetStudent.Club == ClubType.Bully)
			{
				Debug.Log("At this moment, " + this.Name + " is reacting to the murder of a bully. 2");
				this.ScaredAnim = this.EvilWitnessAnim;
				this.Persona = PersonaType.Evil;
				this.FoundEnemyCorpse = true;
			}
			if ((this.Yandere.Lifting && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Dragging && !this.Yandere.CurrentRagdoll.Concealed))
			{
				if (this.Yandere.CurrentRagdoll.Student.Club == ClubType.Bully || this.Yandere.CurrentRagdoll.Student.OriginalClub == ClubType.Bully)
				{
					Debug.Log("At this moment, a delinquent is reacting to the murder of a bully. 3");
					this.ScaredAnim = this.EvilWitnessAnim;
					this.Persona = PersonaType.Evil;
				}
				else
				{
					Debug.Log("Ain't a bully.");
				}
			}
		}
		if (this.Persona == PersonaType.Sleuth)
		{
			Debug.Log("A Sleuth is witnessing a murder.");
			if (this.Yandere.Attacking || this.Yandere.Struggling || (this.Yandere.Carrying && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Lifting && !this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart))
			{
				if (!this.Sleuthing)
				{
					Debug.Log("A Sleuth is changing their Persona.");
					if (this.StudentID == 56)
					{
						this.Persona = PersonaType.Heroic;
					}
					else if (this.StudentManager.Eighties)
					{
						this.Persona = PersonaType.LandlineUser;
					}
					else
					{
						this.Persona = PersonaType.SocialButterfly;
					}
				}
				else if (this.StudentManager.Eighties)
				{
					this.Persona = PersonaType.LandlineUser;
				}
				else
				{
					this.Persona = PersonaType.SocialButterfly;
				}
			}
		}
		if (this.Persona == PersonaType.Heroic)
		{
			this.Yandere.Pursuer = this;
		}
		if (this.StudentID == 1 && this.Persona != PersonaType.Evil && this.Yandere.Mask == null)
		{
			if (!this.Yandere.Attacking && !this.Yandere.Struggling && !this.Yandere.Egg)
			{
				this.SenpaiNoticed();
			}
			this.Fleeing = false;
			this.EyeShrink = 0f;
			this.Yandere.Noticed = true;
			this.Yandere.Talking = false;
			this.CameraEffects.MurderWitnessed();
			this.Yandere.ShoulderCamera.OverShoulder = false;
			this.CharacterAnimation.CrossFade(this.ScaredAnim);
			if (this.Male)
			{
				this.CharacterAnimation["scaredFace_00"].weight = 1f;
			}
			Debug.Log("Senpai should be entereing his scared animation right now.");
		}
		else
		{
			this.SetOutlineColor(Color.red);
			this.SummonWitnessCamera();
			this.CameraEffects.MurderWitnessed();
			this.Witnessed = StudentWitnessType.Murder;
			if (this.Persona != PersonaType.Evil)
			{
				this.Police.Witnesses++;
			}
			if (this.Teacher)
			{
				this.StudentManager.Reporter = this;
			}
			if (this.Talking)
			{
				this.DialogueWheel.End();
				this.Hearts.emission.enabled = false;
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				this.Obstacle.enabled = false;
				this.Talk.enabled = false;
				this.Talking = false;
				this.Waiting = false;
				this.StudentManager.EnablePrompts();
			}
			if (this.Prompt.Label[0] != null && !this.StudentManager.EmptyDemon)
			{
				this.Prompt.Label[0].text = "     Talk";
				this.Prompt.HideButton[0] = true;
			}
		}
		if (this.Persona == PersonaType.TeachersPet && this.StudentManager.Reporter == null && !this.Police.Called)
		{
			this.StudentManager.CorpseLocation.position = this.Yandere.transform.position;
			this.StudentManager.LowerCorpsePosition();
			this.StudentManager.Reporter = this;
			this.ReportingMurder = true;
		}
		if (this.Following)
		{
			this.Hearts.emission.enabled = false;
			this.FollowCountdown.gameObject.SetActive(false);
			this.Yandere.Follower = null;
			this.Yandere.Followers--;
			this.Following = false;
		}
		this.Pathfinding.canSearch = false;
		this.Pathfinding.canMove = false;
		if (!this.Phoneless && (this.Persona == PersonaType.PhoneAddict || this.Sleuthing))
		{
			this.SmartPhone.SetActive(true);
		}
		if (this.SmartPhone.activeInHierarchy)
		{
			if (this.Persona != PersonaType.Heroic && this.Persona != PersonaType.Dangerous && this.Persona != PersonaType.Evil && this.Persona != PersonaType.Violent && this.Persona != PersonaType.Coward && !this.Teacher)
			{
				this.Persona = PersonaType.PhoneAddict;
				if (!this.Sleuthing)
				{
					if (!this.StudentManager.Eighties)
					{
						this.SprintAnim = this.PhoneAnims[2];
					}
				}
				else
				{
					this.SprintAnim = this.SleuthReportAnim;
				}
			}
			else
			{
				this.SmartPhone.SetActive(false);
			}
		}
		this.StopPairing();
		if (this.Persona != PersonaType.Heroic)
		{
			this.AlarmTimer = 0f;
			this.Alarm = 0f;
		}
		if (this.Teacher && this.Struggling)
		{
			Debug.Log("A teacher is currently engaged in a struggle with the player.");
		}
		if ((this.Teacher && this.Persona == PersonaType.Strict) || this.Persona == PersonaType.Heroic)
		{
			if (!this.Yandere.Chased && !this.Struggling && !this.Blind)
			{
				if (this.Teacher)
				{
					Debug.Log("A teacher has reached ChaseYandere through WitnessMurder.");
				}
				this.RetreivingMedicine = false;
				this.ChaseYandere();
			}
		}
		else
		{
			this.SpawnAlarmDisc();
		}
		if (!this.PinDownWitness && this.Persona != PersonaType.Evil && this.Persona != PersonaType.Protective)
		{
			this.StudentManager.Witnesses++;
			this.StudentManager.WitnessList[this.StudentManager.Witnesses] = this;
			this.StudentManager.PinDownCheck();
			this.PinDownWitness = true;
		}
		if (this.Persona == PersonaType.Violent)
		{
			this.Pathfinding.canSearch = false;
			this.Pathfinding.canMove = false;
		}
		if (this.Yandere.Mask == null)
		{
			this.SawMask = false;
			if (this.Persona != PersonaType.Evil)
			{
				this.Grudge = true;
			}
		}
		else
		{
			this.SawMask = true;
		}
		this.StudentManager.UpdateMe(this.StudentID);
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x001F5E04 File Offset: 0x001F4004
	public void DropMisplacedWeapon()
	{
		if (this.BloodPool == null)
		{
			Debug.Log(this.Name + " was told to DropMisplacedWeapon(), but BloodPool was null!!");
		}
		this.WitnessedWeapon = false;
		this.InvestigatingBloodPool = false;
		this.ReturningMisplacedWeaponPhase = 0;
		this.ReturningMisplacedWeapon = false;
		this.BloodPool.GetComponent<WeaponScript>().Returner = null;
		this.BloodPool.GetComponent<WeaponScript>().DoNotRelocate = true;
		this.BloodPool.GetComponent<WeaponScript>().Drop();
		this.BloodPool.GetComponent<WeaponScript>().enabled = true;
		this.BloodPool = null;
		this.WalkAnim = this.BeforeReturnAnim;
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x001F5EA8 File Offset: 0x001F40A8
	private void ChaseYandere()
	{
		if (this.Persona == PersonaType.Coward || this.Persona == PersonaType.Evil || this.Persona == PersonaType.Fragile || this.Persona == PersonaType.Loner || this.Persona == PersonaType.LandlineUser)
		{
			Debug.Log("On second thought, " + this.Name + " will not chase Yandere-chan. She's using one of the cowardly Personas.");
			return;
		}
		Debug.Log(this.Name + " has begun to chase Yandere-chan.");
		this.CurrentDestination = this.Yandere.transform;
		this.Pathfinding.target = this.Yandere.transform;
		this.Pathfinding.speed = 5f;
		if (this.Yandere.Pursuer == null)
		{
			this.Yandere.Pursuer = this;
		}
		this.TargetDistance = 1f;
		this.AlarmTimer = 0f;
		this.Pursuing = true;
		this.Chasing = false;
		this.Fleeing = false;
		this.StudentManager.UpdateStudents(0);
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x001F5FA4 File Offset: 0x001F41A4
	private void PersonaReaction()
	{
		Debug.Log(this.Name + " just called PersonaReaction(). As of now, they are a: " + this.Persona.ToString() + ".");
		if (this.Persona == PersonaType.Strict && !this.Teacher)
		{
			this.Persona = PersonaType.Heroic;
		}
		if (this.Persona == PersonaType.Sleuth)
		{
			if (this.Sleuthing)
			{
				if (!this.Phoneless)
				{
					this.Persona = PersonaType.PhoneAddict;
					this.SmartPhone.SetActive(true);
				}
				else
				{
					this.Persona = PersonaType.Loner;
				}
			}
			else if (this.StudentManager.Eighties)
			{
				this.Persona = PersonaType.LandlineUser;
			}
			else if (!this.Phoneless)
			{
				this.Persona = PersonaType.SocialButterfly;
			}
			else
			{
				this.Persona = PersonaType.Loner;
			}
		}
		if ((this.Persona == PersonaType.PhoneAddict && this.Phoneless) || (this.Persona == PersonaType.SocialButterfly && this.Phoneless))
		{
			this.Persona = PersonaType.Loner;
		}
		if (this.Persona == PersonaType.Vengeful)
		{
			this.Persona = PersonaType.Heroic;
		}
		if (this.Persona == PersonaType.Violent && this.MyWeapon == null)
		{
			this.Persona = PersonaType.Heroic;
		}
		if (!this.Indoors && this.WitnessedMurder && !this.Rival)
		{
			Debug.Log(this.Name + "'s current Persona is: " + this.Persona.ToString());
			if ((this.Persona != PersonaType.Evil && this.Persona != PersonaType.Heroic && this.Persona != PersonaType.Coward && this.Persona != PersonaType.PhoneAddict && this.Persona != PersonaType.Protective && this.Persona != PersonaType.Violent) || this.Injured)
			{
				Debug.Log(this.Name + " is switching to the Loner Persona.");
				this.Persona = PersonaType.Loner;
			}
		}
		if (!this.WitnessedMurder)
		{
			if (this.Persona == PersonaType.Heroic)
			{
				this.SwitchBack = true;
				this.Persona = ((this.Corpse != null) ? PersonaType.TeachersPet : PersonaType.Loner);
			}
			else if (this.Persona == PersonaType.Coward || this.Persona == PersonaType.Evil || this.Persona == PersonaType.Fragile)
			{
				this.Persona = PersonaType.Loner;
			}
			else if (this.Persona == PersonaType.Protective)
			{
				Debug.Log("Raibaru witnessed the corpse of " + this.Corpse.Student.Name + ", and is now switching to the Lovestruck Persona.");
				this.Persona = PersonaType.Lovestruck;
			}
			if (!this.StudentManager.Eighties && (this.StudentID == 2 || this.StudentID == 3) && this.Corpse != null && (this.Corpse.StudentID == 2 || this.Corpse.StudentID == 3))
			{
				this.Persona = PersonaType.Lovestruck;
			}
		}
		if (this.Persona == PersonaType.Lovestruck)
		{
			Debug.Log("Because " + this.Name + " is Lovestruck, she is doing a quick check to see if Senpai is unavailable or dead.");
			if (this.StudentManager.Students[this.LovestruckTarget] != null && this.StudentManager.Students[this.LovestruckTarget].SenpaiWitnessingRivalDie)
			{
				Debug.Log(this.Name + " can't run and tell Senpai, because Senpai is busy mourning a rival's corpse.");
				this.Persona = PersonaType.Loner;
			}
			else if (this.StudentManager.Students[this.LovestruckTarget] != null && !this.StudentManager.Students[this.LovestruckTarget].gameObject.activeInHierarchy)
			{
				Debug.Log(this.Name + " can't run and tell Senpai, because Senpai is disabled for some reason.");
				this.Persona = PersonaType.Loner;
			}
		}
		if (!this.DoNotMourn && this.InCouple && this.Corpse.Student.StudentID == this.PartnerID)
		{
			Debug.Log(this.Name + " is in a couple and just found the corpse of their partner.");
			this.Persona = PersonaType.Lovestruck;
			this.StudentToMourn = this.Corpse.Student;
			this.LovestruckTarget = this.PartnerID;
		}
		if (this.Persona == PersonaType.Loner || this.Persona == PersonaType.Spiteful)
		{
			Debug.Log(this.Name + " is looking in the Loner/Spiteful section of PersonaReaction() to decide what to do next.");
			if (this.Club == ClubType.Delinquent)
			{
				Debug.Log("A delinquent turned into a loner, and now he is fleeing.");
				if (this.Injured && this.WitnessedMurder)
				{
					Debug.Log("You won't get away with what you've done!");
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentInjuredFlee, 1, 3f);
				}
				else if (this.FoundFriendCorpse)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentFriendFlee, 1, 3f);
				}
				else if (this.FoundEnemyCorpse)
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentEnemyFlee, 1, 3f);
				}
				else
				{
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentFlee, 1, 3f);
				}
			}
			else if (this.WitnessedMurder)
			{
				this.Subtitle.UpdateLabel(SubtitleType.LonerMurderReaction, 1, 3f);
			}
			else
			{
				this.Subtitle.UpdateLabel(SubtitleType.LonerCorpseReaction, 1, 3f);
			}
			if (this.Schoolwear > 0)
			{
				if (!this.Bloody)
				{
					this.Pathfinding.target = this.StudentManager.Exit;
					this.TargetDistance = 0f;
					this.Routine = false;
					this.Fleeing = true;
				}
				else
				{
					this.FleeWhenClean = true;
					this.TargetDistance = 1f;
					this.BatheFast = true;
				}
			}
			else
			{
				this.FleeWhenClean = true;
				if (!this.Bloody)
				{
					this.BathePhase = 5;
					this.GoChange();
				}
				else
				{
					this.CurrentDestination = this.StudentManager.FastBatheSpot;
					this.Pathfinding.target = this.StudentManager.FastBatheSpot;
					this.TargetDistance = 1f;
					this.BatheFast = true;
				}
			}
			if (this.Corpse != null && this.StudentID == 1 && this.Corpse.Student.Rival)
			{
				Debug.Log("This is the moment that Senpai decides to run to the corpse of a rival.");
				this.CurrentDestination = this.Corpse.Student.Hips;
				this.Pathfinding.target = this.Corpse.Student.Hips;
				this.SenpaiWitnessingRivalDie = true;
				this.IgnoringPettyActions = true;
				this.DistanceToDestination = 1f;
				this.WitnessRivalDiePhase = 3;
				this.Routine = false;
				this.TargetDistance = 0.5f;
			}
		}
		else if (this.Persona == PersonaType.TeachersPet)
		{
			if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
			{
				if (this.StudentManager.BloodReporter == null)
				{
					if (this.Club != ClubType.Delinquent && !this.Police.Called && !this.LostTeacherTrust)
					{
						this.StudentManager.BloodLocation.position = this.BloodPool.transform.position;
						this.StudentManager.LowerBloodPosition();
						Debug.Log(this.Name + " has become a ''blood reporter''.");
						this.StudentManager.BloodReporter = this;
						this.ReportingBlood = true;
						this.DetermineBloodLocation();
					}
					else
					{
						this.ReportingBlood = false;
					}
				}
			}
			else if (this.StudentManager.Reporter == null && !this.Police.Called && this.StudentManager.CorpseLocation != null)
			{
				if (this.Corpse != null)
				{
					this.StudentManager.CorpseLocation.position = this.Corpse.AllColliders[0].transform.position;
					this.StudentManager.LowerCorpsePosition();
				}
				Debug.Log(this.Name + " has become a ''reporter''.");
				this.StudentManager.Reporter = this;
				this.ReportingMurder = true;
				this.IgnoringPettyActions = true;
				this.DetermineCorpseLocation();
			}
			if (this.StudentManager.Reporter == this)
			{
				Debug.Log(this.Name + " is running to a teacher to report murder.");
				this.Pathfinding.target = this.StudentManager.Teachers[this.Class].transform;
				this.CurrentDestination = this.StudentManager.Teachers[this.Class].transform;
				this.TargetDistance = 2f;
				if (this.Club == ClubType.Council)
				{
					if (this.StudentID == 86)
					{
						this.Subtitle.UpdateLabel(SubtitleType.StrictReport, 1, 5f);
					}
					else if (this.StudentID == 87)
					{
						this.Subtitle.UpdateLabel(SubtitleType.CasualReport, 1, 5f);
					}
					else if (this.StudentID == 88)
					{
						this.Subtitle.UpdateLabel(SubtitleType.GraceReport, 1, 5f);
					}
					else if (this.StudentID == 89)
					{
						this.Subtitle.UpdateLabel(SubtitleType.EdgyReport, 1, 5f);
					}
				}
				else if (this.WitnessedMurder)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetMurderReport, 1, 3f);
				}
				else if (this.WitnessedCorpse)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetCorpseReport, 1, 3f);
				}
			}
			else if (this.StudentManager.BloodReporter == this)
			{
				Debug.Log(this.Name + " is running to a teacher to report something.");
				this.DropPlate();
				this.Pathfinding.target = this.StudentManager.Teachers[this.Class].transform;
				this.CurrentDestination = this.StudentManager.Teachers[this.Class].transform;
				this.TargetDistance = 2f;
				if (this.WitnessedLimb)
				{
					this.Subtitle.UpdateLabel(SubtitleType.LimbReaction, 1, 3f);
				}
				else if (this.WitnessedBloodyWeapon)
				{
					this.Subtitle.UpdateLabel(SubtitleType.BloodyWeaponReaction, 1, 3f);
				}
				else if (this.WitnessedBloodPool)
				{
					this.Subtitle.UpdateLabel(SubtitleType.BloodPoolReaction, 1, 3f);
				}
				else if (this.WitnessedWeapon)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetWeaponReport, 1, 3f);
				}
			}
			else
			{
				Debug.Log(this.Name + " found something scary, and is now deciding what to do next.");
				if (this.Club == ClubType.Council)
				{
					if (this.WitnessedCorpse)
					{
						if (this.StudentManager.CorpseLocation.position == Vector3.zero)
						{
							this.StudentManager.CorpseLocation.position = this.Corpse.AllColliders[0].transform.position;
							this.AssignCorpseGuardLocations();
						}
						if (this.StudentID == 86)
						{
							this.Pathfinding.target = this.StudentManager.CorpseGuardLocation[1];
						}
						else if (this.StudentID == 87)
						{
							this.Pathfinding.target = this.StudentManager.CorpseGuardLocation[2];
						}
						else if (this.StudentID == 88)
						{
							this.Pathfinding.target = this.StudentManager.CorpseGuardLocation[3];
						}
						else if (this.StudentID == 89)
						{
							this.Pathfinding.target = this.StudentManager.CorpseGuardLocation[4];
						}
						this.CurrentDestination = this.Pathfinding.target;
					}
					else
					{
						Debug.Log("A student council member is being told to travel to ''BloodGuardLocation''.");
						if (this.StudentManager.BloodLocation.position == Vector3.zero)
						{
							this.StudentManager.BloodLocation.position = this.BloodPool.transform.position;
							this.AssignBloodGuardLocations();
						}
						if (this.StudentManager.BloodGuardLocation[1].position == Vector3.zero)
						{
							this.AssignBloodGuardLocations();
						}
						if (this.StudentID == 86)
						{
							this.Pathfinding.target = this.StudentManager.BloodGuardLocation[1];
						}
						else if (this.StudentID == 87)
						{
							this.Pathfinding.target = this.StudentManager.BloodGuardLocation[2];
						}
						else if (this.StudentID == 88)
						{
							this.Pathfinding.target = this.StudentManager.BloodGuardLocation[3];
						}
						else if (this.StudentID == 89)
						{
							this.Pathfinding.target = this.StudentManager.BloodGuardLocation[4];
						}
						this.CurrentDestination = this.Pathfinding.target;
						this.Guarding = true;
					}
				}
				else
				{
					Debug.Log(this.Name + " is considering whether or not to hide in their classroom...");
					if (Vector3.Distance(base.transform.position, this.Seat.position) < 2f)
					{
						Debug.Log("...but there is danger in their classroom, so they will flee the school instead.");
						this.Pathfinding.target = this.StudentManager.Exit;
						this.CurrentDestination = this.StudentManager.Exit;
					}
					else
					{
						this.PetDestination = UnityEngine.Object.Instantiate<GameObject>(this.EmptyGameObject, this.Seat.position + this.Seat.forward * -0.5f, Quaternion.identity).transform;
						this.Pathfinding.target = this.PetDestination;
						this.CurrentDestination = this.PetDestination;
					}
					if (this.Distracting)
					{
						if (this.DistractionTarget != null)
						{
							this.DistractionTarget.TargetedForDistraction = false;
						}
						this.ResumeDistracting = false;
						this.Distracting = false;
					}
					this.DropPlate();
				}
				if (this.WitnessedMurder)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetMurderReaction, 1, 3f);
				}
				else if (this.WitnessedCorpse)
				{
					if (this.Club == ClubType.Council)
					{
						this.Subtitle.CustomText = "How did this happen?!";
						this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					}
					else
					{
						this.Subtitle.UpdateLabel(SubtitleType.PetCorpseReaction, 1, 3f);
					}
				}
				else if (this.WitnessedLimb)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetLimbReaction, 1, 3f);
				}
				else if (this.WitnessedBloodyWeapon)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetBloodyWeaponReaction, 1, 3f);
				}
				else if (this.WitnessedBloodPool)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetBloodReaction, 1, 3f);
				}
				else if (this.WitnessedWeapon)
				{
					this.Subtitle.UpdateLabel(SubtitleType.PetWeaponReaction, 1, 3f);
				}
				this.TargetDistance = 1f;
				this.ReportingMurder = false;
				this.ReportingBlood = false;
			}
			this.Routine = false;
			this.Fleeing = true;
		}
		else if (this.Persona == PersonaType.Heroic || this.Persona == PersonaType.Protective)
		{
			Debug.Log("A Heroic student is now running PersonaReaction()...");
			this.Headache = false;
			if (!this.Yandere.Chased)
			{
				this.StudentManager.PinDownCheck();
				if (!this.StudentManager.PinningDown)
				{
					Debug.Log(this.Name + "'s ''Flee'' was set to ''true'' because Hero persona reaction was called.");
					if (this.Persona == PersonaType.Protective)
					{
						this.Subtitle.PreviousSubtitle = SubtitleType.AcceptFood;
						Debug.Log("You won't get away with this!");
						this.Subtitle.UpdateLabel(SubtitleType.ObstacleMurderReaction, 2, 3f);
					}
					else if (this.Persona != PersonaType.Violent)
					{
						this.Subtitle.UpdateLabel(SubtitleType.HeroMurderReaction, 3, 3f);
					}
					else if (this.Defeats > 0)
					{
						this.Subtitle.Speaker = this;
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentResume, 3, 3f);
					}
					else
					{
						this.Subtitle.Speaker = this;
						this.Subtitle.UpdateLabel(SubtitleType.DelinquentMurderReaction, 3, 3f);
					}
					this.Pathfinding.target = this.Yandere.transform;
					this.Pathfinding.speed = 5f;
					this.Yandere.Pursuer = this;
					this.Yandere.Chased = true;
					this.TargetDistance = 1f;
					this.StudentManager.UpdateStudents(0);
					this.Routine = false;
					this.Fleeing = true;
				}
			}
		}
		else if (this.Persona == PersonaType.Coward || this.Persona == PersonaType.Fragile)
		{
			Debug.Log(this.Name + " just set their destination to themself.");
			this.CurrentDestination = base.transform;
			this.Pathfinding.target = base.transform;
			this.Subtitle.UpdateLabel(SubtitleType.CowardMurderReaction, 1, 5f);
			this.Routine = false;
			this.Fleeing = true;
		}
		else if (this.Persona == PersonaType.Evil)
		{
			Debug.Log("This character just set their destination to themself.");
			this.CurrentDestination = base.transform;
			this.Pathfinding.target = base.transform;
			this.Subtitle.UpdateLabel(SubtitleType.EvilMurderReaction, 1, 5f);
			this.Routine = false;
			this.Fleeing = true;
		}
		else if (this.Persona == PersonaType.SocialButterfly)
		{
			Debug.Log("A social butterfly is calling PersonaReaction().");
			this.StudentManager.HidingSpots.List[this.StudentID].position = this.StudentManager.PopulationManager.GetCrowdedLocation();
			this.CurrentDestination = this.StudentManager.HidingSpots.List[this.StudentID];
			this.Pathfinding.target = this.StudentManager.HidingSpots.List[this.StudentID];
			this.Subtitle.UpdateLabel(SubtitleType.SocialDeathReaction, 1, 5f);
			this.TargetDistance = 2f;
			this.ReportPhase = 1;
			this.Routine = false;
			this.Fleeing = true;
			this.Halt = true;
		}
		else if (this.Persona == PersonaType.Lovestruck)
		{
			Debug.Log(this.Name + " is now calling the the Lovestruck Persona code.");
			if (this.Corpse != null)
			{
				Debug.Log(this.Name + " witnessed the corpse of: " + this.Corpse.Student.Name);
			}
			bool flag = false;
			if (!this.WitnessedMurder && this.Corpse != null && ((this.StudentID == 1 && this.Corpse.Student.Rival) || (this.Corpse.Student == this.FollowTarget || (this.InCouple && this.Corpse.Student.StudentID == this.PartnerID)) || (!this.StudentManager.Eighties && this.StudentID == 11 && this.Corpse.StudentID == this.StudentManager.ObstacleID) || (!this.StudentManager.Eighties && this.StudentID > 1 && this.StudentID < 4 && this.Corpse.StudentID > 1 && this.Corpse.StudentID < 4)))
			{
				flag = true;
			}
			if (flag)
			{
				Debug.Log(this.Name + " is going to have a special reaction to the corpse of " + this.Corpse.Student.Name);
				this.CurrentDestination = this.Corpse.Student.Hips;
				this.Pathfinding.target = this.Corpse.Student.Hips;
				this.CorpseHead = this.Corpse.Student.Head;
				this.StudentToMourn = this.Corpse.Student;
				this.SpecialRivalDeathReaction = true;
				this.IgnoringPettyActions = true;
				this.WitnessRivalDiePhase = 1;
				this.Routine = false;
				this.TargetDistance = 0.5f;
				this.CorpseID = this.Corpse.StudentID;
			}
			else
			{
				if (this.LovestruckTarget == 0)
				{
					this.LovestruckTarget = 1;
				}
				if (!this.StudentManager.Students[this.LovestruckTarget].WitnessedMurder)
				{
					Debug.Log(this.Name + "'s new destination should be " + this.StudentManager.Students[this.LovestruckTarget].Name);
					this.CurrentDestination = this.StudentManager.Students[this.LovestruckTarget].transform;
					this.Pathfinding.target = this.StudentManager.Students[this.LovestruckTarget].transform;
					this.StudentManager.Students[this.LovestruckTarget].TargetedForDistraction = true;
					this.TargetDistance = 1f;
					this.ReportPhase = 1;
				}
				else
				{
					Debug.Log(this.Name + "'s new destination should be the exit of the school.");
					this.CurrentDestination = this.StudentManager.Exit;
					this.Pathfinding.target = this.StudentManager.Exit;
					this.TargetDistance = 0f;
					this.ReportPhase = 3;
				}
				if (this.LovestruckTarget == 1)
				{
					this.Subtitle.UpdateLabel(SubtitleType.LovestruckDeathReaction, 0, 5f);
				}
				else if (this.WitnessedMindBrokenMurder)
				{
					this.Subtitle.CustomText = "This can't be happening...";
					this.Subtitle.UpdateLabel(SubtitleType.Custom, 1, 5f);
				}
				else
				{
					this.Subtitle.UpdateLabel(SubtitleType.LovestruckDeathReaction, 1, 5f);
				}
				this.DistanceToDestination = 100f;
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				this.Routine = false;
				this.Fleeing = true;
				this.Halt = true;
			}
		}
		else if (this.Persona == PersonaType.Dangerous)
		{
			Debug.Log("A student council member's PersonaReaction has been triggered.");
			if (this.WitnessedMurder)
			{
				Debug.Log("A student council member's ''WitnessedMurder'' has been set to ''true''.");
				if (!this.Yandere.DelinquentFighting)
				{
					this.Subtitle.UpdateLabel(SubtitleType.Chasing, this.ClubMemberID, 5f);
				}
				this.Pathfinding.target = this.Yandere.transform;
				this.Pathfinding.speed = 5f;
				this.Yandere.Chased = true;
				this.TargetDistance = 1f;
				this.StudentManager.UpdateStudents(0);
				this.Routine = false;
				this.Fleeing = true;
				this.Halt = true;
			}
			else
			{
				Debug.Log("A student council member has transformed into a Teacher's Pet.");
				this.Persona = PersonaType.TeachersPet;
				this.PersonaReaction();
			}
		}
		else if (this.Persona == PersonaType.PhoneAddict)
		{
			Debug.Log(this.Name + " is executing the Phone Addict Persona code.");
			this.CurrentDestination = this.StudentManager.Exit;
			this.Pathfinding.target = this.StudentManager.Exit;
			if (!this.StudentManager.Eighties)
			{
				if (this.MaiHair != null)
				{
					Debug.Log("Freeing Mai's hair from gravity.");
					this.MaiHair.m_Gravity = Vector3.zero;
				}
				this.Countdown.gameObject.SetActive(true);
				if (this.StudentManager.ChaseCamera == null)
				{
					this.StudentManager.ChaseCamera = this.ChaseCamera;
					this.ChaseCamera.SetActive(true);
				}
			}
			this.Routine = false;
			this.Fleeing = true;
		}
		else if (this.Persona == PersonaType.Violent)
		{
			Debug.Log(this.Name + ", a delinquent, is currently in the ''Violent'' part of PersonaReaction()");
			if (this.WitnessedMurder)
			{
				if (!this.Yandere.Chased)
				{
					this.StudentManager.PinDownCheck();
					if (!this.StudentManager.PinningDown)
					{
						Debug.Log(this.Name + " began fleeing because Violent persona reaction was called.");
						if (this.Defeats > 0)
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.UpdateLabel(SubtitleType.DelinquentResume, 3, 3f);
						}
						else
						{
							this.Subtitle.Speaker = this;
							this.Subtitle.UpdateLabel(SubtitleType.DelinquentMurderReaction, 3, 3f);
						}
						this.Pathfinding.target = this.Yandere.transform;
						this.Pathfinding.canSearch = true;
						this.Pathfinding.canMove = true;
						this.Pathfinding.speed = 5f;
						this.Yandere.Pursuer = this;
						this.Yandere.Chased = true;
						this.TargetDistance = 1f;
						this.StudentManager.UpdateStudents(0);
						this.Routine = false;
						this.Fleeing = true;
					}
				}
			}
			else
			{
				Debug.Log("A delinquent reached the ''Flee'' protocol through PersonaReaction().");
				if (this.FoundFriendCorpse)
				{
					Debug.Log("Found Friend Corpse.");
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentFriendFlee, 1, 3f);
				}
				else
				{
					Debug.Log("Didn't Find Friend Corpse.");
					this.Subtitle.Speaker = this;
					this.Subtitle.UpdateLabel(SubtitleType.DelinquentFlee, 1, 3f);
				}
				this.CurrentDestination = this.StudentManager.Exit;
				this.Pathfinding.target = this.StudentManager.Exit;
				this.Pathfinding.canSearch = true;
				this.Pathfinding.canMove = true;
				this.TargetDistance = 0f;
				this.Routine = false;
				this.Fleeing = true;
			}
		}
		else if (this.Persona == PersonaType.Strict)
		{
			if (this.Yandere.Pursuer == this)
			{
				Debug.Log("This teacher is now pursuing Yandere-chan.");
			}
			if (this.WitnessedMurder)
			{
				if (this.Yandere.Pursuer == this)
				{
					Debug.Log("A teacher is now reacting to the sight of murder.");
					this.Subtitle.UpdateLabel(SubtitleType.TeacherMurderReaction, 3, 3f);
					this.Pathfinding.target = this.Yandere.transform;
					this.Pathfinding.speed = 5f;
					this.Yandere.Chased = true;
					this.TargetDistance = 1f;
					this.StudentManager.UpdateStudents(0);
					this.Routine = false;
					this.Fleeing = true;
				}
				else if (!this.Yandere.Chased)
				{
					if (this.Yandere.FightHasBrokenUp)
					{
						Debug.Log("This teacher is returning to normal after witnessing a SC member break up a fight.");
						this.WitnessedMurder = false;
						this.PinDownWitness = false;
						this.Alarmed = false;
						this.Reacted = false;
						this.Routine = true;
						this.Grudge = false;
						this.AlarmTimer = 0f;
						this.PreviousEyeShrink = 0f;
						this.EyeShrink = 0f;
						this.PreviousAlarm = 0f;
						this.MurdersWitnessed = 0;
						this.Concern = 0;
						this.Witnessed = StudentWitnessType.None;
						this.GameOverCause = GameOverType.None;
						this.CurrentDestination = this.Destinations[this.Phase];
						this.Pathfinding.target = this.Destinations[this.Phase];
					}
					else
					{
						Debug.Log("A teacher has reached ChaseYandere through PersonaReaction.");
						this.ChaseYandere();
					}
				}
			}
			else if (this.WitnessedCorpse)
			{
				Debug.Log("A teacher is now reacting to the sight of a corpse.");
				if (this.ReportPhase == 0)
				{
					this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseReaction, 1, 3f);
				}
				float y = this.Corpse.AllColliders[0].transform.position.y;
				float y2;
				if (y > 1.4f && y < 1.6f)
				{
					y2 = 1.4f;
				}
				else if (y < 2f)
				{
					y2 = 0f;
				}
				else if (y < 4f)
				{
					y2 = 2f;
				}
				else if (y < 6f)
				{
					y2 = 4f;
				}
				else if (y < 8f)
				{
					y2 = 6f;
				}
				else if (y < 10f)
				{
					y2 = 8f;
				}
				else if (y < 12f)
				{
					y2 = 10f;
				}
				else
				{
					y2 = 12f;
				}
				Debug.Log("PathfindingTarget's height has been set to: " + y2.ToString());
				this.ExamineCorpseTarget = UnityEngine.Object.Instantiate<GameObject>(this.EmptyGameObject, new Vector3(this.Corpse.AllColliders[0].transform.position.x, y2, this.Corpse.AllColliders[0].transform.position.z), Quaternion.identity).transform;
				this.Pathfinding.target = this.ExamineCorpseTarget;
				this.Pathfinding.target.position = Vector3.MoveTowards(this.Pathfinding.target.position, new Vector3(base.transform.position.x, y2, base.transform.position.z), 1.5f);
				this.TargetDistance = 1f;
				this.ReportPhase = 2;
				this.IgnoringPettyActions = true;
				this.Routine = false;
				this.Fleeing = true;
			}
			else
			{
				Debug.Log("A teacher is now reacting to the sight of a severed limb, blood pool, or weapon.");
				if (this.ReportPhase == 0)
				{
					if (this.WitnessedBloodPool || this.WitnessedBloodyWeapon)
					{
						this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 3, 3f);
					}
					else if (this.WitnessedLimb)
					{
						this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 4, 3f);
					}
					else if (this.WitnessedWeapon)
					{
						this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseInspection, 5, 3f);
					}
				}
				this.TargetDistance = 1f;
				this.ReportPhase = 2;
				this.VerballyReacted = true;
				this.Routine = false;
				this.Fleeing = true;
				this.Halt = true;
			}
		}
		else if (this.Persona == PersonaType.LandlineUser)
		{
			Debug.Log("A Snitch is calling PersonaReaction().");
			this.CurrentDestination = this.StudentManager.LandLineSpot;
			this.Pathfinding.target = this.StudentManager.LandLineSpot;
			this.Subtitle.UpdateLabel(SubtitleType.SocialDeathReaction, 1, 5f);
			this.TargetDistance = 1f;
			this.ReportPhase = 1;
			this.Routine = false;
			this.Fleeing = true;
		}
		if (this.StudentID == 41 && !this.StudentManager.Eighties)
		{
			this.Subtitle.UpdateLabel(SubtitleType.Impatience, 6, 5f);
		}
		Debug.Log(this.Name + " has finished calling PersonaReaction(). As of now, they are a: " + this.Persona.ToString() + ".");
		if (this.WitnessedCorpse)
		{
			Debug.Log(this.Name + " witnessed a corpse...");
			if (this.Distracting)
			{
				Debug.Log("...so ''Distracting'' is being set to false.");
				if (this.DistractionTarget != null)
				{
					this.DistractionTarget.TargetedForDistraction = false;
				}
				this.ResumeDistracting = false;
				this.Distracting = false;
			}
		}
		this.Alarm = 0f;
		this.UpdateDetectionMarker();
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x001F7D7C File Offset: 0x001F5F7C
	private void BeginStruggle()
	{
		Debug.Log(this.Name + " has begun a struggle with Yandere-chan.");
		if (this.Yandere.Hiding)
		{
			this.Yandere.Hiding = false;
		}
		if (this.Yandere.Dragging)
		{
			this.Yandere.Ragdoll.GetComponent<RagdollScript>().StopDragging();
		}
		if (this.Yandere.Armed)
		{
			this.Yandere.EquippedWeapon.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		this.Yandere.StruggleBar.OriginalDOF = this.CameraEffects.GetDOF();
		this.Yandere.StruggleBar.Strength = (float)this.Strength;
		this.Yandere.StruggleBar.Struggling = true;
		this.Yandere.StruggleBar.Student = this;
		this.Yandere.StruggleBar.gameObject.SetActive(true);
		this.CharacterAnimation.CrossFade(this.StruggleAnim);
		this.Yandere.ShoulderCamera.LastPosition = this.Yandere.ShoulderCamera.transform.position;
		this.Yandere.ShoulderCamera.Struggle = true;
		this.Pathfinding.canSearch = false;
		this.Pathfinding.canMove = false;
		this.Obstacle.enabled = true;
		this.Struggling = true;
		this.DiscCheck = false;
		this.Alarmed = false;
		this.Halt = true;
		if (!this.Teacher)
		{
			this.Yandere.CharacterAnimation["f02_struggleA_00"].time = 0f;
		}
		else
		{
			this.Yandere.CharacterAnimation["f02_teacherStruggleA_00"].time = 0f;
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (this.Yandere.Aiming)
		{
			this.Yandere.StopAiming();
		}
		this.Yandere.StopLaughing();
		this.Yandere.TargetStudent = this;
		this.Yandere.YandereVision = false;
		this.Yandere.NearSenpai = false;
		this.Yandere.Struggling = true;
		this.Yandere.CanMove = false;
		if (this.Yandere.DelinquentFighting)
		{
			this.StudentManager.CombatMinigame.Stop();
		}
		this.Yandere.EmptyHands();
		this.Yandere.RPGCamera.enabled = false;
		this.MyController.radius = 0f;
		this.TargetDistance = 100f;
		this.AlarmTimer = 0f;
		this.SpawnAlarmDisc();
		Debug.Log("Manually setting DOF to 1 from here.");
		this.CameraEffects.UpdateDOF(1f);
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x001F8048 File Offset: 0x001F6248
	public void GetDestinations()
	{
		if (!this.Teacher)
		{
			this.MyLocker = this.StudentManager.LockerPositions[this.StudentID];
		}
		if (this.Slave)
		{
			foreach (ScheduleBlock scheduleBlock in this.ScheduleBlocks)
			{
				scheduleBlock.destination = "Slave";
				scheduleBlock.action = "Slave";
			}
		}
		if (this.ScheduleBlocks.Length != 0)
		{
			this.ID = 1;
			while (this.ID < this.JSON.Students[this.StudentID].ScheduleBlocks.Length)
			{
				ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[this.ID];
				if (scheduleBlock2.destination == "Locker")
				{
					this.Destinations[this.ID] = this.MyLocker;
				}
				else if (scheduleBlock2.destination == "Seat")
				{
					this.Destinations[this.ID] = this.Seat;
				}
				else if (scheduleBlock2.destination == "SocialSeat")
				{
					this.Destinations[this.ID] = this.StudentManager.SocialSeats[this.StudentID];
					if (this.Destinations[this.ID] == null)
					{
						this.Destinations[this.ID] = this.Seat;
					}
				}
				else if (scheduleBlock2.destination == "Podium")
				{
					this.Destinations[this.ID] = this.StudentManager.Podiums.List[this.Class];
				}
				else if (scheduleBlock2.destination == "Exit")
				{
					this.Destinations[this.ID] = this.StudentManager.Hangouts.List[0];
				}
				else if (scheduleBlock2.destination == "Hangout")
				{
					this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Week1Hangout")
				{
					this.Destinations[this.ID] = this.StudentManager.Week1Hangouts.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Week2Hangout")
				{
					this.Destinations[this.ID] = this.StudentManager.Week2Hangouts.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Stairway")
				{
					this.Destinations[this.ID] = this.StudentManager.Stairways.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "LunchSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.LunchSpots.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Slave")
				{
					if (!this.FragileSlave)
					{
						this.Destinations[this.ID] = this.StudentManager.SlaveSpot;
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.FragileSlaveSpot;
					}
				}
				else if (scheduleBlock2.destination == "Patrol")
				{
					this.Destinations[this.ID] = this.StudentManager.Patrols.List[this.StudentID].GetChild(0);
					if (this.OriginalClub != ClubType.None && this.Club == ClubType.None && this.OriginalClub != ClubType.Occult && this.OriginalClub != ClubType.Gardening)
					{
						Debug.Log("Student #" + this.StudentID.ToString() + "'s club disbanded, so their destination has been set to ''Hangout''.");
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
				}
				else if (scheduleBlock2.destination == "Search Patrol")
				{
					this.StudentManager.SearchPatrols.List[this.Class].GetChild(0).position = this.Seat.position + this.Seat.forward;
					this.StudentManager.SearchPatrols.List[this.Class].GetChild(0).LookAt(this.Seat);
					this.StudentManager.StolenPhoneSpot.transform.position = this.Seat.position + this.Seat.forward * 0.4f;
					this.StudentManager.StolenPhoneSpot.transform.position += Vector3.up;
					this.StudentManager.StolenPhoneSpot.gameObject.SetActive(true);
					this.Destinations[this.ID] = this.StudentManager.SearchPatrols.List[this.Class].GetChild(0);
				}
				else if (scheduleBlock2.destination == "Graffiti")
				{
					if (this.StudentManager.GraffitiSpots[this.BullyID] != null)
					{
						this.Destinations[this.ID] = this.StudentManager.GraffitiSpots[this.BullyID];
					}
					else if (this.StudentManager.Eighties)
					{
						this.Destinations[this.ID] = this.StudentManager.EightiesHangouts.List[this.StudentID];
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
				}
				else if (scheduleBlock2.destination == "Bully")
				{
					this.Destinations[this.ID] = this.StudentManager.BullySpots[this.BullyID];
				}
				else if (scheduleBlock2.destination == "Mourn")
				{
					this.Destinations[this.ID] = this.StudentManager.MournSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Clean")
				{
					if (this.CleaningSpot != null)
					{
						this.Destinations[this.ID] = this.CleaningSpot.GetChild(0);
					}
				}
				else if (scheduleBlock2.destination == "ShameSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.ShameSpot;
				}
				else if (scheduleBlock2.destination == "Follow")
				{
					if (this.FollowTarget != null)
					{
						this.Destinations[this.ID] = this.FollowTarget.FollowTargetDestination;
					}
				}
				else if (scheduleBlock2.destination == "Cuddle")
				{
					if (this.CuddlePartnerID == 0)
					{
						this.Destinations[this.ID] = this.StudentManager.FemaleCoupleSpots[this.CoupleID];
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.MaleCoupleSpots[this.CoupleID];
					}
				}
				else if (scheduleBlock2.destination == "Peek")
				{
					if (!this.Male)
					{
						this.Destinations[this.ID] = this.StudentManager.FemaleStalkSpot;
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.MaleStalkSpot;
					}
				}
				else if (scheduleBlock2.destination == "Club")
				{
					if (this.Club > ClubType.None)
					{
						this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID];
						if (this.Club == ClubType.Sports)
						{
							this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID].GetChild(0);
						}
					}
					else if (this.OriginalClub == ClubType.Cooking)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
					else if (this.OriginalClub == ClubType.Drama)
					{
						if (!this.StudentManager.MemorialScene.gameObject.activeInHierarchy)
						{
							Debug.Log("The Drama Club was shut down. They are still being told to perform their club action, though.");
							this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID];
						}
						else
						{
							Debug.Log("The Drama Club was shut down and the gym stage has props on it. Drama students are going to go do something else.");
							this.Destinations[this.ID] = this.Seat;
						}
					}
					else if (this.OriginalClub == ClubType.Gaming)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
					else if (this.OriginalClub == ClubType.Art)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
					else if (this.OriginalClub == ClubType.MartialArts)
					{
						this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID];
						this.DressCode = false;
					}
					else if (this.OriginalClub == ClubType.LightMusic)
					{
						this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID];
					}
					else if (this.OriginalClub == ClubType.Photography)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
					else if (this.OriginalClub == ClubType.Science)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
					else if (this.OriginalClub == ClubType.Sports)
					{
						this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID].GetChild(0);
					}
					else if (this.OriginalClub == ClubType.Gardening)
					{
						this.Destinations[this.ID] = this.StudentManager.Clubs.List[this.StudentID].GetChild(0);
					}
					else if (this.OriginalClub == ClubType.Newspaper)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
				}
				else if (scheduleBlock2.destination == "Sulk")
				{
					this.Destinations[this.ID] = this.StudentManager.SulkSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Sleuth")
				{
					this.Destinations[this.ID] = this.SleuthTarget;
				}
				else if (scheduleBlock2.destination == "Stalk")
				{
					this.Destinations[this.ID] = this.StalkTarget;
					this.Stalker = true;
				}
				else if (scheduleBlock2.destination == "Sunbathe")
				{
					this.Destinations[this.ID] = this.StudentManager.StrippingPositions[this.GirlID];
				}
				else if (scheduleBlock2.destination == "Shock")
				{
					if (ClubGlobals.GetClubClosed(ClubType.Gaming))
					{
						this.Destinations[this.ID] = this.StudentManager.AltShockedSpots[this.StudentID - 80];
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.ShockedSpots[this.StudentID - 80];
					}
				}
				else if (scheduleBlock2.destination == "Miyuki")
				{
					this.ClubMemberID = this.StudentID - 35;
					if (this.ClubMemberID > 0 && this.ClubMemberID < 6)
					{
						this.Destinations[this.ID] = this.StudentManager.MiyukiSpots[this.ClubMemberID].transform;
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.MiyukiSpots[1].transform;
					}
				}
				else if (scheduleBlock2.destination == "Practice")
				{
					this.Destinations[this.ID] = this.StudentManager.PracticeSpots[this.ClubMemberID];
					if (this.Club == ClubType.None && !this.StudentManager.Eighties && this.StudentID == 51)
					{
						this.Destinations[this.ID] = this.StudentManager.Hangouts.List[this.StudentID];
					}
				}
				else if (scheduleBlock2.destination == "Lyrics")
				{
					this.Destinations[this.ID] = this.StudentManager.LyricsSpot;
				}
				else if (scheduleBlock2.destination == "Meeting")
				{
					this.Destinations[this.ID] = this.StudentManager.MeetingSpots[this.StudentID].transform;
				}
				else if (scheduleBlock2.destination == "InfirmaryBed")
				{
					Debug.Log("Student #" + this.StudentID.ToString() + " is now trying to assign themself to an infirmary bed.");
					Debug.Log("StudentManager.SedatedStudents is currently: " + this.StudentManager.SedatedStudents.ToString());
					if (this.StudentManager.SedatedStudents < 4)
					{
						this.Destinations[this.ID] = this.StudentManager.RestSpots[this.StudentManager.SedatedStudents];
						if (!this.TakingUpASedatedSpot)
						{
							this.StudentManager.SedatedStudents++;
							this.TakingUpASedatedSpot = true;
						}
					}
					else if (this.StudentManager.HeadacheStudents < 4)
					{
						Debug.Log("Wait. Number of Sedated Students is too high. Someone will have to go sit in the infirmary seat, instead.");
						this.Destinations[this.ID] = this.StudentManager.InfirmarySeats[this.StudentManager.HeadacheStudents];
						if (!this.TakingUpAHeadacheSpot)
						{
							this.StudentManager.HeadacheStudents++;
							this.TakingUpAHeadacheSpot = true;
						}
					}
					else
					{
						Debug.Log("Wait. Number of Headache Students is too high. This student will just have to sit in their classroom stead, instead.");
						this.Destinations[this.ID] = this.Seat;
					}
				}
				else if (scheduleBlock2.destination == "InfirmarySeat")
				{
					this.Destinations[this.ID] = this.StudentManager.InfirmarySeats[this.StudentManager.HeadacheStudents];
					this.StudentManager.HeadacheStudents++;
				}
				else if (scheduleBlock2.destination == "Paint")
				{
					this.Destinations[this.ID] = this.StudentManager.FridaySpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "LockerRoom")
				{
					this.Destinations[this.ID] = this.StudentManager.MaleLockerRoomChangingSpot;
				}
				else if (scheduleBlock2.destination == "LunchWitnessPosition")
				{
					this.Destinations[this.ID] = this.StudentManager.LunchWitnessPositions.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Wait")
				{
					this.Destinations[this.ID] = this.StudentManager.WaitSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "SleepSpot")
				{
					Debug.Log(this.Name + " is setting destination to ''SleepSpot''.");
					this.Destinations[this.ID] = this.StudentManager.SleepSpot;
				}
				else if (scheduleBlock2.destination == "LightFire")
				{
					this.Destinations[this.ID] = this.StudentManager.PyroSpot;
				}
				else if (scheduleBlock2.destination == "EightiesSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.EightiesSpots.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "EightiesShowerSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.EightiesShowerSpots.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "EightiesDramaSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.EightiesDramaSpots.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "EightiesStretchSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.EightiesStretchSpots.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Perform")
				{
					this.Destinations[this.ID] = this.StudentManager.PerformSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "PhotoShoot")
				{
					this.Destinations[this.ID] = this.StudentManager.PhotoShootSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "Self")
				{
					this.Destinations[this.ID] = base.transform;
				}
				else if (scheduleBlock2.destination == "Guard")
				{
					Debug.Log("A student has been instructed to Guard!");
					if (this.StudentID == 20 || this.Rival)
					{
						Debug.Log("The character who needs to Guard someone is a rival!");
						this.Destinations[this.ID] = this.StudentManager.Students[1].transform;
					}
					else if (this.StudentManager.Students[this.StudentManager.RivalID] != null && this.StudentManager.RivalGuardSpots[this.StudentID] != null)
					{
						Debug.Log("A rival is present at school, and a ''RivalGuardSpot'' has been defined for this character!");
						if (this.StudentManager.CustomMode)
						{
							this.StudentManager.RivalGuardSpots[0].parent = this.StudentManager.Students[this.StudentManager.RivalID].transform;
							this.StudentManager.RivalGuardSpots[0].transform.localPosition = new Vector3(0f, 0f, 0f);
							this.StudentManager.RivalGuardSpots[0].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
						}
						this.Destinations[this.ID] = this.StudentManager.RivalGuardSpots[this.StudentID].transform;
					}
					else if (this.StudentManager.Students[this.StudentManager.RivalID] != null)
					{
						Debug.Log("A ''RivalGuardSpot'' has NOT been defined for this character.");
						this.Destinations[this.ID] = this.StudentManager.Students[this.StudentManager.RivalID].transform;
					}
					else
					{
						Debug.Log("...but the rival is gone! This student will just go to their seat instead.");
						this.Destinations[this.ID] = this.Seat;
					}
				}
				else if (scheduleBlock2.destination == "WitnessSpot")
				{
					int witnessBonus = this.WitnessBonus;
					if (this.WitnessID + this.WitnessBonus < this.StudentManager.WitnessSpots.Length)
					{
						this.Destinations[this.ID] = this.StudentManager.WitnessSpots[this.WitnessID + this.WitnessBonus];
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.WitnessSpots[this.WitnessID];
					}
				}
				else if (scheduleBlock2.destination == "LunchWitnessSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.LunchWitnessSpots[this.WitnessID];
				}
				else if (scheduleBlock2.destination == "CleanWitnessSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.CleaningWitnessSpots[this.WitnessID];
				}
				else if (scheduleBlock2.destination == "AfterWitnessSpot")
				{
					this.Destinations[this.ID] = this.StudentManager.AfterClassWitnessSpots[this.WitnessID + this.AfterWitnessBonus];
				}
				else if (scheduleBlock2.destination == "Random")
				{
					this.Destinations[this.ID] = this.StudentManager.RandomSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "CustomHangout")
				{
					this.Destinations[this.ID] = this.StudentManager.CustomHangouts.List[this.StudentID];
				}
				else if (scheduleBlock2.destination == "CustomPatrol")
				{
					this.Destinations[this.ID] = this.StudentManager.CustomPatrols.List[this.StudentID].GetChild(0);
				}
				else if (scheduleBlock2.destination == "RandomPatrol")
				{
					this.Destinations[this.ID] = this.StudentManager.CustomPatrols.List[UnityEngine.Random.Range(1, 101)].GetChild(UnityEngine.Random.Range(0, 2));
				}
				else if (scheduleBlock2.destination == "BakeSale")
				{
					if (this.BakeSalePhase == 0)
					{
						this.Destinations[this.ID] = this.StudentManager.BakeSalePrepSpots[this.StudentID];
					}
					else
					{
						this.Destinations[this.ID] = this.StudentManager.BakeSaleSpots[this.StudentID];
					}
				}
				else if (scheduleBlock2.destination == "Picnic")
				{
					this.Destinations[this.ID] = this.StudentManager.PicnicSpots[this.StudentID];
				}
				else if (scheduleBlock2.destination == "MorningRivalWitnessSpot")
				{
					if (!this.MorningRivalWitness)
					{
						this.Destinations[this.ID] = this.StudentManager.MorningRivalWitnessSpots[this.StudentManager.MorningRivalWitnesses];
						this.StudentManager.MorningRivalWitnesses++;
						this.MorningRivalWitness = true;
					}
				}
				else if (scheduleBlock2.destination == "LunchRivalWitnessSpot")
				{
					if (!this.LunchRivalWitness)
					{
						this.Destinations[this.ID] = this.StudentManager.LunchRivalWitnessSpots[this.StudentManager.LunchRivalWitnesses];
						this.StudentManager.LunchRivalWitnesses++;
						this.LunchRivalWitness = true;
					}
				}
				else if (scheduleBlock2.destination == "AfterRivalWitnessSpot" && !this.AfterRivalWitness)
				{
					this.Destinations[this.ID] = this.StudentManager.AfterRivalWitnessSpots[this.StudentManager.AfterRivalWitnesses];
					this.StudentManager.AfterRivalWitnesses++;
					this.AfterRivalWitness = true;
				}
				if (scheduleBlock2.action == "Stand")
				{
					this.Actions[this.ID] = StudentActionType.AtLocker;
				}
				else if (scheduleBlock2.action == "Socialize")
				{
					if (this.StudentID == 26)
					{
						Debug.Log("Somehow, this guy's shit is being set to ''Socializing''?");
					}
					this.Actions[this.ID] = StudentActionType.Socializing;
				}
				else if (scheduleBlock2.action == "Game")
				{
					this.Actions[this.ID] = StudentActionType.Gaming;
				}
				else if (scheduleBlock2.action == "Slave")
				{
					this.Actions[this.ID] = StudentActionType.Slave;
				}
				else if (scheduleBlock2.action == "Relax")
				{
					this.Actions[this.ID] = StudentActionType.Relax;
				}
				else if (scheduleBlock2.action == "Sit")
				{
					this.Actions[this.ID] = StudentActionType.SitAndTakeNotes;
				}
				else if (scheduleBlock2.action == "Peek")
				{
					this.Actions[this.ID] = StudentActionType.Peek;
				}
				else if (scheduleBlock2.action == "SocialSit")
				{
					this.Actions[this.ID] = StudentActionType.SitAndSocialize;
					if (this.Persona == PersonaType.Sleuth && this.Club == ClubType.None)
					{
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
				}
				else if (scheduleBlock2.action == "Eat")
				{
					this.Actions[this.ID] = StudentActionType.SitAndEatBento;
				}
				else if (scheduleBlock2.action == "Shoes")
				{
					this.Actions[this.ID] = StudentActionType.ChangeShoes;
				}
				else if (scheduleBlock2.action == "Grade")
				{
					this.Actions[this.ID] = StudentActionType.GradePapers;
				}
				else if (scheduleBlock2.action == "Patrol")
				{
					this.Actions[this.ID] = StudentActionType.Patrol;
					if (this.OriginalClub != ClubType.None && this.Club == ClubType.None && this.OriginalClub != ClubType.Occult)
					{
						Debug.Log("Student #" + this.StudentID.ToString() + "'s club disbanded, so their action has been set to ''Socialize''.");
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
				}
				else if (scheduleBlock2.action == "Search Patrol")
				{
					this.Actions[this.ID] = StudentActionType.SearchPatrol;
				}
				else if (scheduleBlock2.action == "Gossip")
				{
					this.Actions[this.ID] = StudentActionType.Gossip;
				}
				else if (scheduleBlock2.action == "Graffiti")
				{
					this.Actions[this.ID] = StudentActionType.Graffiti;
				}
				else if (scheduleBlock2.action == "Bully")
				{
					this.Actions[this.ID] = StudentActionType.Bully;
				}
				else if (scheduleBlock2.action == "Read")
				{
					this.Actions[this.ID] = StudentActionType.Read;
				}
				else if (scheduleBlock2.action == "Text")
				{
					this.Actions[this.ID] = StudentActionType.Texting;
				}
				else if (scheduleBlock2.action == "Mourn")
				{
					this.Actions[this.ID] = StudentActionType.Mourn;
				}
				else if (scheduleBlock2.action == "Cuddle")
				{
					this.Actions[this.ID] = StudentActionType.Cuddle;
				}
				else if (scheduleBlock2.action == "Teach")
				{
					this.Actions[this.ID] = StudentActionType.Teaching;
				}
				else if (scheduleBlock2.action == "Wait")
				{
					this.Actions[this.ID] = StudentActionType.Wait;
				}
				else if (scheduleBlock2.action == "Clean")
				{
					this.Actions[this.ID] = StudentActionType.Clean;
				}
				else if (scheduleBlock2.action == "Shamed")
				{
					this.Actions[this.ID] = StudentActionType.Shamed;
				}
				else if (scheduleBlock2.action == "Follow")
				{
					this.Actions[this.ID] = StudentActionType.Follow;
				}
				else if (scheduleBlock2.action == "Sulk")
				{
					this.Actions[this.ID] = StudentActionType.Sulk;
				}
				else if (scheduleBlock2.action == "Sleuth")
				{
					this.Actions[this.ID] = StudentActionType.Sleuth;
				}
				else if (scheduleBlock2.action == "Stalk")
				{
					this.Actions[this.ID] = StudentActionType.Stalk;
				}
				else if (scheduleBlock2.action == "Sketch")
				{
					this.Actions[this.ID] = StudentActionType.Sketch;
				}
				else if (scheduleBlock2.action == "Sunbathe")
				{
					this.Actions[this.ID] = StudentActionType.Sunbathe;
				}
				else if (scheduleBlock2.action == "Shock")
				{
					this.Actions[this.ID] = StudentActionType.Shock;
				}
				else if (scheduleBlock2.action == "Miyuki")
				{
					this.Actions[this.ID] = StudentActionType.Miyuki;
				}
				else if (scheduleBlock2.action == "Meeting")
				{
					this.Actions[this.ID] = StudentActionType.Meeting;
				}
				else if (scheduleBlock2.action == "Lyrics")
				{
					this.Actions[this.ID] = StudentActionType.Lyrics;
				}
				else if (scheduleBlock2.action == "Practice")
				{
					this.Actions[this.ID] = StudentActionType.Practice;
				}
				else if (scheduleBlock2.action == "Sew")
				{
					this.Actions[this.ID] = StudentActionType.Sew;
				}
				else if (scheduleBlock2.action == "Paint")
				{
					this.Actions[this.ID] = StudentActionType.Paint;
				}
				else if (scheduleBlock2.action == "UpdateAppearance")
				{
					this.Actions[this.ID] = StudentActionType.UpdateAppearance;
				}
				else if (scheduleBlock2.action == "LightCig")
				{
					this.Actions[this.ID] = StudentActionType.LightCig;
				}
				else if (scheduleBlock2.action == "PlaceBag")
				{
					this.Actions[this.ID] = StudentActionType.PlaceBag;
				}
				else if (scheduleBlock2.action == "Sleep")
				{
					Debug.Log(this.Name + " is setting action to ''Sleep''.");
					this.Actions[this.ID] = StudentActionType.Sleep;
				}
				else if (scheduleBlock2.action == "LightFire")
				{
					this.Actions[this.ID] = StudentActionType.LightFire;
				}
				else if (scheduleBlock2.action == "Jog")
				{
					this.Actions[this.ID] = StudentActionType.Jog;
				}
				else if (scheduleBlock2.action == "PrepareFood")
				{
					this.Actions[this.ID] = StudentActionType.PrepareFood;
				}
				else if (scheduleBlock2.action == "Perform")
				{
					this.Actions[this.ID] = StudentActionType.Perform;
				}
				else if (scheduleBlock2.action == "PhotoShoot")
				{
					this.Actions[this.ID] = StudentActionType.PhotoShoot;
				}
				else if (scheduleBlock2.action == "GravurePose")
				{
					this.Actions[this.ID] = StudentActionType.GravurePose;
				}
				else if (scheduleBlock2.action == "Guard")
				{
					this.Actions[this.ID] = StudentActionType.Guard;
				}
				else if (scheduleBlock2.action == "Gaming")
				{
					this.Actions[this.ID] = StudentActionType.Gaming;
				}
				else if (scheduleBlock2.action == "Random")
				{
					this.Actions[this.ID] = StudentActionType.Random;
				}
				else if (scheduleBlock2.action == "HelpTeacher")
				{
					this.Actions[this.ID] = StudentActionType.HelpTeacher;
				}
				else if (scheduleBlock2.action == "Admire")
				{
					this.Actions[this.ID] = StudentActionType.Admire;
					this.Infatuated = true;
					if (!this.Stalker)
					{
						if (this.Rival)
						{
							this.InfatuationID = 1;
						}
						else
						{
							this.InfatuationID = this.StudentManager.RivalID;
						}
					}
					if (this.AdmireAnim == "")
					{
						this.AdmireAnim = this.AdmireAnims[0];
					}
				}
				else if (scheduleBlock2.action == "Rehearse")
				{
					this.Actions[this.ID] = StudentActionType.Rehearse;
				}
				else if (scheduleBlock2.action == "Stretch")
				{
					this.Actions[this.ID] = StudentActionType.Stretch;
				}
				else if (scheduleBlock2.action == "Club")
				{
					if (this.Club > ClubType.None)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
					}
					else if (this.OriginalClub == ClubType.Cooking)
					{
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
					else if (this.OriginalClub == ClubType.Drama)
					{
						if (!this.StudentManager.MemorialScene.gameObject.activeInHierarchy)
						{
							Debug.Log("The Drama Club was shut down. They are still being told to perform their club action, though.");
							this.Actions[this.ID] = StudentActionType.ClubAction;
							Debug.Log(string.Concat(new string[]
							{
								this.Name,
								"'s Actions[",
								this.ID.ToString(),
								"] should be: ",
								this.Actions[this.ID].ToString()
							}));
						}
						else
						{
							Debug.Log("The Drama Club was shut down and the gym stage has props on it. Drama students are going to go do something else.");
							this.Actions[this.ID] = StudentActionType.SitAndTakeNotes;
						}
					}
					else if (this.OriginalClub == ClubType.Occult)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
					}
					else if (this.OriginalClub == ClubType.Gaming)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
					}
					else if (this.OriginalClub == ClubType.Art)
					{
						this.Actions[this.ID] = StudentActionType.Sketch;
					}
					else if (this.OriginalClub == ClubType.MartialArts)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
						this.DressCode = false;
					}
					else if (this.OriginalClub == ClubType.LightMusic)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
					}
					else if (this.OriginalClub == ClubType.LightMusic)
					{
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
					else if (this.OriginalClub == ClubType.Science)
					{
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
					else if (this.OriginalClub == ClubType.Sports)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
					}
					else if (this.OriginalClub == ClubType.Gardening)
					{
						this.Actions[this.ID] = StudentActionType.ClubAction;
					}
					else if (this.OriginalClub == ClubType.Newspaper)
					{
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
					else
					{
						Debug.Log(string.Concat(new string[]
						{
							"Somehow, Student#",
							this.StudentID.ToString(),
							" - ",
							this.Name,
							" - reached this part of the code."
						}));
						this.Actions[this.ID] = StudentActionType.Socializing;
					}
				}
				else if (scheduleBlock2.action == "CustomHangout")
				{
					this.Actions[this.ID] = StudentActionType.CustomHangout;
				}
				else if (scheduleBlock2.action == "CustomPatrol")
				{
					this.Actions[this.ID] = StudentActionType.CustomPatrol;
				}
				else if (scheduleBlock2.action == "RandomPatrol")
				{
					this.Actions[this.ID] = StudentActionType.RandomPatrol;
				}
				else if (scheduleBlock2.action == "BakeSale")
				{
					this.Actions[this.ID] = StudentActionType.BakeSale;
				}
				else if (scheduleBlock2.action == "Picnic")
				{
					this.Actions[this.ID] = StudentActionType.Picnic;
				}
				else if (scheduleBlock2.action == "DeskDraw")
				{
					this.Actions[this.ID] = StudentActionType.DeskDraw;
				}
				this.ID++;
			}
		}
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x001FA3FC File Offset: 0x001F85FC
	public void SetOutlineColor(Color NewColor)
	{
		this.ID = 0;
		while (this.ID < this.Outlines.Length)
		{
			if (this.Outlines[this.ID] != null)
			{
				this.Outlines[this.ID].color = NewColor;
				this.Outlines[this.ID].enabled = true;
			}
			this.ID++;
		}
		this.ID = 0;
		while (this.ID < this.RiggedAccessoryOutlines.Length)
		{
			if (this.RiggedAccessoryOutlines[this.ID] != null)
			{
				this.RiggedAccessoryOutlines[this.ID].color = NewColor;
				this.RiggedAccessoryOutlines[this.ID].enabled = this.Outlines[0].enabled;
			}
			this.ID++;
		}
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x001FA4DC File Offset: 0x001F86DC
	public void AddOutlineToHair()
	{
		if (this.Cosmetic.HairRenderer != null)
		{
			if (this.Cosmetic.HairRenderer.GetComponent<OutlineScript>() == null)
			{
				this.Cosmetic.HairRenderer.gameObject.AddComponent<OutlineScript>();
			}
			this.Outlines[1] = this.Cosmetic.HairRenderer.gameObject.GetComponent<OutlineScript>();
			if (this.Outlines[1].h == null)
			{
				this.Outlines[1].Awake();
			}
			this.Outlines[1].color = this.Outlines[0].color;
			this.Outlines[1].enabled = this.Outlines[0].enabled;
			this.Outlines[1].h.enabled = this.Outlines[1].enabled;
		}
		if (this.Teacher && this.StudentManager.Eighties && this.EightiesTeacherAttacher != null && this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer != null)
		{
			this.EightiesTeacherRenderer = this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer;
			if (this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer.gameObject.GetComponent<OutlineScript>() == null)
			{
				this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer.gameObject.AddComponent<OutlineScript>();
			}
			this.MyRenderer = this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer;
			this.Outlines[0] = this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer.gameObject.GetComponent<OutlineScript>();
			this.Outlines[0].color = this.Outlines[1].color;
			if (this.Outlines[0].h == null)
			{
				this.Outlines[0].Awake();
			}
		}
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x001FA6D0 File Offset: 0x001F88D0
	public void PickRandomAnim()
	{
		if (this.Grudge)
		{
			this.RandomAnim = this.BulliedIdleAnim;
			return;
		}
		if (this.Club != ClubType.Delinquent)
		{
			this.RandomAnim = this.AnimationNames[UnityEngine.Random.Range(0, this.AnimationNames.Length)];
			return;
		}
		this.RandomAnim = this.DelinquentAnims[UnityEngine.Random.Range(0, this.DelinquentAnims.Length)];
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x001FA734 File Offset: 0x001F8934
	private void PickRandomGossipAnim()
	{
		if (this.Grudge)
		{
			this.RandomAnim = this.BulliedIdleAnim;
			return;
		}
		this.RandomGossipAnim = this.GossipAnims[UnityEngine.Random.Range(0, this.GossipAnims.Length)];
		if (this.Actions[this.Phase] == StudentActionType.Gossip && this.DistanceToPlayer < 3f)
		{
			if (!ConversationGlobals.GetTopicDiscovered(19))
			{
				this.Yandere.NotificationManager.DisplayNotification(NotificationType.Topic);
				ConversationGlobals.SetTopicDiscovered(19, true);
			}
			if (!this.StudentManager.GetTopicLearnedByStudent(19, this.StudentID))
			{
				this.Yandere.NotificationManager.DisplayNotification(NotificationType.Opinion);
				this.StudentManager.SetTopicLearnedByStudent(19, this.StudentID, true);
			}
		}
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x001FA7EC File Offset: 0x001F89EC
	private void PickRandomSleuthAnim()
	{
		if (!this.Sleuthing)
		{
			this.RandomSleuthAnim = this.SleuthAnims[UnityEngine.Random.Range(0, 3)];
			return;
		}
		this.RandomSleuthAnim = this.SleuthAnims[UnityEngine.Random.Range(3, 6)];
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x001FA820 File Offset: 0x001F8A20
	private void BecomeTeacher()
	{
		base.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
		this.StudentManager.Teachers[this.Class] = this;
		if (this.Class != 1)
		{
			this.GradingPaper = this.StudentManager.FacultyDesks[this.Class];
			if (this.StudentID == 94 && this.StudentManager.Eighties && this.StudentManager.Week > 1)
			{
				this.GradingPaper.enabled = false;
				this.GradingPaper = this.StudentManager.FacultyDesks[1];
			}
			this.GradingPaper.LeftHand = this.LeftHand.parent;
			this.GradingPaper.Character = this.Character;
			this.GradingPaper.Teacher = this;
			this.SkirtCollider.gameObject.SetActive(false);
			this.LowPoly.MyMesh = this.LowPoly.TeacherMesh;
			this.PantyCollider.enabled = false;
		}
		if (this.Class > 1)
		{
			this.VisionDistance = 12f * this.Paranoia;
			base.name = "Teacher_" + this.Class.ToString() + "_" + this.Name;
			this.OriginalIdleAnim = "f02_idleShort_00";
			this.IdleAnim = "f02_idleShort_00";
		}
		else if (this.Class == 1)
		{
			this.VisionDistance = 12f * this.Paranoia;
			this.PatrolAnim = "f02_idle_00";
			base.name = "Nurse_" + this.Name;
			if (this.StudentManager.Eighties)
			{
				this.HeadacheMedicinePrompt.SetActive(true);
			}
		}
		else
		{
			this.VisionDistance = 16f * this.Paranoia;
			this.PatrolAnim = "f02_stretch_00";
			base.name = "Coach_" + this.Name;
			this.OriginalIdleAnim = "f02_tsunIdle_00";
			this.IdleAnim = "f02_tsunIdle_00";
		}
		this.StruggleAnim = "f02_teacherStruggleB_00";
		this.StruggleWonAnim = "f02_teacherStruggleWinB_00";
		this.StruggleLostAnim = "f02_teacherStruggleLoseB_00";
		this.OriginallyTeacher = true;
		this.Spawned = true;
		this.Teacher = true;
		if (this.StudentID > 89 && this.StudentID < 98 && this.StudentManager.Eighties)
		{
			this.SmartPhone = this.EightiesPhone;
			if (this.StudentID != 97 && this.StudentID != 90)
			{
				this.EightiesTeacherAttacher.SetActive(true);
				this.MyRenderer.enabled = false;
			}
		}
		this.SmartPhone.SetActive(false);
		base.gameObject.tag = "Untagged";
		this.MyController.skinWidth = 0.09f;
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x001FAAE4 File Offset: 0x001F8CE4
	public void RemoveShoes()
	{
		if (!this.Male)
		{
			this.MyRenderer.materials[0].mainTexture = this.Cosmetic.SocksTexture;
			this.MyRenderer.materials[1].mainTexture = this.Cosmetic.SocksTexture;
			return;
		}
		this.MyRenderer.materials[this.Cosmetic.UniformID].mainTexture = this.Cosmetic.SocksTexture;
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x001FAB5C File Offset: 0x001F8D5C
	public void BecomeRagdoll()
	{
		Debug.Log(this.Name + " is now becoming a ragdoll.");
		if (this.NewFriend)
		{
			this.Police.EndOfDay.NewFriends--;
		}
		if (this.HeadacheMedicinePrompt != null)
		{
			this.HeadacheMedicinePrompt.GetComponent<GenericEightiesTaskScript>().Disable();
		}
		if (!this.Rival && this.StudentManager.ChallengeManager.RivalsOnly)
		{
			this.StudentManager.ChallengeManager.GameOverTimer = 1f;
		}
		if (this.StudentID == 19)
		{
			this.StudentManager.ZeroAllStalkerSpots();
		}
		if (this.BloodPool != null)
		{
			PromptScript component = this.BloodPool.GetComponent<PromptScript>();
			if (component != null)
			{
				Debug.Log("Re-enabling an object's prompt.");
				component.enabled = true;
			}
		}
		if (this.FollowTarget != null)
		{
			this.FollowTarget.Follower = null;
		}
		this.Meeting = false;
		if (this.Rival)
		{
			this.StudentManager.RivalEliminated = true;
			if (this.Follower != null && this.Follower.FollowTarget != null && this.StudentManager.LastKnownOsana.position == Vector3.zero)
			{
				this.StudentManager.LastKnownOsana.position = base.transform.position;
				this.Follower.Destinations[this.Follower.Phase] = this.StudentManager.LastKnownOsana;
				if (this.Follower.CurrentDestination == this.Follower.FollowTarget)
				{
					this.Follower.Pathfinding.target = this.StudentManager.LastKnownOsana;
					this.Follower.CurrentDestination = this.StudentManager.LastKnownOsana;
				}
			}
		}
		if (this.BikiniAttacher != null && this.BikiniAttacher.newRenderer != null)
		{
			this.BikiniAttacher.newRenderer.updateWhenOffscreen = true;
		}
		if (this.EightiesTeacherAttacher != null && this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer != null)
		{
			this.EightiesTeacherAttacher.GetComponent<RiggedAccessoryAttacher>().newRenderer.updateWhenOffscreen = true;
		}
		if (this.LabcoatAttacher.newRenderer != null)
		{
			this.LabcoatAttacher.newRenderer.updateWhenOffscreen = true;
		}
		if (this.ApronAttacher.newRenderer != null)
		{
			this.ApronAttacher.newRenderer.updateWhenOffscreen = true;
		}
		if (this.Attacher.newRenderer != null)
		{
			this.Attacher.newRenderer.updateWhenOffscreen = true;
		}
		if (this.DrinkingFountain != null)
		{
			this.DrinkingFountain.Occupied = false;
		}
		if (!this.Ragdoll.enabled)
		{
			this.EmptyHands();
			if (this.Broken != null)
			{
				this.Broken.enabled = false;
				this.Broken.MyAudio.Stop();
			}
			if (this.Club == ClubType.Delinquent && this.MyWeapon != null)
			{
				this.MyWeapon.transform.parent = null;
				this.MyWeapon.MyCollider.enabled = true;
				this.MyWeapon.Prompt.enabled = true;
				Rigidbody component2 = this.MyWeapon.GetComponent<Rigidbody>();
				component2.constraints = RigidbodyConstraints.None;
				component2.isKinematic = false;
				component2.useGravity = true;
				this.MyWeapon = null;
			}
			if (this.StudentManager.ChaseCamera == this.ChaseCamera)
			{
				this.StudentManager.ChaseCamera = null;
			}
			this.Countdown.gameObject.SetActive(false);
			this.ChaseCamera.SetActive(false);
			if (this.Club == ClubType.Council)
			{
				this.Police.CouncilDeath = true;
			}
			if (this.WillChase)
			{
				this.Yandere.Chasers--;
			}
			ParticleSystem.EmissionModule emission = this.Hearts.emission;
			if (this.Following)
			{
				emission.enabled = false;
				this.FollowCountdown.gameObject.SetActive(false);
				this.Yandere.Follower = null;
				this.Yandere.Followers--;
				this.Following = false;
			}
			if (this == this.StudentManager.Reporter)
			{
				this.StudentManager.Reporter = null;
			}
			if (this.Pushed)
			{
				this.Police.SuicideStudent = base.gameObject;
				this.Police.SuicideID = this.StudentID;
				this.Police.SuicideScene = true;
				this.Ragdoll.Suicide = true;
				this.Police.Suicide = true;
			}
			if (!this.Tranquil)
			{
				Debug.Log("This part of " + this.Name + "'s code is now updating police numbers.");
				if (!this.Ragdoll.Burning && !this.Ragdoll.Disturbing)
				{
					if (this.Police == null)
					{
						this.Police = this.StudentManager.Police;
					}
					if (this.Police.Corpses < 0)
					{
						this.Police.Corpses = 0;
					}
					if (this.Police.Corpses < this.Police.CorpseList.Length)
					{
						this.Police.CorpseList[this.Police.Corpses] = this.Ragdoll;
					}
					this.Police.Corpses++;
				}
			}
			if (!this.Male)
			{
				this.LiquidProjector.ignoreLayers = -2049;
				this.RightHandCollider.enabled = false;
				this.LeftHandCollider.enabled = false;
				this.PantyCollider.enabled = false;
				this.SkirtCollider.gameObject.SetActive(false);
			}
			this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
			this.Ragdoll.AllColliders[10].isTrigger = false;
			this.NotFaceCollider.enabled = false;
			this.FaceCollider.enabled = false;
			this.MyController.enabled = false;
			emission.enabled = false;
			this.SpeechLines.Stop();
			if (this.MyRenderer.enabled)
			{
				this.MyRenderer.updateWhenOffscreen = true;
			}
			AIDestinationSetter component3 = base.GetComponent<AIDestinationSetter>();
			if (component3 != null)
			{
				component3.enabled = false;
			}
			this.Pathfinding.enabled = false;
			this.HipCollider.enabled = true;
			base.enabled = false;
			this.UnWet();
			this.Prompt.Hide();
			this.Prompt.enabled = false;
			this.Prompt.Hide();
			this.Ragdoll.CharacterAnimation = this.CharacterAnimation;
			this.Ragdoll.DetectionMarker = this.DetectionMarker;
			this.Ragdoll.RightEyeOrigin = this.RightEyeOrigin;
			this.Ragdoll.LeftEyeOrigin = this.LeftEyeOrigin;
			this.Ragdoll.Electrocuted = this.Electrocuted;
			this.Ragdoll.NeckSnapped = this.NeckSnapped;
			this.Ragdoll.BreastSize = this.BreastSize;
			this.Ragdoll.EyeShrink = this.EyeShrink;
			this.Ragdoll.StudentID = this.StudentID;
			this.Ragdoll.Tranquil = this.Tranquil;
			this.Ragdoll.Burning = this.Burning;
			this.Ragdoll.Drowned = this.Drowned;
			this.Ragdoll.Yandere = this.Yandere;
			this.Ragdoll.Police = this.Police;
			this.Ragdoll.Pushed = this.Pushed;
			this.Ragdoll.Male = this.Male;
			if (!this.Tranquil)
			{
				this.Police.Deaths++;
			}
			if (!this.NoRagdoll)
			{
				this.Ragdoll.enabled = true;
			}
			if (this.Reputation == null)
			{
				this.Reputation = this.StudentManager.Reputation;
			}
			this.Reputation.PendingRep -= this.PendingRep;
			if (this.WitnessedMurder && this.Persona != PersonaType.Evil)
			{
				this.Police.Witnesses--;
			}
			this.SetOutlineColor(new Color(1f, 0.5f, 0f, 1f));
			if (this.DetectionMarker != null)
			{
				this.DetectionMarker.Tex.enabled = false;
			}
			GameObjectUtils.SetLayerRecursively(base.gameObject, 11);
			this.MapMarker.gameObject.layer = 10;
			base.tag = "Blood";
			this.LowPoly.transform.parent = this.Hips;
			this.LowPoly.transform.localPosition = new Vector3(0f, -1f, 0f);
			this.LowPoly.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		if (this.SmartPhone.transform.parent == this.ItemParent)
		{
			this.SmartPhone.SetActive(false);
		}
		if (this.Yandere.Pursuer == this)
		{
			this.Yandere.Pursuer = null;
		}
		this.InEvent = false;
		this.Grudge = false;
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x001FB4B0 File Offset: 0x001F96B0
	public void GetWet()
	{
		if (this.InvestigatingBloodPool)
		{
			this.ForgetAboutBloodPool();
		}
		if ((SchemeGlobals.CurrentScheme == 1 && SchemeGlobals.GetSchemeStage(1) < 4 && this.Rival) || (SchemeGlobals.CurrentScheme == 2 && SchemeGlobals.GetSchemeStage(2) < 4 && this.StudentID == 2))
		{
			Debug.Log("A scheme-related character was just splashed with water.");
			SchemeGlobals.SetSchemeStage(SchemeGlobals.CurrentScheme, 4);
			this.Yandere.PauseScreen.Schemes.UpdateInstructions();
		}
		this.TargetDistance = 1f;
		this.FocusOnStudent = false;
		this.FocusOnYandere = false;
		this.BeenSplashed = true;
		this.BatheFast = true;
		this.LiquidProjector.gameObject.SetActive(true);
		this.LiquidProjector.enabled = true;
		this.Emetic = false;
		this.Sedated = false;
		this.Headache = false;
		this.Vomiting = false;
		this.DressCode = false;
		this.Reacted = false;
		this.Alarmed = false;
		if (this.Gas)
		{
			this.LiquidProjector.material = this.GasMaterial;
		}
		else if (this.Bloody)
		{
			this.LiquidProjector.material = this.BloodMaterial;
		}
		else if (this.DyedBrown)
		{
			this.LiquidProjector.material = this.BrownMaterial;
			if (!this.StudentManager.Eighties && this.StudentID == 5)
			{
				this.HorudaCollider.gameObject.SetActive(true);
			}
		}
		else
		{
			this.LiquidProjector.material = this.WaterMaterial;
		}
		this.ID = 0;
		while (this.ID < this.LiquidEmitters.Length)
		{
			ParticleSystem particleSystem = this.LiquidEmitters[this.ID];
			particleSystem.gameObject.SetActive(true);
			ParticleSystem.MainModule main = particleSystem.main;
			if (this.Gas)
			{
				main.startColor = new Color(1f, 1f, 0f, 1f);
			}
			else if (this.Bloody)
			{
				main.startColor = new Color(1f, 0f, 0f, 1f);
			}
			else if (this.DyedBrown)
			{
				main.startColor = new Color(0.5f, 0.25f, 0f, 1f);
			}
			else
			{
				main.startColor = new Color(0f, 1f, 1f, 1f);
			}
			this.ID++;
		}
		if (!this.Slave)
		{
			this.CharacterAnimation[this.SplashedAnim].speed = 1f;
			this.CharacterAnimation.CrossFade(this.SplashedAnim);
			this.Subtitle.UpdateLabel(this.SplashSubtitleType, 0, 1f);
			this.SpeechLines.Stop();
			this.Hearts.Stop();
			this.StopMeeting();
			this.Pathfinding.canSearch = false;
			this.Pathfinding.canMove = false;
			this.SplashTimer = 0f;
			this.SplashPhase = 1;
			this.BathePhase = 1;
			this.ForgetRadio();
			if (this.Distracting)
			{
				this.DistractionTarget.TargetedForDistraction = false;
				this.DistractionTarget.Octodog.SetActive(false);
				this.DistractionTarget.Distracted = false;
				this.Distracting = false;
				this.CanTalk = true;
			}
			if (this.Investigating)
			{
				this.Investigating = false;
			}
			this.SchoolwearUnavailable = true;
			this.SentToLocker = false;
			this.Distracted = true;
			this.Splashed = true;
			this.Routine = false;
			this.GoAway = false;
			this.Wet = true;
			if (this.Following)
			{
				this.FollowCountdown.gameObject.SetActive(false);
				this.Yandere.Follower = null;
				this.Yandere.Followers--;
				this.Following = false;
			}
			this.SpawnAlarmDisc();
			if (this.Club == ClubType.Cooking)
			{
				this.IdleAnim = this.OriginalIdleAnim;
				this.WalkAnim = this.OriginalWalkAnim;
				this.LeanAnim = this.OriginalLeanAnim;
				this.ClubActivityPhase = 0;
				this.ClubTimer = 0f;
			}
			if (this.ReturningMisplacedWeapon)
			{
				this.DropMisplacedWeapon();
			}
			this.EmptyHands();
		}
		this.Alarm = 0f;
		this.UpdateDetectionMarker();
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x001FB8F0 File Offset: 0x001F9AF0
	public void GetWetNoConsequences()
	{
		this.LiquidProjector.gameObject.SetActive(true);
		this.LiquidProjector.enabled = true;
		if (this.Gas)
		{
			this.LiquidProjector.material = this.GasMaterial;
		}
		else if (this.Bloody)
		{
			this.LiquidProjector.material = this.BloodMaterial;
		}
		else if (this.DyedBrown)
		{
			this.LiquidProjector.material = this.BrownMaterial;
		}
		else
		{
			this.LiquidProjector.material = this.WaterMaterial;
		}
		this.ID = 0;
		while (this.ID < this.LiquidEmitters.Length)
		{
			ParticleSystem particleSystem = this.LiquidEmitters[this.ID];
			particleSystem.gameObject.SetActive(true);
			ParticleSystem.MainModule main = particleSystem.main;
			if (this.Gas)
			{
				main.startColor = new Color(1f, 1f, 0f, 1f);
			}
			else if (this.Bloody)
			{
				main.startColor = new Color(1f, 0f, 0f, 1f);
			}
			else if (this.DyedBrown)
			{
				main.startColor = new Color(0.5f, 0.25f, 0f, 1f);
			}
			else
			{
				main.startColor = new Color(0f, 1f, 1f, 1f);
			}
			this.ID++;
		}
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x001FBA80 File Offset: 0x001F9C80
	public void UnWet()
	{
		this.ID = 0;
		while (this.ID < this.LiquidEmitters.Length)
		{
			this.LiquidEmitters[this.ID].gameObject.SetActive(false);
			this.ID++;
		}
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x001FBACC File Offset: 0x001F9CCC
	public void SetSplashes(bool Bool)
	{
		this.ID = 0;
		while (this.ID < this.SplashEmitters.Length)
		{
			this.SplashEmitters[this.ID].gameObject.SetActive(Bool);
			this.ID++;
		}
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x001FBB18 File Offset: 0x001F9D18
	public void StopMeeting()
	{
		this.Prompt.Label[0].text = "     Talk";
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.DistanceToDestination = 100f;
		this.Drownable = false;
		this.Pushable = false;
		this.Meeting = false;
		this.CanTalk = true;
		this.StudentManager.UpdateMe(this.StudentID);
		this.MeetTimer = 0f;
		this.RemoveOfferHelpPrompt();
		if (this.Rival)
		{
			this.StudentManager.UpdateInfatuatedTargetDistances();
		}
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x001FBBB0 File Offset: 0x001F9DB0
	public void RemoveOfferHelpPrompt()
	{
		OfferHelpScript offerHelpScript = null;
		if (this.StudentManager.Eighties && this.StudentID == this.StudentManager.RivalID)
		{
			offerHelpScript = this.StudentManager.EightiesOfferHelp;
			this.StudentManager.LoveManager.RivalWaiting = false;
		}
		else if (this.StudentID == this.StudentManager.RivalID)
		{
			offerHelpScript = this.StudentManager.OsanaOfferHelp;
			this.StudentManager.LoveManager.RivalWaiting = false;
		}
		else if (this.StudentID == 30)
		{
			offerHelpScript = this.StudentManager.OfferHelp;
			this.StudentManager.LoveManager.RivalWaiting = false;
		}
		else if (this.StudentID == 5)
		{
			offerHelpScript = this.StudentManager.FragileOfferHelp;
		}
		if (offerHelpScript != null)
		{
			offerHelpScript.transform.position = Vector3.zero;
			offerHelpScript.enabled = false;
			offerHelpScript.Prompt.Hide();
			offerHelpScript.Prompt.enabled = false;
		}
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x001FBCA8 File Offset: 0x001F9EA8
	public void Combust()
	{
		this.Police.CorpseList[this.Police.Corpses] = this.Ragdoll;
		this.Police.Corpses++;
		GameObjectUtils.SetLayerRecursively(base.gameObject, 11);
		this.MapMarker.gameObject.layer = 10;
		base.tag = "Blood";
		this.Dying = true;
		this.CharacterAnimation.CrossFade(this.BurningAnim);
		this.CharacterAnimation[this.WetAnim].weight = 0f;
		this.Pathfinding.canSearch = false;
		this.Pathfinding.canMove = false;
		this.Ragdoll.BurningAnimation = true;
		this.Ragdoll.Disturbing = true;
		this.Ragdoll.Burning = true;
		this.WitnessedCorpse = false;
		this.FocusOnStudent = false;
		this.FocusOnYandere = false;
		this.Investigating = false;
		this.EatingSnack = false;
		this.DiscCheck = false;
		this.WalkBack = false;
		this.Alarmed = false;
		this.CanTalk = false;
		this.Fleeing = false;
		this.Routine = false;
		this.Reacted = false;
		this.Burning = true;
		this.Wet = false;
		this.SpawnAlarmDisc();
		if (!this.NoScream)
		{
			AudioSource component = base.GetComponent<AudioSource>();
			component.clip = this.BurningClip;
			component.Play();
		}
		this.LiquidProjector.enabled = false;
		this.UnWet();
		if (this.Following)
		{
			this.FollowCountdown.gameObject.SetActive(false);
			this.Yandere.Follower = null;
			this.Yandere.Followers--;
			this.Following = false;
		}
		this.ID = 0;
		while (this.ID < this.FireEmitters.Length)
		{
			this.FireEmitters[this.ID].gameObject.SetActive(true);
			this.ID++;
		}
		if (this.Attacked)
		{
			this.BurnTarget = this.Yandere.transform.position + this.Yandere.transform.forward;
			this.Attacked = false;
		}
		if (!this.Male)
		{
			this.HorudaCollider.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x001FBEE8 File Offset: 0x001FA0E8
	public void JojoReact()
	{
		UnityEngine.Object.Instantiate<GameObject>(this.JojoHitEffect, base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
		if (!this.Dying)
		{
			this.Dying = true;
			this.SpawnAlarmDisc();
			this.CharacterAnimation.CrossFade(this.JojoReactAnim);
			this.CharacterAnimation[this.WetAnim].weight = 0f;
			this.Pathfinding.canSearch = false;
			this.Pathfinding.canMove = false;
			this.WitnessedCorpse = false;
			this.Investigating = false;
			this.EatingSnack = false;
			this.DiscCheck = false;
			this.WalkBack = false;
			this.Alarmed = false;
			this.CanTalk = false;
			this.Fleeing = false;
			this.Routine = false;
			this.Reacted = false;
			this.Wet = false;
			base.GetComponent<AudioSource>().Play();
			if (this.Following)
			{
				this.FollowCountdown.gameObject.SetActive(false);
				this.Yandere.Follower = null;
				this.Yandere.Followers--;
				this.Following = false;
			}
		}
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x001FC020 File Offset: 0x001FA220
	private void Nude()
	{
		if (!this.Male)
		{
			this.PantyCollider.enabled = false;
			this.SkirtCollider.gameObject.SetActive(false);
		}
		if (!this.Male)
		{
			this.MyRenderer.sharedMesh = this.TowelMesh;
			if (this.Club == ClubType.Bully)
			{
				this.Cosmetic.DeactivateBullyAccessories();
			}
			this.MyRenderer.materials[0].SetFloat("_BlendAmount", 0f);
			this.MyRenderer.materials[0].mainTexture = this.TowelTexture;
			this.MyRenderer.materials[1].mainTexture = this.TowelTexture;
			if (this.MyRenderer.materials.Length > 2)
			{
				this.MyRenderer.materials[2].mainTexture = this.Cosmetic.FaceTexture;
			}
			this.Cosmetic.MyRenderer.materials[1].SetFloat("_BlendAmount", 0f);
		}
		else
		{
			this.MyRenderer.sharedMesh = this.BaldNudeMesh;
			this.MyRenderer.materials[0].mainTexture = this.NudeTexture;
			this.MyRenderer.materials[1].mainTexture = null;
			this.MyRenderer.materials[2].mainTexture = this.Cosmetic.FaceTextures[this.SkinColor];
		}
		this.Cosmetic.RemoveCensor();
		if (!this.AoT)
		{
			if (this.Male)
			{
				this.ID = 0;
				while (this.ID < this.CensorSteam.Length)
				{
					this.CensorSteam[this.ID].SetActive(true);
					this.ID++;
				}
			}
		}
		else if (!this.Male)
		{
			this.MyRenderer.sharedMesh = this.BaldNudeMesh;
			this.MyRenderer.materials[0].mainTexture = this.NudeTexture;
			this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
			if (this.MyRenderer.materials.Length > 2)
			{
				this.MyRenderer.materials[2].mainTexture = this.NudeTexture;
			}
		}
		else
		{
			this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTextures[this.SkinColor];
		}
		if (this.Club == ClubType.Cooking)
		{
			this.ApronAttacher.newRenderer.gameObject.SetActive(false);
			Debug.Log("We were told to disable this apron attacher...");
		}
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x001FC2A4 File Offset: 0x001FA4A4
	public void ChangeSchoolwear()
	{
		this.ID = 0;
		while (this.ID < this.CensorSteam.Length)
		{
			this.CensorSteam[this.ID].SetActive(false);
			this.ID++;
		}
		if (this.Attacher.gameObject.activeInHierarchy)
		{
			this.Attacher.RemoveAccessory();
		}
		if (this.LabcoatAttacher.enabled)
		{
			UnityEngine.Object.Destroy(this.LabcoatAttacher.newRenderer);
			this.LabcoatAttacher.enabled = false;
		}
		if (!this.Male && this.BikiniAttacher.enabled)
		{
			Debug.Log("Destroying Bikini's newRenderer, re-enabling MyRenderer.");
			UnityEngine.Object.Destroy(this.BikiniAttacher.newRenderer);
			this.BikiniAttacher.enabled = false;
			this.MyRenderer.enabled = true;
		}
		if (this.Schoolwear == 0)
		{
			this.Nude();
		}
		else if (this.Schoolwear == 1)
		{
			if (!this.Male)
			{
				this.Cosmetic.SetFemaleUniform();
				this.SkirtCollider.gameObject.SetActive(true);
				if (this.PantyCollider != null)
				{
					this.PantyCollider.enabled = true;
				}
				if (this.Club == ClubType.Bully)
				{
					if (this.Cosmetic.FemaleUniformID < 2 || this.Cosmetic.FemaleUniformID == 3)
					{
						this.Cosmetic.RightWristband.SetActive(true);
						this.Cosmetic.LeftWristband.SetActive(true);
					}
					else
					{
						this.Cosmetic.RightWristband.SetActive(false);
						this.Cosmetic.LeftWristband.SetActive(false);
					}
					this.Cosmetic.Bookbag.SetActive(true);
					this.Cosmetic.Hoodie.SetActive(true);
				}
			}
			else
			{
				this.Cosmetic.SetMaleUniform();
			}
		}
		else if (this.Schoolwear == 2)
		{
			if (this.Club == ClubType.Sports && this.Male)
			{
				this.MyRenderer.sharedMesh = this.SwimmingTrunks;
				this.MyRenderer.SetBlendShapeWeight(0, (float)(20 * (6 - this.ClubMemberID)));
				this.MyRenderer.SetBlendShapeWeight(1, (float)(20 * (6 - this.ClubMemberID)));
				this.MyRenderer.materials[0].mainTexture = this.Cosmetic.Trunks[this.StudentID];
				this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
				this.MyRenderer.materials[2].mainTexture = this.Cosmetic.Trunks[this.StudentID];
			}
			else
			{
				this.MyRenderer.sharedMesh = this.SchoolSwimsuit;
				if (!this.Male)
				{
					if (this.Club == ClubType.Bully)
					{
						this.MyRenderer.materials[0].mainTexture = this.Cosmetic.GanguroSwimsuitTextures[this.BullyID];
						this.MyRenderer.materials[1].mainTexture = this.Cosmetic.GanguroSwimsuitTextures[this.BullyID];
						this.Cosmetic.RightWristband.SetActive(false);
						this.Cosmetic.LeftWristband.SetActive(false);
						this.Cosmetic.Bookbag.SetActive(false);
						this.Cosmetic.Hoodie.SetActive(false);
					}
					else
					{
						this.MyRenderer.materials[0].mainTexture = this.SwimsuitTexture;
						this.MyRenderer.materials[1].mainTexture = this.SwimsuitTexture;
					}
					this.MyRenderer.materials[2].mainTexture = this.Cosmetic.FaceTexture;
					this.MyRenderer.materials[0].SetFloat("_BlendAmount", 0f);
					this.MyRenderer.materials[1].SetFloat("_BlendAmount", 0f);
					this.MyRenderer.materials[0].SetFloat("_BlendAmount1", 0f);
					this.MyRenderer.materials[1].SetFloat("_BlendAmount1", 0f);
				}
				else
				{
					this.MyRenderer.materials[0].mainTexture = this.SwimsuitTexture;
					this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
					this.MyRenderer.materials[2].mainTexture = this.SwimsuitTexture;
				}
			}
		}
		else if (this.Schoolwear == 3)
		{
			this.MyRenderer.sharedMesh = this.GymUniform;
			if (this.StudentManager.Eighties)
			{
				this.GymTexture = this.EightiesGymTexture;
			}
			if (!this.Male)
			{
				this.MyRenderer.materials[0].mainTexture = this.GymTexture;
				this.MyRenderer.materials[1].mainTexture = this.GymTexture;
				this.MyRenderer.materials[2].mainTexture = this.Cosmetic.FaceTexture;
				if (this.Club == ClubType.Bully)
				{
					this.Cosmetic.ActivateBullyAccessories();
				}
			}
			else
			{
				Debug.Log(this.Name + ", a male, is putting on a gym uniform.");
				this.MyRenderer.materials[0].mainTexture = this.GymTexture;
				this.MyRenderer.materials[2].mainTexture = this.Cosmetic.SkinTextures[this.Cosmetic.SkinColor];
				this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
			}
		}
		if (!this.Male)
		{
			this.Cosmetic.Stockings = ((this.Schoolwear == 1) ? this.Cosmetic.OriginalStockings : string.Empty);
			base.StartCoroutine(this.Cosmetic.PutOnStockings());
			if (GameGlobals.CensorPanties)
			{
				this.Cosmetic.CensorPanties();
			}
		}
		while (this.ID < this.Outlines.Length)
		{
			if (this.Outlines[this.ID] != null && this.Outlines[this.ID].h != null)
			{
				this.Outlines[this.ID].h.ReinitMaterials();
			}
			this.ID++;
		}
		if (this.Club == ClubType.Cooking)
		{
			if (this.Schoolwear == 1)
			{
				this.ApronAttacher.newRenderer.gameObject.SetActive(true);
			}
			else
			{
				this.ApronAttacher.newRenderer.gameObject.SetActive(false);
			}
		}
		if (this.Slave)
		{
			Debug.Log("Huh? A mind-broken slave got to this code?");
		}
		if (this.Club == ClubType.LightMusic)
		{
			if (this.Schoolwear == 1)
			{
				this.InstrumentBag[this.ClubMemberID].gameObject.SetActive(true);
			}
			else
			{
				this.InstrumentBag[this.ClubMemberID].gameObject.SetActive(false);
			}
		}
		this.WalkAnim = this.OriginalWalkAnim;
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x001FC990 File Offset: 0x001FAB90
	public void AttackOnTitan()
	{
		this.CharacterAnimation.CrossFade(this.WalkAnim);
		this.Nape.enabled = true;
		this.Blind = true;
		this.Hurry = true;
		this.AoT = true;
		this.TargetDistance = 5f;
		this.SprintAnim = this.WalkAnim;
		this.RunAnim = this.WalkAnim;
		this.MyController.center = new Vector3(this.MyController.center.x, 0.0825f, this.MyController.center.z);
		this.MyController.radius = 0.015f;
		this.MyController.height = 0.15f;
		if (!this.Male)
		{
			this.Cosmetic.FaceTexture = this.TitanFaceTexture;
		}
		else
		{
			this.Cosmetic.FaceTextures[this.SkinColor] = this.TitanFaceTexture;
		}
		this.NudeTexture = this.TitanBodyTexture;
		this.Nude();
		this.ID = 0;
		while (this.ID < this.Outlines.Length)
		{
			if (this.Outlines[this.ID] != null)
			{
				OutlineScript outlineScript = this.Outlines[this.ID];
				if (outlineScript.h == null)
				{
					outlineScript.Awake();
				}
				outlineScript.h.ReinitMaterials();
			}
			this.ID++;
		}
		if (!this.Male && !this.Teacher)
		{
			this.PantyCollider.enabled = false;
			this.SkirtCollider.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x001FCB24 File Offset: 0x001FAD24
	public void Spook()
	{
		if (!this.Male)
		{
			this.RightEye.gameObject.SetActive(false);
			this.LeftEye.gameObject.SetActive(false);
			this.MyRenderer.enabled = false;
			this.ID = 0;
			while (this.ID < this.Bones.Length)
			{
				this.Bones[this.ID].SetActive(true);
				this.ID++;
			}
		}
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x001FCBA4 File Offset: 0x001FADA4
	private void Unspook()
	{
		this.MyRenderer.enabled = true;
		this.ID = 0;
		while (this.ID < this.Bones.Length)
		{
			this.Bones[this.ID].SetActive(false);
			this.ID++;
		}
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x001FCBF8 File Offset: 0x001FADF8
	public void GoChange()
	{
		if (!this.Male)
		{
			this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
			this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
		}
		else
		{
			this.CurrentDestination = this.StudentManager.MaleStripSpot;
			this.Pathfinding.target = this.StudentManager.MaleStripSpot;
		}
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		if (!this.SchoolwearUnavailable && !this.BeenSplashed)
		{
			this.MustChangeClothing = true;
		}
		else
		{
			Debug.Log(this.Name + " should not try to change clothing later on this day.");
		}
		this.Distracted = false;
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x001FCCB8 File Offset: 0x001FAEB8
	public void SpawnAlarmDisc()
	{
		Debug.Log(base.name + " is now spawning an Alarm Disc.");
		AlarmDiscScript component = UnityEngine.Object.Instantiate<GameObject>(this.AlarmDisc, base.transform.position + Vector3.up, Quaternion.identity).GetComponent<AlarmDiscScript>();
		component.Male = this.Male;
		component.Originator = this;
		if (this.Splashed)
		{
			component.Shocking = true;
			component.NoScream = true;
		}
		if (this.Struggling || this.Shoving || this.MurderSuicidePhase == 100 || this.StudentManager.CombatMinigame.Delinquent == this)
		{
			component.NoScream = true;
		}
		if (this.Pushed)
		{
			component.Silent = true;
		}
		if (this.Club == ClubType.Delinquent)
		{
			component.Delinquent = true;
		}
		if ((this.Yandere.RoofPush && this.Yandere.TargetStudent) || this.Burning)
		{
			Debug.Log("Alarming death! Alarm Disc should be LOUD!");
			component.Loud = true;
		}
		if (this.Dying && this.Yandere.Equipped > 0 && this.Yandere.EquippedWeapon.WeaponID == 7)
		{
			component.Long = true;
		}
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x001FCDF0 File Offset: 0x001FAFF0
	public void SpawnSmallAlarmDisc()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.AlarmDisc, base.transform.position + Vector3.up, Quaternion.identity);
		gameObject.transform.localScale = new Vector3(100f, 1f, 100f);
		gameObject.GetComponent<AlarmDiscScript>().NoScream = true;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x001FCE4C File Offset: 0x001FB04C
	public void ChangeClubwear()
	{
		if (!this.ClubAttire)
		{
			this.Cosmetic.RemoveCensor();
			this.DistanceToDestination = 100f;
			this.ClubAttire = true;
			if (this.Club == ClubType.Art)
			{
				if (!this.Male)
				{
					this.RightBreast.gameObject.name = "RightBreastRENAMED";
					this.LeftBreast.gameObject.name = "LeftBreastRENAMED";
				}
				if (!this.Attacher.gameObject.activeInHierarchy)
				{
					this.Attacher.gameObject.SetActive(true);
					return;
				}
				this.Attacher.Start();
				return;
			}
			else if (this.Club == ClubType.MartialArts)
			{
				this.MyRenderer.sharedMesh = this.JudoGiMesh;
				if (!this.Male)
				{
					this.MyRenderer.materials[0].mainTexture = this.Cosmetic.FaceTexture;
					this.MyRenderer.materials[1].mainTexture = this.JudoGiTexture;
					this.MyRenderer.materials[2].mainTexture = this.JudoGiTexture;
					this.SkirtCollider.gameObject.SetActive(false);
					this.PantyCollider.enabled = false;
					this.MyRenderer.materials[0].SetFloat("_BlendAmount", 0f);
				}
				else
				{
					this.MyRenderer.materials[0].mainTexture = this.JudoGiTexture;
					this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
					this.MyRenderer.materials[2].mainTexture = this.JudoGiTexture;
				}
				if (this.Armband != null)
				{
					this.Armband.transform.localPosition = new Vector3(-0.1f, -0.019f, 0.01f);
					this.Armband.transform.localScale = new Vector3(1.33333f, 1.33333f, 1.33333f);
					return;
				}
			}
			else
			{
				if (this.Club == ClubType.Science)
				{
					this.WearLabCoat();
					return;
				}
				if (this.OriginalClub == ClubType.Sports)
				{
					if (this.Clock.Period >= 3 && !this.StudentManager.PoolClosed)
					{
						if (this.Armband != null)
						{
							this.Armband.transform.localPosition = new Vector3(-0.1f, -0.01f, 0f);
							Physics.SyncTransforms();
						}
						this.MyRenderer.sharedMesh = this.SwimmingTrunks;
						if (this.Male)
						{
							this.MyRenderer.sharedMesh = this.SwimmingTrunks;
							this.MyRenderer.SetBlendShapeWeight(0, (float)(20 * (6 - this.ClubMemberID)));
							this.MyRenderer.materials[0].mainTexture = this.Cosmetic.Trunks[this.StudentID];
							this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
							this.MyRenderer.materials[2].mainTexture = this.Cosmetic.Trunks[this.StudentID];
						}
						else
						{
							Debug.Log("A female student is now changing into a swimsuit.");
							this.MyRenderer.sharedMesh = this.SchoolSwimsuit;
							this.MyRenderer.materials[0].mainTexture = this.SwimsuitTexture;
							this.MyRenderer.materials[1].mainTexture = this.SwimsuitTexture;
							this.MyRenderer.materials[2].mainTexture = this.Cosmetic.FaceTexture;
							this.MyRenderer.materials[0].SetFloat("_BlendAmount", 0f);
							this.MyRenderer.materials[1].SetFloat("_BlendAmount", 0f);
							this.MyRenderer.materials[0].SetFloat("_BlendAmount1", 0f);
							this.MyRenderer.materials[1].SetFloat("_BlendAmount1", 0f);
						}
						this.ClubAnim = this.GenderPrefix + "poolDive_00";
						this.ClubActivityPhase = 15;
						this.Destinations[this.Phase] = this.StudentManager.Clubs.List[this.StudentID].GetChild(this.ClubActivityPhase);
						return;
					}
					if (this.StudentManager.Eighties)
					{
						this.GymTexture = this.EightiesGymTexture;
					}
					this.MyRenderer.sharedMesh = this.GymUniform;
					if (this.Male)
					{
						this.MyRenderer.materials[0].mainTexture = this.GymTexture;
						this.MyRenderer.materials[2].mainTexture = this.Cosmetic.SkinTextures[this.Cosmetic.SkinID];
						this.MyRenderer.materials[1].mainTexture = this.Cosmetic.FaceTexture;
					}
					else
					{
						this.MyRenderer.materials[0].mainTexture = this.GymTexture;
						this.MyRenderer.materials[1].mainTexture = this.GymTexture;
						this.MyRenderer.materials[2].mainTexture = this.Cosmetic.FaceTexture;
					}
					if (this.Armband != null)
					{
						this.Armband.transform.localPosition = new Vector3(-0.1f, 0f, -0.005f);
						Physics.SyncTransforms();
						return;
					}
				}
			}
		}
		else
		{
			this.ClubAttire = false;
			if (this.Club == ClubType.Art)
			{
				this.Attacher.RemoveAccessory();
				return;
			}
			if (this.Club == ClubType.Science)
			{
				this.WearLabCoat();
				return;
			}
			this.ChangeSchoolwear();
			if (this.StudentID == 46)
			{
				this.Armband.transform.localPosition = new Vector3(-0.1f, 0f, 0f);
				this.Armband.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
				return;
			}
			if (this.StudentID == 47 || this.StudentID == 49)
			{
				this.StudentManager.ConvoManager.BothCharactersInPosition = false;
				if (this.Male)
				{
					this.ClubAnim = "idle_20";
					return;
				}
				this.ClubAnim = "f02_idle_20";
			}
		}
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x001FD470 File Offset: 0x001FB670
	private void WearLabCoat()
	{
		if (this.LabcoatAttacher.enabled)
		{
			if (!this.Male)
			{
				this.RightBreast.gameObject.name = "RightBreastRENAMED";
				this.LeftBreast.gameObject.name = "LeftBreastRENAMED";
				this.SkirtCollider.gameObject.SetActive(true);
				this.PantyCollider.enabled = true;
			}
			UnityEngine.Object.Destroy(this.LabcoatAttacher.newRenderer);
			this.LabcoatAttacher.enabled = false;
			this.ChangeSchoolwear();
			return;
		}
		this.MyRenderer.sharedMesh = this.HeadAndHands;
		this.LabcoatAttacher.enabled = true;
		if (!this.Male)
		{
			this.RightBreast.gameObject.name = "RightBreastRENAMED";
			this.LeftBreast.gameObject.name = "LeftBreastRENAMED";
		}
		if (this.LabcoatAttacher.Initialized)
		{
			this.LabcoatAttacher.AttachAccessory();
		}
		if (!this.Male)
		{
			this.MyRenderer.materials[0].mainTexture = this.Cosmetic.FaceTexture;
			this.MyRenderer.materials[1].mainTexture = this.NudeTexture;
			this.MyRenderer.materials[2].mainTexture = null;
			this.HideLabCoatPanties();
			return;
		}
		this.MyRenderer.materials[0].mainTexture = this.Cosmetic.FaceTextures[this.SkinColor];
		this.MyRenderer.materials[1].mainTexture = this.NudeTexture;
		this.MyRenderer.materials[2].mainTexture = this.NudeTexture;
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x001FD614 File Offset: 0x001FB814
	public void HideLabCoatPanties()
	{
		this.MyRenderer.materials[0].SetFloat("_BlendAmount", 0f);
		this.MyRenderer.materials[1].SetFloat("_BlendAmount", 0f);
		this.SkirtCollider.gameObject.SetActive(false);
		this.PantyCollider.enabled = false;
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x001FD678 File Offset: 0x001FB878
	public void WearBikini()
	{
		if (!this.BikiniAttacher.enabled)
		{
			Debug.Log("Putting bikini on now.");
			this.WearingBikini = true;
			this.BikiniAttacher.enabled = true;
			this.MyRenderer.enabled = false;
			this.RightBreast.gameObject.name = "RightBreastRENAMED";
			this.LeftBreast.gameObject.name = "LeftBreastRENAMED";
			if (this.BikiniAttacher.Initialized)
			{
				this.BikiniAttacher.AttachAccessory();
			}
			this.Cosmetic.MyRenderer.materials[1].SetFloat("_BlendAmount", 0f);
			this.SkirtCollider.gameObject.SetActive(false);
			this.PantyCollider.enabled = false;
			return;
		}
		Debug.Log("Removing bikini now.");
		this.WearingBikini = false;
		this.MyRenderer.enabled = true;
		this.RightBreast.gameObject.name = "RightBreastRENAMED";
		this.LeftBreast.gameObject.name = "LeftBreastRENAMED";
		this.SkirtCollider.gameObject.SetActive(true);
		this.PantyCollider.enabled = true;
		UnityEngine.Object.Destroy(this.BikiniAttacher.newRenderer);
		this.BikiniAttacher.enabled = false;
		this.ChangeSchoolwear();
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x001FD7C8 File Offset: 0x001FB9C8
	public void AttachRiggedAccessory()
	{
		this.RiggedAccessory.GetComponent<RiggedAccessoryAttacher>().ID = this.StudentID;
		if (this.Cosmetic.Accessory > 0)
		{
			this.Cosmetic.FemaleAccessories[this.Cosmetic.Accessory].SetActive(false);
		}
		if (this.StudentID == 26)
		{
			this.MyRenderer.sharedMesh = this.NoArmsNoTorso;
		}
		this.RiggedAccessory.SetActive(true);
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x001FD840 File Offset: 0x001FBA40
	public void CameraReact()
	{
		Debug.Log(this.Name + " just fired CameraReact()");
		this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
		this.Pathfinding.canSearch = false;
		this.Pathfinding.canMove = false;
		this.Obstacle.enabled = true;
		this.CameraReacting = true;
		this.CameraReactPhase = 1;
		this.SpeechLines.Stop();
		this.Routine = false;
		this.StopPairing();
		if (!this.Sleuthing)
		{
			this.SmartPhone.SetActive(false);
		}
		this.OccultBook.SetActive(false);
		this.Scrubber.SetActive(false);
		this.Eraser.SetActive(false);
		this.Pen.SetActive(false);
		this.Pencil.SetActive(false);
		this.Sketchbook.SetActive(false);
		if (this.Club == ClubType.Gardening)
		{
			if (!this.StudentManager.Eighties || this.WaterLow)
			{
				this.WateringCan.transform.parent = this.Hips;
				this.WateringCan.transform.localPosition = new Vector3(0f, 0.0135f, -0.184f);
				this.WateringCan.transform.localEulerAngles = new Vector3(0f, 90f, 30f);
			}
		}
		else if (this.Club == ClubType.LightMusic)
		{
			if (this.StudentID == 51)
			{
				if (this.InstrumentBag[this.ClubMemberID].transform.parent == null)
				{
					this.Instruments[this.ClubMemberID].transform.parent = null;
					if (!this.StudentManager.Eighties)
					{
						this.Instruments[this.ClubMemberID].transform.position = new Vector3(-0.5f, 4.5f, 22.45666f);
						this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, 0f, 0f);
					}
					else
					{
						this.Instruments[this.ClubMemberID].transform.position = new Vector3(2.105f, 4.5f, 25.5f);
						this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
					}
					this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().playOnAwake = false;
					this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Stop();
				}
				else
				{
					this.Instruments[this.ClubMemberID].SetActive(false);
				}
			}
			else
			{
				this.Instruments[this.ClubMemberID].SetActive(false);
			}
			this.Drumsticks[0].SetActive(false);
			this.Drumsticks[1].SetActive(false);
		}
		foreach (GameObject gameObject in this.ScienceProps)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		foreach (GameObject gameObject2 in this.Fingerfood)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
		}
		this.CharacterAnimation.CrossFade(this.CameraAnims[1]);
		this.EmptyHands();
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x001FDB88 File Offset: 0x001FBD88
	private void LookForYandere()
	{
		if (!this.Yandere.Chased && !this.Yandere.Invisible && this.CanSeeObject(this.Yandere.gameObject, this.Yandere.HeadPosition))
		{
			this.ReportPhase++;
		}
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x001FDBDC File Offset: 0x001FBDDC
	public void UpdatePerception()
	{
		if ((this.Yandere != null && this.Yandere.Club == ClubType.Occult) || (this.Yandere != null && this.Yandere.Class.StealthBonus > 0))
		{
			this.Perception = 0.5f;
		}
		else
		{
			this.Perception = 1f;
		}
		this.ChameleonCheck();
		if (this.Chameleon)
		{
			this.Perception *= 0.5f;
		}
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x001FDC60 File Offset: 0x001FBE60
	public void StopInvestigating()
	{
		Debug.Log(this.Name + " was investigating something, but has stopped.");
		this.BountyCollider.SetActive(false);
		this.Giggle = null;
		if (this.Sleuthing && this.CurrentAction != StudentActionType.SitAndEatBento)
		{
			this.CurrentDestination = this.SleuthTarget;
			this.Pathfinding.target = this.SleuthTarget;
		}
		else
		{
			this.CurrentDestination = this.Destinations[this.Phase];
			this.Pathfinding.target = this.Destinations[this.Phase];
			if (this.Actions[this.Phase] == StudentActionType.Sunbathe && this.SunbathePhase > 1)
			{
				this.CurrentDestination = this.StudentManager.SunbatheSpots[this.StudentID];
				this.Pathfinding.target = this.StudentManager.SunbatheSpots[this.StudentID];
			}
		}
		this.InvestigationDistance = 0.8f;
		this.InvestigationTimer = 0f;
		this.InvestigationPhase = 0;
		if (!this.Hurry)
		{
			this.Pathfinding.speed = this.WalkSpeed;
		}
		else
		{
			this.Pathfinding.speed = 4f;
			this.WalkSpeed = 4f;
		}
		if (!this.ReturningMisplacedWeapon && this.Club == ClubType.Sports && this.CurrentAction == StudentActionType.ClubAction && this.ClubActivityPhase > 2 && this.ClubActivityPhase < 14)
		{
			Debug.Log("Student was jogging before they started investigating, and will now return to jogging.");
			this.Jog();
		}
		if (this.CurrentAction == StudentActionType.Clean)
		{
			this.SmartPhone.SetActive(false);
			this.Scrubber.SetActive(true);
			if (this.CleaningRole == 5)
			{
				this.Scrubber.GetComponent<Renderer>().material.mainTexture = this.Eraser.GetComponent<Renderer>().material.mainTexture;
				this.Eraser.SetActive(true);
			}
		}
		if (this.DistanceToDestination == 0.5f)
		{
			this.DistanceToDestination = 0.66666f;
		}
		this.YandereInnocent = false;
		this.Investigating = false;
		this.EatingSnack = false;
		this.HeardScream = false;
		this.DiscCheck = false;
		this.Routine = true;
		this.TargetDistance = 0.5f;
		if (this.BeforeReturnAnim != "")
		{
			this.WalkAnim = this.BeforeReturnAnim;
		}
		if (this.SearchingForPhone)
		{
			this.PatrolID = 0;
		}
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x001FDEAC File Offset: 0x001FC0AC
	public void Jog()
	{
		string str = "";
		if (!this.Male)
		{
			str = "f02_";
		}
		this.WalkAnim = str + "trackJog_00";
		this.Hurry = true;
		this.CharacterAnimation[this.ClubAnim].time = 0f;
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.speed = 4f;
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x001FDF24 File Offset: 0x001FC124
	public void ForgetGiggle()
	{
		Debug.Log(this.Name + " was just told to ForgetGiggle() and stop investigating.");
		this.Giggle = null;
		this.InvestigationTimer = 0f;
		this.InvestigationPhase = 0;
		this.YandereInnocent = false;
		this.Investigating = false;
		this.DiscCheck = false;
	}

	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x060021A6 RID: 8614 RVA: 0x001FDF74 File Offset: 0x001FC174
	public bool InCouple
	{
		get
		{
			return this.CoupleID > 0;
		}
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x001FDF80 File Offset: 0x001FC180
	private bool LovedOneIsTargeted(int yandereTargetID)
	{
		Debug.Log("The player is allegedly attacking a student with an ID of: " + yandereTargetID.ToString());
		bool flag = this.StudentID == this.StudentManager.SuitorID && yandereTargetID == this.StudentManager.RivalID;
		if (this.StudentManager.KokonaTutorial)
		{
			return false;
		}
		if (!this.StudentManager.Eighties)
		{
			bool flag2 = this.InCouple && this.PartnerID == yandereTargetID;
			bool flag3 = this.StudentID == 3 && yandereTargetID == 2;
			bool flag4 = this.StudentID == 2 && yandereTargetID == 3;
			bool flag5 = this.StudentID == 11 && yandereTargetID == 10;
			bool flag6 = this.StudentID == 38 && yandereTargetID == 37;
			bool flag7 = this.StudentID == 37 && yandereTargetID == 38;
			bool flag8 = this.StudentID == 30 && yandereTargetID == 25;
			bool flag9 = this.StudentID == 25 && yandereTargetID == 30;
			bool flag10 = this.StudentID == 28 && yandereTargetID == 30;
			bool flag11 = false;
			bool flag12 = this.StudentID > 55 && this.StudentID < 61 && yandereTargetID > 55 && yandereTargetID < 61;
			if (this.Injured)
			{
				flag11 = (this.Club == ClubType.Delinquent && this.StudentManager.Students[yandereTargetID].Club == ClubType.Delinquent);
			}
			return flag2 || flag3 || flag4 || flag5 || flag6 || flag7 || flag8 || flag9 || flag10 || flag11 || flag12 || flag;
		}
		bool flag13 = this.Male && yandereTargetID == 19;
		return flag || flag13;
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x001FE124 File Offset: 0x001FC324
	private void Pose()
	{
		this.StudentManager.PoseMode.ChoosingAction = true;
		this.StudentManager.PoseMode.Panel.enabled = true;
		this.StudentManager.PoseMode.Student = this;
		this.StudentManager.PoseMode.UpdateLabels();
		this.StudentManager.PoseMode.Show = true;
		this.DialogueWheel.PromptBar.ClearButtons();
		this.DialogueWheel.PromptBar.Label[0].text = "Confirm";
		this.DialogueWheel.PromptBar.Label[1].text = "Back";
		this.DialogueWheel.PromptBar.Label[4].text = "Change";
		this.DialogueWheel.PromptBar.Label[5].text = "Increase/Decrease";
		this.DialogueWheel.PromptBar.UpdateButtons();
		this.DialogueWheel.PromptBar.Show = true;
		this.Yandere.Character.GetComponent<Animation>().CrossFade(this.Yandere.IdleAnim);
		this.Yandere.CanMove = false;
		this.Posing = true;
		this.SimpleLook.enabled = false;
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x001FE26C File Offset: 0x001FC46C
	public void DisableEffects()
	{
		this.LiquidProjector.enabled = false;
		if (this.ElectroSteam.Length != 0)
		{
			this.ElectroSteam[0].SetActive(false);
			this.ElectroSteam[1].SetActive(false);
			this.ElectroSteam[2].SetActive(false);
			this.ElectroSteam[3].SetActive(false);
		}
		if (this.CensorSteam.Length != 0)
		{
			this.CensorSteam[0].SetActive(false);
			this.CensorSteam[1].SetActive(false);
			this.CensorSteam[2].SetActive(false);
			this.CensorSteam[3].SetActive(false);
		}
		ParticleSystem[] array = this.LiquidEmitters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		array = this.FireEmitters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		this.ID = 0;
		while (this.ID < this.Bones.Length)
		{
			if (this.Bones[this.ID] != null)
			{
				this.Bones[this.ID].SetActive(false);
			}
			this.ID++;
		}
		if (this.Persona != PersonaType.PhoneAddict)
		{
			this.SmartPhone.SetActive(false);
		}
		this.Note.SetActive(false);
		this.SetSplashes(false);
		if (!this.Slave)
		{
			UnityEngine.Object.Destroy(this.Broken);
		}
		if (!this.Male)
		{
			this.HorudaCollider.gameObject.SetActive(false);
		}
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x001FE3F4 File Offset: 0x001FC5F4
	public void DetermineSenpaiReaction()
	{
		Debug.Log("We are now determining Senpai's reaction to Yandere-chan's behavior.");
		if (this.StudentManager.ChallengeManager.NoAlerts)
		{
			this.Subtitle.CustomText = "You alarmed a student! You have failed the ''NO ALERTS'' Challenge.";
			this.Subtitle.UpdateLabel(SubtitleType.Custom, 1, 4.5f);
		}
		if (this.Witnessed == StudentWitnessType.WeaponAndBloodAndInsanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiInsanityReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.WeaponAndBlood)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiWeaponReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.WeaponAndInsanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiInsanityReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.BloodAndInsanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiInsanityReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.Weapon)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiWeaponReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.Blood)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiBloodReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.Insanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiInsanityReaction, 1, 4.5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.Lewd || this.Witnessed == StudentWitnessType.Poisoning || this.Witnessed == StudentWitnessType.Pickpocketing || this.Witnessed == StudentWitnessType.Theft)
		{
			Debug.Log("Senpai is supposed to choose the ''Lewd'' reaction now.");
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiLewdReaction, 1, 4.5f);
			return;
		}
		if (this.GameOverCause == GameOverType.Stalking)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiStalkingReaction, this.Concern, 4.5f);
			return;
		}
		if (this.GameOverCause == GameOverType.Murder)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiMurderReaction, this.MurderReaction, 4.5f);
			return;
		}
		if (this.GameOverCause == GameOverType.Violence)
		{
			this.Subtitle.UpdateLabel(SubtitleType.SenpaiViolenceReaction, 1, 4.5f);
		}
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x001FE5D4 File Offset: 0x001FC7D4
	public void ForgetRadio()
	{
		bool flag = false;
		if (this.CurrentAction == StudentActionType.Sunbathe && this.SunbathePhase > 2)
		{
			this.SunbathePhase = 2;
			flag = true;
		}
		if (((this.Persona == PersonaType.PhoneAddict && !this.Phoneless && !flag) || this.Persona == PersonaType.Sleuth || this.StudentID == 20) && !this.Phoneless)
		{
			this.SmartPhone.SetActive(true);
		}
		this.BountyCollider.SetActive(false);
		this.TurnOffRadio = false;
		this.RadioTimer = 0f;
		this.RadioPhase = 1;
		this.Routine = true;
		this.Radio = null;
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x001FE670 File Offset: 0x001FC870
	public void RealizePhoneIsMissing()
	{
		Debug.Log(this.Name + " is updating their routine to involve ''Search Patrol''.");
		this.MustChangeClothing = false;
		this.SearchingForPhone = true;
		this.Phoneless = true;
		this.PatrolID = 0;
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[2];
		scheduleBlock.destination = "Search Patrol";
		scheduleBlock.action = "Search Patrol";
		ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[4];
		scheduleBlock2.destination = "Search Patrol";
		scheduleBlock2.action = "Search Patrol";
		ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[7];
		scheduleBlock3.destination = "Search Patrol";
		scheduleBlock3.action = "Search Patrol";
		this.GetDestinations();
		this.CurrentAction = StudentActionType.SearchPatrol;
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x001FE714 File Offset: 0x001FC914
	public void TeleportToDestination()
	{
		this.GetDestinations();
		int phase = this.Phase;
		if (this.Phase < this.ScheduleBlocks.Length && this.Clock.HourTime >= this.ScheduleBlocks[this.Phase].time)
		{
			this.Phase++;
			if (this.Actions[this.Phase] == StudentActionType.Patrol || (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Gardening))
			{
				this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
				this.Pathfinding.target = this.CurrentDestination;
			}
			else
			{
				this.CurrentDestination = this.Destinations[this.Phase];
				this.Pathfinding.target = this.Destinations[this.Phase];
			}
			if (this.CurrentDestination != null)
			{
				base.transform.position = this.CurrentDestination.position;
			}
		}
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x001FE828 File Offset: 0x001FCA28
	public void AltTeleportToDestination()
	{
		if (this.Club != ClubType.Council)
		{
			this.Phase++;
			if (this.Club == ClubType.Bully)
			{
				ScheduleBlock scheduleBlock = this.ScheduleBlocks[2];
				scheduleBlock.destination = "Patrol";
				scheduleBlock.action = "Patrol";
			}
			this.GetDestinations();
			this.CurrentAction = this.Actions[this.Phase];
			if (this.CurrentAction == StudentActionType.Patrol || (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Gardening))
			{
				this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
				this.Pathfinding.target = this.CurrentDestination;
				base.transform.position = this.CurrentDestination.position;
				return;
			}
			if (this.Destinations[this.Phase] != null)
			{
				this.CurrentDestination = this.Destinations[this.Phase];
				this.Pathfinding.target = this.Destinations[this.Phase];
				if (this.StudentID == 8)
				{
					base.transform.position = this.CurrentDestination.position;
				}
				Physics.SyncTransforms();
				return;
			}
			Debug.Log(this.Name + "'s destination for this phase of the day is null. Problem?");
		}
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x001FE980 File Offset: 0x001FCB80
	public void GoCommitMurder()
	{
		Debug.Log("A mind-broken slave has just been instructed to go kill somebody.");
		this.StudentManager.MurderTakingPlace = true;
		this.StudentManager.MindBrokenSlave = this;
		Debug.Log("MurderTakingPlace should now be true!");
		this.StudentManager.UpdateTeachers();
		if (!this.FragileSlave && !this.Yandere.Succubus)
		{
			this.Yandere.EquippedWeapon.transform.parent = this.HipCollider.transform;
			this.Yandere.EquippedWeapon.transform.localPosition = Vector3.zero;
			this.Yandere.EquippedWeapon.transform.localScale = Vector3.zero;
			this.MyWeapon = this.Yandere.EquippedWeapon;
			this.MyWeapon.FingerprintID = this.StudentID;
			this.Yandere.EquippedWeapon = null;
			this.Yandere.Equipped = 0;
			this.StudentManager.UpdateStudents(0);
			this.Yandere.WeaponManager.UpdateLabels();
			this.Yandere.WeaponMenu.UpdateSprites();
			this.Yandere.WeaponWarning = false;
		}
		else
		{
			this.StudentManager.FragileWeapon.transform.parent = this.HipCollider.transform;
			this.StudentManager.FragileWeapon.transform.localPosition = Vector3.zero;
			this.StudentManager.FragileWeapon.transform.localScale = Vector3.zero;
			this.MyWeapon = this.StudentManager.FragileWeapon;
			this.MyWeapon.FingerprintID = this.StudentID;
			this.MyWeapon.MyCollider.enabled = false;
		}
		this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
		if (!this.Male)
		{
			this.CharacterAnimation.CrossFade("f02_brokenStandUp_00");
			this.MurderSuicideAnim = "f02_murderSuicide_00";
		}
		else
		{
			this.MurderSuicideAnim = "murderSuicide_00";
		}
		if (this.HuntTarget != this)
		{
			this.DistanceToDestination = 100f;
			if (!this.Male)
			{
				this.Broken.Hunting = true;
			}
			this.TargetDistance = 1f;
			this.Routine = false;
			this.Hunting = true;
		}
		else
		{
			if (!this.Male)
			{
				this.Broken.Done = true;
			}
			this.Routine = false;
			this.Suicide = true;
		}
		this.Prompt.Hide();
		this.Prompt.enabled = false;
		if (this.NEStairs == null)
		{
			this.NEStairs = GameObject.Find("NEStairs").GetComponent<Collider>();
			this.NWStairs = GameObject.Find("NWStairs").GetComponent<Collider>();
			this.SEStairs = GameObject.Find("SEStairs").GetComponent<Collider>();
			this.SWStairs = GameObject.Find("SWStairs").GetComponent<Collider>();
			this.PoolStairs = GameObject.Find("PoolStairs").GetComponent<Collider>();
		}
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x001FEC68 File Offset: 0x001FCE68
	public void Shove()
	{
		if (!this.Yandere.Shoved && !this.Dying && !this.Yandere.Egg && !this.Yandere.Lifting && !this.Yandere.SneakingShot && !this.ShoeRemoval.enabled && !this.Yandere.Talking && !this.SentToLocker)
		{
			if (this.Giggle != null)
			{
				this.ForgetGiggle();
			}
			this.ForgetRadio();
			base.GetComponent<AudioSource>();
			if (this.StudentID == 86)
			{
				this.Subtitle.UpdateLabel(SubtitleType.Shoving, 1, 5f);
			}
			else if (this.StudentID == 87)
			{
				this.Subtitle.UpdateLabel(SubtitleType.Shoving, 2, 5f);
			}
			else if (this.StudentID == 88)
			{
				this.Subtitle.UpdateLabel(SubtitleType.Shoving, 3, 5f);
			}
			else if (this.StudentID == 89)
			{
				this.Subtitle.UpdateLabel(SubtitleType.Shoving, 4, 5f);
			}
			if (this.Yandere.Aiming)
			{
				this.Yandere.StopAiming();
			}
			if (this.Yandere.Laughing)
			{
				this.Yandere.StopLaughing();
			}
			if (this.Yandere.Stance.Current != StanceType.Standing)
			{
				this.Yandere.Stance.Current = StanceType.Standing;
				this.Yandere.CrawlTimer = 0f;
				this.Yandere.Uncrouch();
			}
			base.transform.rotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
			this.Yandere.transform.rotation = Quaternion.LookRotation(new Vector3(this.Hips.transform.position.x, this.Yandere.transform.position.y, this.Hips.transform.position.z) - this.Yandere.transform.position);
			this.CharacterAnimation[this.ShoveAnim].time = 0f;
			this.CharacterAnimation.CrossFade(this.ShoveAnim);
			this.FocusOnStudent = false;
			this.FocusOnYandere = false;
			this.Investigating = false;
			this.Distracted = true;
			this.Alarmed = false;
			this.Routine = false;
			this.Shoving = true;
			this.NoTalk = false;
			this.Patience--;
			if (this.StudentManager.BloodReporter == this)
			{
				this.StudentManager.BloodReporter = null;
				this.ReportingBlood = false;
			}
			if (this.Club == ClubType.Delinquent && (this.WitnessedBloodPool || this.FoundEnemyCorpse))
			{
				this.StudentManager.CombatMinigame.ExitSchoolWhenDone = true;
			}
			this.WitnessedBloodyWeapon = false;
			this.WitnessedBloodPool = false;
			this.WitnessedSomething = false;
			this.WitnessedMurder = false;
			this.WitnessedWeapon = false;
			this.WitnessedLimb = false;
			this.BloodPool = null;
			if (this.Club != ClubType.Council && this.Persona != PersonaType.Violent && this.StudentID != 20)
			{
				Debug.Log("Patience shot up to 999.");
				this.Patience = 999;
			}
			if (this.Patience < 1)
			{
				this.Yandere.CannotRecover = true;
			}
			if (this.ReturningMisplacedWeapon)
			{
				this.DropMisplacedWeapon();
			}
			if (this.StudentManager.Eighties && this.StudentID > 85 && this.StudentID < 89)
			{
				this.Yandere.ShoveAnim = "f02_shoveA_01";
			}
			else
			{
				this.Yandere.ShoveAnim = "f02_shoveA_01";
			}
			this.Yandere.CharacterAnimation[this.Yandere.ShoveAnim].time = 0f;
			this.Yandere.CharacterAnimation.CrossFade(this.Yandere.ShoveAnim);
			this.Yandere.YandereVision = false;
			this.Yandere.NearSenpai = false;
			this.Yandere.Degloving = false;
			this.Yandere.Flicking = false;
			this.Yandere.Punching = false;
			this.Yandere.CanMove = false;
			this.Yandere.Shoved = true;
			if (this.Yandere.PickUp != null && this.Yandere.PickUp.PreventTipping)
			{
				Debug.Log("She's holding a water cooler. Don't do a god damn thing.");
			}
			else
			{
				this.Yandere.EmptyHands();
			}
			this.Yandere.GloveTimer = 0f;
			this.Yandere.h = 0f;
			this.Yandere.v = 0f;
			this.Yandere.ShoveSpeed = 2f;
			if (this.Distraction != null)
			{
				this.TargetedForDistraction = false;
				this.Pathfinding.speed = this.WalkSpeed;
				this.SpeechLines.Stop();
				this.Distraction = null;
				this.CanTalk = true;
			}
			if (this.Actions[this.Phase] != StudentActionType.Patrol)
			{
				this.CurrentDestination = this.Destinations[this.Phase];
				this.Pathfinding.target = this.CurrentDestination;
			}
			this.Pathfinding.canSearch = false;
			this.Pathfinding.canMove = false;
		}
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x001FF200 File Offset: 0x001FD400
	public void PushYandereAway()
	{
		if (this.Yandere.Aiming)
		{
			this.Yandere.StopAiming();
		}
		if (this.Yandere.Laughing)
		{
			this.Yandere.StopLaughing();
		}
		this.Yandere.transform.rotation = Quaternion.LookRotation(new Vector3(this.Hips.transform.position.x, this.Yandere.transform.position.y, this.Hips.transform.position.z) - this.Yandere.transform.position);
		this.Yandere.CharacterAnimation["f02_shoveA_01"].time = 0f;
		this.Yandere.CharacterAnimation.CrossFade("f02_shoveA_01");
		this.Yandere.YandereVision = false;
		this.Yandere.NearSenpai = false;
		this.Yandere.Degloving = false;
		this.Yandere.Flicking = false;
		this.Yandere.Punching = false;
		this.Yandere.CanMove = false;
		this.Yandere.Shoved = true;
		this.Yandere.EmptyHands();
		this.Yandere.GloveTimer = 0f;
		this.Yandere.h = 0f;
		this.Yandere.v = 0f;
		this.Yandere.ShoveSpeed = 2f;
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x001FF380 File Offset: 0x001FD580
	public void Spray()
	{
		Debug.Log(this.Name + " is trying to Spray Yandere-chan!");
		if (this.Yandere.Attacking || this.Yandere.Struggling)
		{
			this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
			return;
		}
		bool flag = false;
		if (this.Yandere.DelinquentFighting && !this.NoBreakUp && !this.StudentManager.CombatMinigame.Delinquent.WitnessedMurder)
		{
			flag = true;
		}
		if (!flag)
		{
			if (!this.Yandere.Sprayed && !this.Dying && !this.Blind && !this.Yandere.Egg && !this.Yandere.Dumping && !this.Yandere.Bathing && !this.Yandere.Noticed && !this.Yandere.CannotBeSprayed)
			{
				if (this.SprayTimer > 0f)
				{
					this.SprayTimer = Mathf.MoveTowards(this.SprayTimer, 0f, Time.deltaTime);
				}
				else
				{
					AudioSource.PlayClipAtPoint(this.PepperSpraySFX, base.transform.position);
					if (this.StudentID == 86)
					{
						this.Subtitle.UpdateLabel(SubtitleType.Spraying, 1, 5f);
					}
					else if (this.StudentID == 87)
					{
						this.Subtitle.UpdateLabel(SubtitleType.Spraying, 2, 5f);
					}
					else if (this.StudentID == 88)
					{
						this.Subtitle.UpdateLabel(SubtitleType.Spraying, 3, 5f);
					}
					else if (this.StudentID == 89)
					{
						this.Subtitle.UpdateLabel(SubtitleType.Spraying, 4, 5f);
					}
					if (this.Yandere.Aiming)
					{
						this.Yandere.StopAiming();
					}
					if (this.Yandere.Laughing)
					{
						this.Yandere.StopLaughing();
					}
					base.transform.rotation = Quaternion.LookRotation(new Vector3(this.Yandere.Hips.transform.position.x, base.transform.position.y, this.Yandere.Hips.transform.position.z) - base.transform.position);
					this.Yandere.transform.rotation = Quaternion.LookRotation(new Vector3(this.Hips.transform.position.x, this.Yandere.transform.position.y, this.Hips.transform.position.z) - this.Yandere.transform.position);
					Debug.Log("This is the exact moment that the character is being told to perform a spraying animation.");
					if (this.SprayAnim == "")
					{
						if (this.Male)
						{
							this.SprayAnim = "spray_00";
						}
						else
						{
							this.SprayAnim = "f02_sprayCouncilEdgy_00";
						}
					}
					this.CharacterAnimation.CrossFade(this.SprayAnim);
					this.PepperSpray.SetActive(true);
					this.FocusOnStudent = false;
					this.FocusOnYandere = false;
					this.Distracted = true;
					this.Spraying = true;
					this.Alarmed = false;
					this.Routine = false;
					this.Fleeing = true;
					this.Blind = true;
					this.Yandere.CharacterAnimation.CrossFade("f02_sprayed_00");
					this.Yandere.YandereVision = false;
					this.Yandere.NearSenpai = false;
					this.Yandere.Attacking = false;
					this.Yandere.FollowHips = true;
					this.Yandere.Punching = false;
					this.Yandere.CanMove = false;
					this.Yandere.Sprayed = true;
					this.Pathfinding.canSearch = false;
					this.Pathfinding.canMove = false;
					this.StudentManager.YandereDying = true;
					this.StudentManager.StopMoving();
					this.Yandere.Blur.Size = 1f;
					this.Yandere.Jukebox.Volume = 0f;
					if (this.Yandere.DelinquentFighting)
					{
						this.StudentManager.CombatMinigame.Stop();
					}
					this.DetectionMarker.gameObject.SetActive(false);
					if (this.SmartPhone != null && this.SmartPhone.activeInHierarchy)
					{
						this.SmartPhone.SetActive(false);
					}
				}
			}
			else if (!this.Yandere.Sprayed)
			{
				this.CharacterAnimation.CrossFade(this.ReadyToFightAnim);
				if (this.Yandere.Egg)
				{
					this.Yandere.CanMove = true;
					this.Yandere.Chased = false;
					this.Yandere.Chasers = 0;
					this.BecomeRagdoll();
				}
			}
		}
		else
		{
			Debug.Log("A student council member is breaking up the fight.");
			if (this.StudentManager.CombatMinigame.Delinquent.Male)
			{
				this.StudentManager.CombatMinigame.Delinquent.CharacterAnimation.Play("stopFighting_00");
				this.StudentManager.CombatMinigame.StopFightingAnim = "stopFighting_00";
			}
			else
			{
				this.StudentManager.CombatMinigame.Delinquent.CharacterAnimation.Play("f02_stopFighting_00");
				this.StudentManager.CombatMinigame.StopFightingAnim = "f02_stopFighting_00";
			}
			this.Yandere.CharacterAnimation.Play("f02_stopFighting_00");
			this.Yandere.FightHasBrokenUp = true;
			this.Yandere.BreakUpTimer = 10f;
			this.StudentManager.CombatMinigame.Path = 7;
			this.StudentManager.Portal.SetActive(true);
			if (!this.BreakingUpFight && this.Club == ClubType.Council)
			{
				this.Subtitle.UpdateLabel(SubtitleType.BreakingUp, this.ClubMemberID, 5f);
			}
			this.CharacterAnimation.Play(this.BreakUpAnim);
			this.BreakingUpFight = true;
			this.SprayTimer = 1f;
		}
		this.StudentManager.CombatMinigame.DisablePrompts();
		this.StudentManager.CombatMinigame.MyVocals.Stop();
		this.StudentManager.CombatMinigame.MyAudio.Stop();
		Time.timeScale = 1f;
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x001FF9D4 File Offset: 0x001FDBD4
	private void DetermineCorpseLocation()
	{
		Debug.Log(this.Name + " has called the DetermineCorpseLocation() function.");
		if (this.StudentManager.Reporter == null)
		{
			this.StudentManager.Reporter = this;
		}
		if (this.Teacher)
		{
			this.StudentManager.CorpseLocation.position = this.Corpse.AllColliders[0].transform.position;
			this.StudentManager.CorpseLocation.LookAt(new Vector3(base.transform.position.x, this.StudentManager.CorpseLocation.position.y, base.transform.position.z));
			this.StudentManager.CorpseLocation.Translate(this.StudentManager.CorpseLocation.forward);
			this.StudentManager.LowerCorpsePosition();
		}
		if (this.ExamineCorpseTarget == null)
		{
			this.Pathfinding.target = this.StudentManager.CorpseLocation;
			this.CurrentDestination = this.StudentManager.CorpseLocation;
		}
		this.AssignCorpseGuardLocations();
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x001FFAF8 File Offset: 0x001FDCF8
	private void DetermineBloodLocation()
	{
		Debug.Log(this.Name + " is now firing DetermineBloodLocation().");
		if (this.StudentManager.BloodReporter == null)
		{
			this.StudentManager.BloodReporter = this;
		}
		if (this.Teacher)
		{
			this.StudentManager.BloodLocation.position = this.BloodPool.transform.position;
			this.StudentManager.BloodLocation.LookAt(new Vector3(base.transform.position.x, this.StudentManager.BloodLocation.position.y, base.transform.position.z));
			this.StudentManager.BloodLocation.Translate(this.StudentManager.BloodLocation.forward, Space.World);
			this.StudentManager.LowerBloodPosition();
		}
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x001FFBDC File Offset: 0x001FDDDC
	private void AssignCorpseGuardLocations()
	{
		this.StudentManager.CorpseGuardLocation[1].position = this.StudentManager.CorpseLocation.position + new Vector3(0f, 0f, 1f);
		this.LookAway(this.StudentManager.CorpseGuardLocation[1], this.StudentManager.CorpseLocation);
		this.StudentManager.CorpseGuardLocation[2].position = this.StudentManager.CorpseLocation.position + new Vector3(1f, 0f, 0f);
		this.LookAway(this.StudentManager.CorpseGuardLocation[2], this.StudentManager.CorpseLocation);
		this.StudentManager.CorpseGuardLocation[3].position = this.StudentManager.CorpseLocation.position + new Vector3(0f, 0f, -1f);
		this.LookAway(this.StudentManager.CorpseGuardLocation[3], this.StudentManager.CorpseLocation);
		this.StudentManager.CorpseGuardLocation[4].position = this.StudentManager.CorpseLocation.position + new Vector3(-1f, 0f, 0f);
		this.LookAway(this.StudentManager.CorpseGuardLocation[4], this.StudentManager.CorpseLocation);
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x001FFD50 File Offset: 0x001FDF50
	private void AssignBloodGuardLocations()
	{
		this.StudentManager.BloodGuardLocation[1].position = this.StudentManager.BloodLocation.position + new Vector3(0f, 0f, 1f);
		this.LookAway(this.StudentManager.BloodGuardLocation[1], this.StudentManager.BloodLocation);
		this.StudentManager.BloodGuardLocation[2].position = this.StudentManager.BloodLocation.position + new Vector3(1f, 0f, 0f);
		this.LookAway(this.StudentManager.BloodGuardLocation[2], this.StudentManager.BloodLocation);
		this.StudentManager.BloodGuardLocation[3].position = this.StudentManager.BloodLocation.position + new Vector3(0f, 0f, -1f);
		this.LookAway(this.StudentManager.BloodGuardLocation[3], this.StudentManager.BloodLocation);
		this.StudentManager.BloodGuardLocation[4].position = this.StudentManager.BloodLocation.position + new Vector3(-1f, 0f, 0f);
		this.LookAway(this.StudentManager.BloodGuardLocation[4], this.StudentManager.BloodLocation);
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x001FFEC4 File Offset: 0x001FE0C4
	private void AssignTeacherGuardLocations()
	{
		this.StudentManager.TeacherGuardLocation[1].position = this.StudentManager.CorpseLocation.position + new Vector3(0.75f, 0f, 0.75f);
		this.LookAway(this.StudentManager.TeacherGuardLocation[1], this.StudentManager.CorpseLocation);
		this.StudentManager.TeacherGuardLocation[2].position = this.StudentManager.CorpseLocation.position + new Vector3(0.75f, 0f, -0.75f);
		this.LookAway(this.StudentManager.TeacherGuardLocation[2], this.StudentManager.CorpseLocation);
		this.StudentManager.TeacherGuardLocation[3].position = this.StudentManager.CorpseLocation.position + new Vector3(-0.75f, 0f, -0.75f);
		this.LookAway(this.StudentManager.TeacherGuardLocation[3], this.StudentManager.CorpseLocation);
		this.StudentManager.TeacherGuardLocation[4].position = this.StudentManager.CorpseLocation.position + new Vector3(-0.75f, 0f, 0.75f);
		this.LookAway(this.StudentManager.TeacherGuardLocation[4], this.StudentManager.CorpseLocation);
		this.StudentManager.TeacherGuardLocation[5].position = this.StudentManager.CorpseLocation.position + new Vector3(0f, 0f, 0.5f);
		this.LookAway(this.StudentManager.TeacherGuardLocation[5], this.StudentManager.CorpseLocation);
		this.StudentManager.TeacherGuardLocation[6].position = this.StudentManager.CorpseLocation.position + new Vector3(0f, 0f, -0.5f);
		this.LookAway(this.StudentManager.TeacherGuardLocation[6], this.StudentManager.CorpseLocation);
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x002000E8 File Offset: 0x001FE2E8
	private void LookAway(Transform T1, Transform T2)
	{
		T1.LookAt(T2);
		float y = T1.eulerAngles.y + 180f;
		T1.eulerAngles = new Vector3(T1.eulerAngles.x, y, T1.eulerAngles.z);
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x00200130 File Offset: 0x001FE330
	public void TurnToStone()
	{
		this.Cosmetic.RightEyeRenderer.material.mainTexture = this.Yandere.Stone;
		this.Cosmetic.LeftEyeRenderer.material.mainTexture = this.Yandere.Stone;
		this.Cosmetic.HairRenderer.material.mainTexture = this.Yandere.Stone;
		if (this.Cosmetic.HairRenderer.materials.Length > 1)
		{
			this.Cosmetic.HairRenderer.materials[1].mainTexture = this.Yandere.Stone;
		}
		this.Cosmetic.RightEyeRenderer.material.color = new Color(1f, 1f, 1f, 1f);
		this.Cosmetic.LeftEyeRenderer.material.color = new Color(1f, 1f, 1f, 1f);
		this.Cosmetic.HairRenderer.material.color = new Color(1f, 1f, 1f, 1f);
		this.MyRenderer.materials[0].mainTexture = this.Yandere.Stone;
		this.MyRenderer.materials[1].mainTexture = this.Yandere.Stone;
		this.MyRenderer.materials[2].mainTexture = this.Yandere.Stone;
		if (this.Teacher && this.Cosmetic.TeacherAccessories[8].activeInHierarchy)
		{
			this.MyRenderer.materials[3].mainTexture = this.Yandere.Stone;
		}
		if (this.PickPocket != null)
		{
			this.PickPocket.enabled = false;
			this.PickPocket.Prompt.Hide();
			this.PickPocket.Prompt.enabled = false;
		}
		this.MyRenderer.materials[0].SetFloat("_BlendAmount", 0f);
		this.MyRenderer.materials[1].SetFloat("_BlendAmount", 0f);
		UnityEngine.Object.Destroy(this.DetectionMarker.gameObject);
		AudioSource.PlayClipAtPoint(this.Yandere.Petrify, base.transform.position + new Vector3(0f, 1f, 0f));
		UnityEngine.Object.Instantiate<GameObject>(this.Yandere.Pebbles, this.Hips.position, Quaternion.identity);
		this.Pathfinding.enabled = false;
		this.ShoeRemoval.enabled = false;
		this.CharacterAnimation.Stop();
		this.Prompt.enabled = false;
		this.SpeechLines.Stop();
		this.Prompt.Hide();
		base.enabled = false;
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x00200414 File Offset: 0x001FE614
	public void StopPairing()
	{
		if (this.Actions[this.Phase] != StudentActionType.Clean && this.Persona == PersonaType.PhoneAddict && !this.Phoneless && !this.LostTeacherTrust && !this.StudentManager.Eighties)
		{
			this.WalkAnim = this.PhoneAnims[1];
		}
		this.Spawned = true;
		this.Paired = false;
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x00200478 File Offset: 0x001FE678
	public void ChameleonCheck()
	{
		this.ChameleonBonus = 0f;
		this.Chameleon = false;
		if (this.Yandere != null && ((this.Yandere.Persona == YanderePersonaType.Scholarly && this.Persona == PersonaType.TeachersPet) || (this.Yandere.Persona == YanderePersonaType.Scholarly && this.Club == ClubType.Science) || (this.Yandere.Persona == YanderePersonaType.Scholarly && this.Club == ClubType.Art) || (this.Yandere.Persona == YanderePersonaType.Chill && this.Persona == PersonaType.SocialButterfly) || (this.Yandere.Persona == YanderePersonaType.Chill && this.Club == ClubType.Photography) || (this.Yandere.Persona == YanderePersonaType.Chill && this.Club == ClubType.Gaming) || (this.Yandere.Persona == YanderePersonaType.Confident && this.Persona == PersonaType.Heroic) || (this.Yandere.Persona == YanderePersonaType.Confident && this.Club == ClubType.MartialArts) || (this.Yandere.Persona == YanderePersonaType.Elegant && this.Club == ClubType.Drama) || (this.Yandere.Persona == YanderePersonaType.Girly && this.Persona == PersonaType.SocialButterfly) || (this.Yandere.Persona == YanderePersonaType.Girly && this.Club == ClubType.Cooking) || (this.Yandere.Persona == YanderePersonaType.Graceful && this.Club == ClubType.Gardening) || (this.Yandere.Persona == YanderePersonaType.Haughty && this.Club == ClubType.Bully) || (this.Yandere.Persona == YanderePersonaType.Lively && this.Persona == PersonaType.SocialButterfly) || (this.Yandere.Persona == YanderePersonaType.Lively && this.Club == ClubType.LightMusic) || (this.Yandere.Persona == YanderePersonaType.Lively && this.Club == ClubType.Sports) || (this.Yandere.Persona == YanderePersonaType.Shy && this.Persona == PersonaType.Loner) || (this.Yandere.Persona == YanderePersonaType.Shy && this.Club == ClubType.Occult) || (this.Yandere.Persona == YanderePersonaType.Shy && this.Shy) || (this.Yandere.Persona == YanderePersonaType.Tough && this.Persona == PersonaType.Spiteful) || (this.Yandere.Persona == YanderePersonaType.Tough && this.Club == ClubType.Delinquent) || (this.StudentManager.CustomMode && this.Yandere.AnimSetID == this.AnimSetID)))
		{
			Debug.Log("Chameleon is true!");
			Debug.Log("Yandere.AnimSetID is " + this.Yandere.AnimSetID.ToString() + " and Student's AnimSetID is " + this.AnimSetID.ToString());
			this.ChameleonBonus = this.VisionDistance * 0.5f;
			this.Chameleon = true;
		}
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x00200740 File Offset: 0x001FE940
	private void PhoneAddictGameOver()
	{
		if (!this.Yandere.Lost && !this.Yandere.ShoulderCamera.HeartbrokenCamera.activeInHierarchy)
		{
			this.Yandere.CharacterAnimation.CrossFade("f02_down_22");
			this.Yandere.ShoulderCamera.HeartbrokenCamera.SetActive(true);
			this.Yandere.RPGCamera.enabled = false;
			this.Yandere.Jukebox.GameOver();
			this.Yandere.enabled = false;
			this.Yandere.EmptyHands();
			this.Countdown.gameObject.SetActive(false);
			this.ChaseCamera.SetActive(false);
			this.Police.Heartbroken.Exposed = true;
			this.StudentManager.StopMoving();
			this.Fleeing = false;
		}
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x0020081C File Offset: 0x001FEA1C
	private void EndAlarm()
	{
		Debug.Log(this.Name + " just fired the EndAlarm() function.");
		if (this.ReturnToRoutineAfter)
		{
			this.CurrentDestination = this.Destinations[this.Phase];
			this.Pathfinding.target = this.Destinations[this.Phase];
			this.ReturnToRoutineAfter = false;
		}
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		if (this.TurnOffRadio)
		{
			this.RadioTimer = 3f;
		}
		if (this.StudentID == 1 || this.Teacher)
		{
			this.IgnoreTimer = 0.0001f;
		}
		else
		{
			this.IgnoreTimer = 5f;
		}
		if (this.Persona == PersonaType.PhoneAddict && !this.Phoneless)
		{
			this.SmartPhone.SetActive(true);
		}
		this.FocusOnStudent = false;
		this.FocusOnYandere = false;
		this.DiscCheck = false;
		this.Alarmed = false;
		this.Reacted = false;
		this.Hesitation = 0f;
		this.AlarmTimer = 0f;
		if (this.WitnessedCorpse)
		{
			this.PersonaReaction();
		}
		else if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
		{
			Debug.Log(this.Name + " will now investigate a suspicious object on the ground...");
			if (this.Following)
			{
				this.Hearts.emission.enabled = false;
				this.FollowCountdown.gameObject.SetActive(false);
				this.Yandere.Follower = null;
				this.Yandere.Followers--;
				this.Following = false;
			}
			if (this.BeforeReturnAnim == "")
			{
				this.BeforeReturnAnim = this.WalkAnim;
			}
			this.WalkAnim = this.OriginalWalkAnim;
			this.CharacterAnimation.CrossFade(this.WalkAnim);
			this.CurrentDestination = this.BloodPool;
			this.Pathfinding.target = this.BloodPool;
			this.Pathfinding.canSearch = true;
			this.Pathfinding.canMove = true;
			this.WalkSpeed = 1f;
			this.Pathfinding.speed = this.WalkSpeed;
			this.InvestigatingBloodPool = true;
			this.Routine = false;
			this.IgnoreTimer = 0.0001f;
		}
		else if (!this.Following && !this.Wet && !this.Investigating)
		{
			this.Routine = true;
		}
		if (this.ResumeDistracting)
		{
			Debug.Log(this.Name + " was told to resume distracting.");
			this.CharacterAnimation.CrossFade(this.WalkAnim);
			this.Distracting = true;
			this.Routine = false;
			this.CurrentDestination = this.DistractionTarget.transform;
			this.Pathfinding.target = this.DistractionTarget.transform;
			this.ResumeDistracting = false;
		}
		if (this.ResumeTakingOutTrash)
		{
			Debug.Log("This character was told to resume taking out the trash.");
			this.CharacterAnimation.CrossFade(this.WalkAnim);
			this.TakingOutTrash = true;
			this.Routine = false;
		}
		if (this.CurrentAction == StudentActionType.Clean)
		{
			this.SmartPhone.SetActive(false);
			this.Scrubber.SetActive(true);
			if (this.CleaningRole == 5)
			{
				this.Scrubber.GetComponent<Renderer>().material.mainTexture = this.Eraser.GetComponent<Renderer>().material.mainTexture;
				this.Eraser.SetActive(true);
			}
		}
		if (this.TurnOffRadio)
		{
			this.Routine = false;
		}
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x00200B8C File Offset: 0x001FED8C
	public void GetSleuthTarget()
	{
		if (!this.SleuthInitialized)
		{
			this.SleuthInitialized = true;
			this.GetInitialSleuthTarget();
		}
		this.WalkAnim = this.SleuthWalkAnim;
		this.TargetDistance = 2f;
		this.SleuthID++;
		if (this.SleuthID < 98)
		{
			if (this.StudentManager.Students[this.SleuthID] == null)
			{
				this.GetSleuthTarget();
				return;
			}
			if (!this.StudentManager.Students[this.SleuthID].gameObject.activeInHierarchy)
			{
				this.GetSleuthTarget();
				return;
			}
			if ((this.CurrentDestination != null && this.StudentManager.LockerRoomArea.bounds.Contains(this.CurrentDestination.position)) || (this.CurrentDestination != null && this.StudentManager.MaleLockerRoomArea.bounds.Contains(this.CurrentDestination.position)))
			{
				this.GetSleuthTarget();
				return;
			}
			if (this.StudentManager.Students[this.SleuthID].Slave)
			{
				this.GetSleuthTarget();
				return;
			}
			this.SleuthTarget = this.StudentManager.Students[this.SleuthID].transform;
			this.Pathfinding.target = this.SleuthTarget;
			this.CurrentDestination = this.SleuthTarget;
			return;
		}
		else
		{
			if (this.SleuthID != 98)
			{
				this.SleuthID = 0;
				this.GetSleuthTarget();
				return;
			}
			if (this.Yandere.Club == ClubType.Photography)
			{
				this.SleuthID = 0;
				this.GetSleuthTarget();
				return;
			}
			Debug.Log(this.Name + "'s SleuthTarget became the player...");
			this.SleuthTarget = this.Yandere.transform;
			this.Pathfinding.target = this.SleuthTarget;
			this.CurrentDestination = this.SleuthTarget;
			return;
		}
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x00200D68 File Offset: 0x001FEF68
	public void GetFoodTarget()
	{
		this.Attempts++;
		if (this.Attempts >= 100)
		{
			this.Routine = true;
			this.Phase++;
			return;
		}
		if (this.SleuthID < 1)
		{
			this.SleuthID = 1;
		}
		this.SleuthID++;
		if (this.SleuthID >= 90)
		{
			this.SleuthID = 0;
			this.GetFoodTarget();
			return;
		}
		if (this.SleuthID == this.StudentID)
		{
			this.GetFoodTarget();
			return;
		}
		if (this.StudentManager.Students[this.SleuthID] == null)
		{
			this.GetFoodTarget();
			return;
		}
		if (this.StudentManager.Students[this.SleuthID].FollowTarget != null)
		{
			this.GetFoodTarget();
			return;
		}
		if (!this.StudentManager.Students[this.SleuthID].gameObject.activeInHierarchy)
		{
			this.GetFoodTarget();
			return;
		}
		if (this.StudentManager.Students[this.SleuthID].CurrentAction == StudentActionType.SitAndEatBento || this.StudentManager.Students[this.SleuthID].CurrentAction == StudentActionType.AtLocker || this.StudentManager.Students[this.SleuthID].CurrentAction == StudentActionType.Admire || this.StudentManager.Students[this.SleuthID].CurrentDestination == this.StudentManager.Exit || this.StudentManager.Students[this.SleuthID].Club == ClubType.Cooking || this.StudentManager.Students[this.SleuthID].Club == ClubType.Delinquent || this.StudentManager.Students[this.SleuthID].Club == ClubType.Sports || this.StudentManager.Students[this.SleuthID].TargetedForDistraction || this.StudentManager.Students[this.SleuthID].ClubActivityPhase >= 16 || this.StudentManager.Students[this.SleuthID].InEvent || !this.StudentManager.Students[this.SleuthID].Routine || this.StudentManager.Students[this.SleuthID].Posing || this.StudentManager.Students[this.SleuthID].Slave || this.StudentManager.Students[this.SleuthID].Wet || this.StudentManager.Students[this.SleuthID].Sedated || this.StudentManager.Students[this.SleuthID].DoNotFeed || this.StudentManager.Students[this.SleuthID].AlreadyFed || (this.StudentManager.Students[this.SleuthID].Club == ClubType.LightMusic && this.StudentManager.PracticeMusic.isPlaying))
		{
			StudentActionType currentAction = this.StudentManager.Students[this.SleuthID].CurrentAction;
			StudentActionType currentAction2 = this.StudentManager.Students[this.SleuthID].CurrentAction;
			this.StudentManager.Students[this.SleuthID].CurrentDestination == this.StudentManager.Exit;
			ClubType club = this.StudentManager.Students[this.SleuthID].Club;
			ClubType club2 = this.StudentManager.Students[this.SleuthID].Club;
			ClubType club3 = this.StudentManager.Students[this.SleuthID].Club;
			bool targetedForDistraction = this.StudentManager.Students[this.SleuthID].TargetedForDistraction;
			int clubActivityPhase = this.StudentManager.Students[this.SleuthID].ClubActivityPhase;
			bool inEvent = this.StudentManager.Students[this.SleuthID].InEvent;
			bool routine = this.StudentManager.Students[this.SleuthID].Routine;
			bool posing = this.StudentManager.Students[this.SleuthID].Posing;
			bool slave = this.StudentManager.Students[this.SleuthID].Slave;
			bool wet = this.StudentManager.Students[this.SleuthID].Wet;
			bool sedated = this.StudentManager.Students[this.SleuthID].Sedated;
			bool doNotFeed = this.StudentManager.Students[this.SleuthID].DoNotFeed;
			bool alreadyFed = this.StudentManager.Students[this.SleuthID].AlreadyFed;
			if (this.StudentManager.Students[this.SleuthID].Club == ClubType.LightMusic)
			{
				bool isPlaying = this.StudentManager.PracticeMusic.isPlaying;
			}
			this.GetFoodTarget();
			return;
		}
		if (this.StudentManager.LockerRoomArea.bounds.Contains(this.StudentManager.Students[this.SleuthID].transform.position) || this.StudentManager.MaleLockerRoomArea.bounds.Contains(this.StudentManager.Students[this.SleuthID].transform.position) || this.StudentManager.EastBathroomArea.bounds.Contains(this.StudentManager.Students[this.SleuthID].transform.position) || this.StudentManager.WestBathroomArea.bounds.Contains(this.StudentManager.Students[this.SleuthID].transform.position) || this.StudentManager.Students[this.SleuthID].transform.position.z < -100f)
		{
			this.GetFoodTarget();
			return;
		}
		this.CharacterAnimation.CrossFade(this.WalkAnim);
		this.DistractionTarget = this.StudentManager.Students[this.SleuthID];
		this.DistractionTarget.TargetedForDistraction = true;
		this.SleuthTarget = this.StudentManager.Students[this.SleuthID].transform;
		this.Pathfinding.target = this.SleuthTarget;
		this.CurrentDestination = this.SleuthTarget;
		this.TargetDistance = 0.75f;
		this.DistractTimer = 8f;
		this.Distracting = true;
		this.CanTalk = false;
		this.Routine = false;
		this.Attempts = 0;
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x002013F8 File Offset: 0x001FF5F8
	public void GetTeacherTarget()
	{
		Debug.Log(this.Name + " is now attempting to select a teacher to talk to.");
		this.Attempts++;
		if (this.Attempts >= 100)
		{
			Debug.Log(this.Name + " is now giving up on attempting to select a teacher to talk to.");
			this.Phase++;
			return;
		}
		this.TeacherID++;
		if (this.TeacherID >= 97)
		{
			Debug.Log(this.Name + " went past Teacher #96 so they're resetting back to $90.");
			this.TeacherID = 90;
			this.GetTeacherTarget();
			return;
		}
		if (this.StudentManager.Students[this.TeacherID] == null)
		{
			Debug.Log(this.Name + " can't try to talk to Teacher #" + this.TeacherID.ToString() + " because that teacher is not at school right now.");
			this.GetTeacherTarget();
			return;
		}
		if (!this.StudentManager.Students[this.TeacherID].gameObject.activeInHierarchy)
		{
			Debug.Log(this.Name + " can't try to talk to Teacher #" + this.TeacherID.ToString() + " because that teacher is disabled right now.");
			this.GetTeacherTarget();
			return;
		}
		if (this.StudentManager.Students[this.TeacherID].TargetedForDistraction || this.StudentManager.Students[this.TeacherID].InEvent || !this.StudentManager.Students[this.TeacherID].Routine || this.StudentManager.Students[this.TeacherID].Posing)
		{
			this.GetTeacherTarget();
			return;
		}
		Debug.Log(this.Name + " is choosing Teacher #" + this.TeacherID.ToString() + " as their target.");
		this.CharacterAnimation.CrossFade(this.WalkAnim);
		this.DistractionTarget = this.StudentManager.Students[this.TeacherID];
		this.DistractionTarget.TargetedForDistraction = true;
		this.DistractionTarget = this.StudentManager.Students[this.TeacherID];
		this.Pathfinding.target = this.DistractionTarget.gameObject.transform;
		this.CurrentDestination = this.DistractionTarget.gameObject.transform;
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.TargetDistance = 0.75f;
		this.DistractTimer = 10f;
		this.Distracting = true;
		this.CanTalk = false;
		this.Routine = false;
		this.Attempts = 0;
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x00201680 File Offset: 0x001FF880
	private void PhoneAddictCameraUpdate()
	{
		if (this.SmartPhone.transform.parent != null)
		{
			this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
			if (!this.StudentManager.Eighties)
			{
				this.SmartPhone.transform.localPosition = new Vector3(0f, 0.005f, -0.01f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(7.33333f, -154f, 173.66666f);
			}
			else
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.085f, -0.0015f, 0.003f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(-10f, 30f, 165f);
			}
			this.SmartPhone.SetActive(true);
			if (this.Sleuthing)
			{
				if (this.AlarmTimer < 2f)
				{
					this.AlarmTimer = 2f;
					this.ScaredAnim = this.SleuthReactAnim;
					this.SprintAnim = this.SleuthReportAnim;
					this.CharacterAnimation.CrossFade(this.ScaredAnim);
				}
				if (!this.CameraFlash.activeInHierarchy && this.CharacterAnimation[this.ScaredAnim].time > 2f)
				{
					this.CameraFlash.SetActive(true);
					if (this.Yandere.Mask != null)
					{
						this.Countdown.MaskedPhoto = true;
						return;
					}
				}
			}
			else
			{
				if (!this.StudentManager.Eighties)
				{
					this.ScaredAnim = this.PhoneAnims[4];
				}
				this.CharacterAnimation.CrossFade(this.ScaredAnim);
				if (!this.CameraFlash.activeInHierarchy && (double)this.CharacterAnimation[this.ScaredAnim].time > 3.66666)
				{
					this.CameraFlash.SetActive(true);
					if (this.Yandere.Mask != null)
					{
						this.Countdown.MaskedPhoto = true;
						return;
					}
					if (this.Grudge)
					{
						this.Police.PhotoEvidence++;
						this.PhotoEvidence = true;
					}
				}
			}
		}
	}

	// Token: 0x060021C2 RID: 8642 RVA: 0x002018B4 File Offset: 0x001FFAB4
	private void ReturnToRoutine()
	{
		Debug.Log(this.Name + " is now calling ReturnToRoutine.");
		if (this.Actions[this.Phase] == StudentActionType.Patrol || (this.Actions[this.Phase] == StudentActionType.ClubAction && this.Club == ClubType.Gardening))
		{
			this.CurrentDestination = this.StudentManager.Patrols.List[this.StudentID].GetChild(this.PatrolID);
			this.Pathfinding.target = this.CurrentDestination;
		}
		else
		{
			this.CurrentDestination = this.Destinations[this.Phase];
			this.Pathfinding.target = this.Destinations[this.Phase];
		}
		if (this.BreakingUpFight)
		{
			this.SetOutlineColor(new Color(1f, 1f, 0f, 1f));
		}
		else
		{
			this.SetOutlineColor(new Color(1f, 0.5f, 0f, 1f));
		}
		if (this.Yandere.Pursuer == this)
		{
			this.Yandere.Pursuer = null;
		}
		this.BreakingUpFight = false;
		this.WitnessedMurder = false;
		this.Prompt.enabled = true;
		this.Alarmed = false;
		this.Fleeing = false;
		this.Routine = true;
		this.Grudge = false;
		this.Pathfinding.speed = this.WalkSpeed;
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x00201A18 File Offset: 0x001FFC18
	public void EmptyHands()
	{
		bool flag = false;
		if ((this.SentHome && this.SmartPhone.activeInHierarchy) || this.PhotoEvidence || (this.Persona == PersonaType.PhoneAddict && !this.Dying && !this.Wet))
		{
			flag = true;
		}
		if (this.BloodPool != null && this.BloodPool.transform.parent == this.ItemParent && this.BloodPool.gameObject.GetComponent<WeaponScript>() != null)
		{
			this.BloodPool.gameObject.GetComponent<WeaponScript>().Drop();
			this.BloodPool.gameObject.GetComponent<WeaponScript>().enabled = true;
		}
		if (this.MyPlate != null && this.MyPlate.parent != null)
		{
			if (this.WitnessedMurder || this.WitnessedCorpse)
			{
				this.DropPlate();
			}
			else
			{
				this.MyPlate.gameObject.SetActive(false);
			}
		}
		if (this.Club == ClubType.Gardening && (!this.StudentManager.Eighties || this.WaterLow))
		{
			this.WateringCan.transform.parent = this.Hips;
			this.WateringCan.transform.localPosition = new Vector3(0f, 0.0135f, -0.184f);
			this.WateringCan.transform.localEulerAngles = new Vector3(0f, 90f, 30f);
		}
		if (this.Club == ClubType.LightMusic)
		{
			if (this.StudentID == 51)
			{
				if (this.InstrumentBag[this.ClubMemberID].transform.parent == null)
				{
					this.Instruments[this.ClubMemberID].transform.parent = null;
					if (!this.StudentManager.Eighties)
					{
						this.Instruments[this.ClubMemberID].transform.position = new Vector3(-0.5f, 4.5f, 22.45666f);
						this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, 0f, 0f);
					}
					else
					{
						this.Instruments[this.ClubMemberID].transform.position = new Vector3(2.105f, 4.5f, 25.5f);
						this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
					}
					this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().playOnAwake = false;
					this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Stop();
				}
				else
				{
					this.Instruments[this.ClubMemberID].SetActive(false);
				}
			}
			else
			{
				this.Instruments[this.ClubMemberID].SetActive(false);
			}
			this.Drumsticks[0].SetActive(false);
			this.Drumsticks[1].SetActive(false);
			this.AirGuitar.Stop();
		}
		if (!this.Male)
		{
			this.PicnicProps[0].SetActive(false);
			this.PicnicProps[1].SetActive(false);
			this.PicnicProps[2].SetActive(false);
			this.Handkerchief.SetActive(false);
			this.GiftBag.SetActive(false);
		}
		else
		{
			this.PinkSeifuku.SetActive(false);
		}
		if (!flag)
		{
			this.SmartPhone.SetActive(false);
		}
		if (this.BagOfChips != null)
		{
			this.BagOfChips.SetActive(false);
		}
		this.Chopsticks[0].SetActive(false);
		this.Chopsticks[1].SetActive(false);
		this.Sketchbook.SetActive(false);
		this.OccultBook.SetActive(false);
		this.Paintbrush.SetActive(false);
		this.Cigarette.SetActive(false);
		this.EventBook.SetActive(false);
		this.Scrubber.SetActive(false);
		this.Drawing.SetActive(false);
		this.Lighter.SetActive(false);
		this.Octodog.SetActive(false);
		this.Palette.SetActive(false);
		this.Eraser.SetActive(false);
		this.Pencil.SetActive(false);
		this.Pen.SetActive(false);
		if (this.Bento.transform.parent != null)
		{
			this.Bento.SetActive(false);
		}
		if (this.TrashDestination != null && this.TrashDestination.parent == this.ItemParent)
		{
			Debug.Log("Attempting to drop trash bag.");
			this.TrashDestination.gameObject.GetComponent<PickUpScript>().DoNotRelocate = true;
			this.TrashDestination.gameObject.GetComponent<PickUpScript>().Drop();
		}
		foreach (GameObject gameObject in this.ScienceProps)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		foreach (GameObject gameObject2 in this.Fingerfood)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
		}
		this.BountyCollider.SetActive(false);
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x00201F3C File Offset: 0x0020013C
	public void UpdateAnimLayers()
	{
		this.CharacterAnimation[this.ConfusedSitAnim].speed += (float)this.StudentID * 0.01f;
		this.CharacterAnimation[this.WalkAnim].time = UnityEngine.Random.Range(0f, this.CharacterAnimation[this.WalkAnim].length);
		this.CharacterAnimation[this.WetAnim].layer = 9;
		this.CharacterAnimation.Play(this.WetAnim);
		this.CharacterAnimation[this.WetAnim].weight = 0f;
		if (!this.Male)
		{
			this.CharacterAnimation[this.StripAnim].speed = 1.5f;
			this.CharacterAnimation[this.GameAnim].speed = 2f;
			this.CharacterAnimation["f02_casualWave_04"].layer = 11;
			this.CharacterAnimation["f02_casualWave_04"].weight = 0f;
			this.CharacterAnimation["f02_friendWave_00"].layer = 10;
			this.CharacterAnimation["f02_friendWave_00"].weight = 0f;
			this.CharacterAnimation["f02_moLipSync_00"].layer = 9;
			this.CharacterAnimation.Play("f02_moLipSync_00");
			this.CharacterAnimation["f02_moLipSync_00"].weight = 0f;
			this.CharacterAnimation["f02_topHalfTexting_00"].layer = 8;
			this.CharacterAnimation.Play("f02_topHalfTexting_00");
			this.CharacterAnimation["f02_topHalfTexting_00"].weight = 0f;
			this.CharacterAnimation[this.CarryAnim].layer = 7;
			this.CharacterAnimation.Play(this.CarryAnim);
			this.CharacterAnimation[this.CarryAnim].weight = 0f;
			this.CharacterAnimation[this.SocialSitAnim].layer = 6;
			this.CharacterAnimation.Play(this.SocialSitAnim);
			this.CharacterAnimation[this.SocialSitAnim].weight = 0f;
			this.CharacterAnimation[this.ShyAnim].layer = 5;
			this.CharacterAnimation.Play(this.ShyAnim);
			this.CharacterAnimation[this.ShyAnim].weight = 0f;
			this.CharacterAnimation[this.FistAnim].layer = 4;
			this.CharacterAnimation[this.FistAnim].weight = 0f;
			this.CharacterAnimation[this.BentoAnim].layer = 3;
			this.CharacterAnimation.Play(this.BentoAnim);
			this.CharacterAnimation[this.BentoAnim].weight = 0f;
			this.CharacterAnimation[this.AngryFaceAnim].layer = 2;
			this.CharacterAnimation.Play(this.AngryFaceAnim);
			this.CharacterAnimation[this.AngryFaceAnim].weight = 0f;
			this.CharacterAnimation["f02_wetIdle_00"].speed = 1.25f;
			this.CharacterAnimation["f02_sleuthScan_00"].speed = 1.4f;
			this.BoobsResized = false;
		}
		else
		{
			this.CharacterAnimation[this.ConfusedSitAnim].speed *= -1f;
			this.CharacterAnimation["f02_casualWave_04"].layer = 10;
			this.CharacterAnimation["f02_casualWave_04"].weight = 0f;
			this.CharacterAnimation["friendWave_00"].layer = 9;
			this.CharacterAnimation["friendWave_00"].weight = 0f;
			if (!this.StudentManager.Eighties && this.StudentID == 37)
			{
				this.CharacterAnimation["lockEyebrows_00"].layer = 8;
				this.CharacterAnimation.Play("lockEyebrows_00");
				this.CharacterAnimation["lockEyebrows_00"].weight = 1f;
			}
			this.CharacterAnimation[this.ToughFaceAnim].layer = 7;
			this.CharacterAnimation.Play(this.ToughFaceAnim);
			this.CharacterAnimation[this.ToughFaceAnim].weight = 0f;
			this.CharacterAnimation[this.SocialSitAnim].layer = 6;
			this.CharacterAnimation.Play(this.SocialSitAnim);
			this.CharacterAnimation[this.SocialSitAnim].weight = 0f;
			this.CharacterAnimation[this.CarryShoulderAnim].layer = 5;
			this.CharacterAnimation.Play(this.CarryShoulderAnim);
			this.CharacterAnimation[this.CarryShoulderAnim].weight = 0f;
			this.CharacterAnimation["scaredFace_00"].layer = 4;
			this.CharacterAnimation.Play("scaredFace_00");
			this.CharacterAnimation["scaredFace_00"].weight = 0f;
			this.CharacterAnimation[this.SadFaceAnim].layer = 3;
			this.CharacterAnimation.Play(this.SadFaceAnim);
			this.CharacterAnimation[this.SadFaceAnim].weight = 0f;
			this.CharacterAnimation[this.AngryFaceAnim].layer = 2;
			this.CharacterAnimation.Play(this.AngryFaceAnim);
			this.CharacterAnimation[this.AngryFaceAnim].weight = 0f;
			this.CharacterAnimation["sleuthScan_00"].speed = 1.4f;
		}
		if (this.Persona == PersonaType.Sleuth)
		{
			this.CharacterAnimation[this.WalkAnim].time = UnityEngine.Random.Range(0f, this.CharacterAnimation[this.WalkAnim].length);
		}
		if (this.Club == ClubType.Bully)
		{
			if (!StudentGlobals.GetStudentBroken(this.StudentID) && this.BullyID > 1)
			{
				this.CharacterAnimation["f02_bullyLaugh_00"].speed = 0.9f + (float)this.BullyID * 0.1f;
			}
		}
		else if (this.Club == ClubType.Delinquent)
		{
			this.CharacterAnimation[this.WalkAnim].time = UnityEngine.Random.Range(0f, this.CharacterAnimation[this.WalkAnim].length);
			this.CharacterAnimation[this.LeanAnim].speed = 0.5f;
		}
		else if (this.Club == ClubType.Council)
		{
			if (!this.StudentManager.Eighties)
			{
				this.CharacterAnimation["f02_faceCouncil" + this.Suffix + "_00"].layer = 10;
				this.CharacterAnimation.Play("f02_faceCouncil" + this.Suffix + "_00");
			}
		}
		else if (this.Club == ClubType.Gaming)
		{
			this.CharacterAnimation[this.VictoryAnim].speed -= 0.1f * (float)(this.StudentID - 36);
			this.CharacterAnimation[this.VictoryAnim].speed = 0.866666f;
		}
		else if (this.Club == ClubType.Cooking && this.ClubActivityPhase > 0)
		{
			this.WalkAnim = this.PlateWalkAnim;
		}
		if (!this.StudentManager.Eighties)
		{
			if (this.StudentID == 36)
			{
				this.CharacterAnimation[this.ToughFaceAnim].weight = 1f;
			}
			else if (this.StudentID == 66)
			{
				this.CharacterAnimation[this.ToughFaceAnim].weight = 1f;
			}
		}
		if (this.Tranquil)
		{
			Debug.Log("This character was tranquilized at the point in time when UpdateAnimLayers() was called.");
			this.CharacterAnimation.Play("f02_carryDisposeB_00");
			this.CharacterAnimation["f02_carryDisposeB_00"].time = this.CharacterAnimation["f02_carryDisposeB_00"].length;
		}
		if (this.Ragdoll.enabled)
		{
			Debug.Log("This character was dead at the point in time when UpdateAnimLayers() was called.");
			if (this.Ragdoll.Concealed)
			{
				if (this.Ragdoll.InsideIncinerator)
				{
					Debug.Log("This character was wrapped in a garbage bag and inside of an incinerator at the point in time when UpdateAnimLayers() was called.");
					if (this.Male)
					{
						this.CharacterAnimation.Play("carryDisposeB_00");
						this.CharacterAnimation["carryDisposeB_00"].time = this.CharacterAnimation["carryDisposeB_00"].length;
					}
					else
					{
						this.CharacterAnimation.Play("f02_carryDisposeB_00");
						this.CharacterAnimation["f02_carryDisposeB_00"].time = this.CharacterAnimation["f02_carryDisposeB_00"].length;
					}
					base.gameObject.SetActive(false);
				}
			}
			else
			{
				this.Ragdoll.EnableRigidbodies();
			}
		}
		if (this.Ragdoll.InsideIncinerator)
		{
			Debug.Log("This character was wrapped in a garbage bag and inside of an incinerator at the point in time when UpdateAnimLayers() was called.");
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x002028D4 File Offset: 0x00200AD4
	private void SpawnDetectionMarker()
	{
		this.DetectionMarker = UnityEngine.Object.Instantiate<GameObject>(this.Marker, this.Yandere.DetectionPanel.transform.position, Quaternion.identity).GetComponent<DetectionMarkerScript>();
		if (this.StudentID == 1)
		{
			this.DetectionMarker.GetComponent<DetectionMarkerScript>().Tex.color = new Color(1f, 0f, 0f, 0f);
		}
		this.DetectionMarker.transform.parent = this.Yandere.DetectionPanel.transform;
		this.DetectionMarker.Target = base.transform;
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x0020297C File Offset: 0x00200B7C
	public void EquipCleaningItems()
	{
		if (this.CurrentAction == StudentActionType.Clean)
		{
			if (!this.Phoneless && (this.Persona == PersonaType.PhoneAddict || this.Persona == PersonaType.Sleuth))
			{
				this.WalkAnim = this.OriginalWalkAnim;
			}
			this.SmartPhone.SetActive(false);
			this.Scrubber.SetActive(true);
			if (this.CleaningRole == 5)
			{
				this.Scrubber.GetComponent<Renderer>().material.mainTexture = this.Eraser.GetComponent<Renderer>().material.mainTexture;
				this.Eraser.SetActive(true);
			}
			if (this.StudentID == 9 || this.StudentID == 60)
			{
				this.WalkAnim = this.OriginalOriginalWalkAnim;
			}
		}
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x00202A38 File Offset: 0x00200C38
	public void DetermineWhatWasWitnessed()
	{
		Debug.Log("We are now determining what " + this.Name + " witnessed.");
		if (this.Witnessed == StudentWitnessType.Murder)
		{
			Debug.Log("No need to go through the entire chain. We already know that this character witnessed murder.");
			this.Concern = 5;
		}
		else if (this.YandereVisible)
		{
			Debug.Log(this.Name + " can see Yandere-chan right now.");
			bool flag = false;
			bool flag2 = false;
			if (this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && !this.Yandere.Paint)
			{
				flag = true;
			}
			if (this.Yandere.Club == ClubType.Art && this.Yandere.ClubAttire)
			{
				flag = false;
				flag2 = true;
			}
			if (this.Yandere.Armed)
			{
				this.Yandere.EquippedWeapon.SuspicionCheck();
			}
			bool flag3 = this.Yandere.Armed && this.Yandere.EquippedWeapon.Suspicious;
			bool flag4 = this.Yandere.PickUp != null && this.Yandere.PickUp.Suspicious;
			bool flag5 = this.Yandere.PickUp != null && this.Yandere.PickUp.BodyPart != null;
			bool flag6 = this.Yandere.PickUp != null && this.Yandere.PickUp.Clothing && this.Yandere.PickUp.Evidence;
			bool flag7 = false;
			if ((!this.StudentManager.Eighties && this.StudentID == 48 && this.TaskPhase == 4 && this.Yandere.Armed && this.Yandere.EquippedWeapon.WeaponID == 12) || (!this.StudentManager.Eighties && this.StudentID == 50 && this.TaskPhase == 4 && this.Yandere.Armed && this.Yandere.EquippedWeapon.WeaponID == 24))
			{
				flag3 = false;
				flag7 = true;
			}
			int concern = this.Concern;
			if (flag3)
			{
				this.WeaponToTakeAway = this.Yandere.EquippedWeapon;
			}
			if (flag3)
			{
				Debug.Log(this.Name + " saw the player carrying a suspicious weapon.");
			}
			bool flag8 = false;
			if (this.Yandere.Club == ClubType.Occult && this.Yandere.OccultRobe)
			{
				flag8 = true;
			}
			if (this.Yandere.Rummaging || this.Yandere.TheftTimer > 0f)
			{
				Debug.Log("Saw Yandere-chan stealing.");
				this.Witnessed = StudentWitnessType.Theft;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (this.Yandere.Pickpocketing || this.Yandere.Caught)
			{
				Debug.Log("Saw Yandere-chan pickpocketing.");
				this.Witnessed = StudentWitnessType.Pickpocketing;
				this.RepLoss = 10f;
				this.Concern = 5;
				this.Yandere.StudentManager.PickpocketMinigame.Failure = true;
				this.Yandere.StudentManager.PickpocketMinigame.End();
				this.Yandere.Caught = true;
				if (this.Teacher)
				{
					this.Witnessed = StudentWitnessType.Theft;
				}
			}
			else if (flag3 && this.Yandere.Bloodiness > 0f && this.Yandere.Sanity < 33.333f)
			{
				Debug.Log("Saw Yandere-chan armed, bloody, and insane.");
				this.TimesWeaponWitnessed++;
				this.TimesBloodWitnessed++;
				this.Witnessed = StudentWitnessType.WeaponAndBloodAndInsanity;
				this.RepLoss = 30f;
				this.Concern = 5;
			}
			else if (flag3 && this.Yandere.Sanity < 33.333f)
			{
				Debug.Log("Saw Yandere-chan armed and insane.");
				this.TimesWeaponWitnessed++;
				this.Witnessed = StudentWitnessType.WeaponAndInsanity;
				this.RepLoss = 20f;
				this.Concern = 5;
			}
			else if (flag && this.Yandere.Sanity < 33.333f)
			{
				Debug.Log("Saw Yandere-chan bloody, and insane.");
				this.TimesBloodWitnessed++;
				this.Witnessed = StudentWitnessType.BloodAndInsanity;
				this.RepLoss = 20f;
				this.Concern = 5;
			}
			else if (flag3 && this.Yandere.Bloodiness > 0f)
			{
				Debug.Log("Saw Yandere-chan armed and bloody.");
				this.TimesWeaponWitnessed++;
				this.TimesBloodWitnessed++;
				this.Witnessed = StudentWitnessType.WeaponAndBlood;
				this.RepLoss = 20f;
				this.Concern = 5;
			}
			else if (flag3)
			{
				Debug.Log("Saw Yandere-chan with a suspicious weapon.");
				this.TimesWeaponWitnessed++;
				this.WeaponWitnessed = this.Yandere.EquippedWeapon.WeaponID;
				this.PlayerHeldBloodyWeapon = this.Yandere.EquippedWeapon.Bloody;
				this.Witnessed = StudentWitnessType.Weapon;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (flag4)
			{
				Debug.Log("Saw Yandere-chan with a suspicious object.");
				if (this.Yandere.PickUp.CleaningProduct)
				{
					if (this.StudentID == 1)
					{
						this.Witnessed = StudentWitnessType.Lewd;
					}
					else
					{
						this.Witnessed = StudentWitnessType.CleaningItem;
					}
				}
				else if (this.Teacher || this.Club == ClubType.Council)
				{
					this.Witnessed = StudentWitnessType.Insanity;
				}
				else
				{
					this.Witnessed = StudentWitnessType.Suspicious;
				}
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (this.Yandere.Bloodiness > 0f && !flag2)
			{
				Debug.Log("Saw Yandere-chan splattered with blood.");
				this.TimesBloodWitnessed++;
				this.Witnessed = StudentWitnessType.Blood;
				if (!this.Bloody)
				{
					this.RepLoss = 10f;
					this.Concern = 5;
				}
				else
				{
					this.RepLoss = 0f;
					this.Concern = 0;
				}
			}
			else if (this.Yandere.Sanity < 33.333f)
			{
				Debug.Log("Saw Yandere-chan acting insane.");
				this.Witnessed = StudentWitnessType.Insanity;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (this.Yandere.Lewd)
			{
				Debug.Log("Saw Yandere-chan being lewd.");
				this.Witnessed = StudentWitnessType.Lewd;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if ((this.Yandere.Laughing && this.Yandere.LaughIntensity > 15f) || (this.StudentID > 1 && this.Yandere.Stance.Current == StanceType.Crouching) || (this.Yandere.Stance.Current == StanceType.Crawling || this.Yandere.SuspiciousActionTimer > 0f || (this.Yandere.WearingRaincoat && !flag8)) || (this.Yandere.Lockpicking || (this.Yandere.Carrying && this.Yandere.CurrentRagdoll.Concealed)) || (this.Yandere.Dragging && this.Yandere.CurrentRagdoll.Concealed) || (this.Yandere.Schoolwear == 2 && this.Yandere.transform.position.z < 30f) || (this.AnnoyedByRadio > 1 && this.Yandere.AnnoyingGiggleTimer > 0f) || (this.Yandere.PreparingThrow && this.Yandere.Obvious))
			{
				if (this.StudentID == 1 && !this.Yandere.Laughing && this.Yandere.Sanity > 33f)
				{
					Debug.Log("Saw Yandere-chan being lewd.");
					this.Witnessed = StudentWitnessType.Lewd;
				}
				else
				{
					Debug.Log("Saw Yandere-chan acting insane.");
					this.Witnessed = StudentWitnessType.Insanity;
				}
				this.RepLoss = 10f;
				if (this.Yandere.Stance.Current == StanceType.Crouching)
				{
					this.AnnoyedByGiggles++;
				}
				if ((this.Yandere.Laughing && this.Yandere.LaughIntensity > 15f) || this.Yandere.BreakingGlass)
				{
					this.Concern = 5;
				}
				else if (this.AnnoyedByGiggles > 4)
				{
					if (this.StudentID == 1 && this.AnnoyedByRadio > 1 && this.Yandere.PotentiallyAnnoyingTimer > 0f)
					{
						this.Concern++;
					}
					else
					{
						this.Concern = 5;
					}
				}
				else
				{
					this.Concern++;
				}
			}
			else if (this.Yandere.Poisoning)
			{
				Debug.Log("Saw Yandere-chan poisoning a bento.");
				this.Witnessed = StudentWitnessType.Poisoning;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (this.Yandere.Trespassing && this.StudentID > 1)
			{
				Debug.Log("Saw Yandere-chan trespassing.");
				this.Witnessed = (this.Private ? StudentWitnessType.Interruption : StudentWitnessType.Trespassing);
				this.Witness = true;
				if (!this.Teacher)
				{
					this.RepLoss = 10f;
				}
				this.Concern++;
			}
			else if (this.Yandere.NearSenpai || (this.StudentID == 1 && this.Yandere.Stance.Current == StanceType.Crouching))
			{
				if (this.StudentID == 1)
				{
					Debug.Log("Saw Yandere-chan stalking.");
					this.Witnessed = StudentWitnessType.Stalking;
				}
				else
				{
					Debug.Log("Saw Yandere-chan acting insane.");
					this.Witnessed = StudentWitnessType.Insanity;
					this.RepLoss = 10f;
				}
				this.Concern++;
			}
			else if (this.Yandere.Eavesdropping)
			{
				if (this.StudentID == 1)
				{
					Debug.Log("Saw Yandere-chan stalking.");
					this.Witnessed = StudentWitnessType.Stalking;
					this.Concern++;
				}
				else
				{
					if (this.InEvent)
					{
						this.EventInterrupted = true;
					}
					Debug.Log("Saw Yandere-chan eavesdropping.");
					this.Witnessed = StudentWitnessType.Eavesdropping;
					this.RepLoss = 10f;
					this.Concern = 5;
				}
			}
			else if (this.Yandere.Aiming)
			{
				Debug.Log("Saw Yandere-chan stalking.");
				this.Witnessed = StudentWitnessType.Stalking;
				this.Concern++;
			}
			else if (this.Yandere.DelinquentFighting)
			{
				Debug.Log("Saw Yandere-chan fighting a delinquent.");
				this.Witnessed = StudentWitnessType.Violence;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (this.Yandere.PickUp != null && this.Yandere.PickUp.Clothing && this.Yandere.PickUp.Evidence)
			{
				Debug.Log("Saw Yandere-chan with bloody clothing.");
				this.Witnessed = StudentWitnessType.HoldingBloodyClothing;
				this.RepLoss = 10f;
				this.Concern = 5;
			}
			else if (flag5 || flag6)
			{
				Debug.Log("Saw Yandere-chan attempting to cover up a murder.");
				this.Witnessed = StudentWitnessType.CoverUp;
			}
			else if (flag7)
			{
				if (this.StudentID == 48)
				{
					this.Subtitle.CustomText = "Is that dumbbell for me? Drop it over here!";
				}
				else if (this.StudentID == 50)
				{
					this.Subtitle.CustomText = "Are you going to use that pipe wrench to fix the training dummy?";
				}
				this.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
			}
			else if (this.Yandere.LaughTimer > 0f)
			{
				this.Witnessed = StudentWitnessType.Insanity;
				this.RepLoss = 10f;
				this.Concern++;
			}
			if ((this.StudentID == 1 || this.Club == ClubType.Council) && this.Witnessed == StudentWitnessType.Insanity && (this.Yandere.Stance.Current == StanceType.Crouching || this.Yandere.Stance.Current == StanceType.Crawling))
			{
				Debug.Log("Saw Yandere-chan stalking.");
				this.Witnessed = StudentWitnessType.Stalking;
				this.Concern = concern;
				this.Concern++;
			}
		}
		else
		{
			Debug.Log(this.Name + " is reacting to something other than Yandere-chan.");
			if (this.WitnessedLimb)
			{
				this.Witnessed = StudentWitnessType.SeveredLimb;
			}
			else if (this.WitnessedBloodyWeapon)
			{
				this.Witnessed = StudentWitnessType.BloodyWeapon;
			}
			else if (this.WitnessedBloodPool)
			{
				this.Witnessed = StudentWitnessType.BloodPool;
			}
			else if (this.WitnessedWeapon)
			{
				this.Witnessed = StudentWitnessType.DroppedWeapon;
			}
			else if (this.WitnessedCorpse)
			{
				this.Witnessed = StudentWitnessType.Corpse;
			}
			else
			{
				Debug.Log(this.Name + " was alarmed by something, but didn't see what it was. DiscCheck is being set to true.");
				this.Witnessed = StudentWitnessType.None;
				this.DiscCheck = true;
				this.Witness = false;
			}
		}
		if (this.Concern == 5 && this.Club == ClubType.Council && this.Yandere.Pursuer == null)
		{
			Debug.Log("A member of the student council is being transformed into a teacher.");
			this.Teacher = true;
		}
		if (this.StudentID > 1 && this.Witnessed != StudentWitnessType.None)
		{
			this.SetOutlineColor(new Color(1f, 1f, 0f, 1f));
		}
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x00203724 File Offset: 0x00201924
	public void DetermineTeacherSubtitle()
	{
		Debug.Log("We are now determining what line of dialogue the teacher should say.");
		if (this.Club == ClubType.Council)
		{
			this.Subtitle.UpdateLabel(SubtitleType.CouncilToCounselor, this.ClubMemberID, 5f);
			return;
		}
		bool flag = false;
		if (this.Yandere.Club == ClubType.Occult && this.Yandere.OccultRobe)
		{
			flag = true;
		}
		if (this.Guarding && this.YandereVisible)
		{
			Debug.Log("Teacher reached this code while guarding and able to see Yandere-chan.");
			if (this.Yandere.Bloodiness + (float)this.Yandere.GloveBlood > 0f && !this.Yandere.Paint)
			{
				this.Witnessed = StudentWitnessType.Blood;
			}
			else if (this.Yandere.Armed)
			{
				this.Witnessed = StudentWitnessType.Weapon;
			}
			else if (this.Yandere.Sanity < 66.66666f || (this.Yandere.WearingRaincoat && !flag) || this.Yandere.Lockpicking)
			{
				this.Witnessed = StudentWitnessType.Insanity;
			}
		}
		if (this.Witnessed == StudentWitnessType.Murder)
		{
			if (this.WitnessedMindBrokenMurder)
			{
				this.Subtitle.UpdateLabel(SubtitleType.TeacherMurderReaction, 4, 6f);
			}
			else
			{
				this.Subtitle.UpdateLabel(SubtitleType.TeacherMurderReaction, 1, 6f);
			}
			this.GameOverCause = GameOverType.Murder;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.WeaponAndBloodAndInsanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherInsanityHostile, 1, 6f);
			this.GameOverCause = GameOverType.Insanity;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.WeaponAndBlood)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherWeaponHostile, 1, 6f);
			this.GameOverCause = GameOverType.Weapon;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.WeaponAndInsanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherInsanityHostile, 1, 6f);
			this.GameOverCause = GameOverType.Insanity;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.BloodAndInsanity)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherInsanityHostile, 1, 6f);
			this.GameOverCause = GameOverType.Insanity;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.Weapon)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherWeaponHostile, 1, 6f);
			this.GameOverCause = GameOverType.Weapon;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.Blood)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherBloodHostile, 1, 6f);
			this.GameOverCause = GameOverType.Blood;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.Insanity || this.Witnessed == StudentWitnessType.Poisoning)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherInsanityHostile, 1, 6f);
			this.GameOverCause = GameOverType.Insanity;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.Lewd)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherLewdReaction, 1, 6f);
			this.GameOverCause = GameOverType.Lewd;
			return;
		}
		if (this.Witnessed == StudentWitnessType.Trespassing)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherTrespassingReaction, this.Concern, 5f);
			return;
		}
		if (this.Witnessed == StudentWitnessType.Corpse)
		{
			Debug.Log(this.Name + " just discovered a corpse and called the cops.");
			this.DetermineCorpseLocation();
			this.Subtitle.UpdateLabel(SubtitleType.TeacherCorpseReaction, 1, 3f);
			this.Police.Called = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.CoverUp)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherCoverUpHostile, 1, 6f);
			this.GameOverCause = GameOverType.Blood;
			this.WitnessedMurder = true;
			return;
		}
		if (this.Witnessed == StudentWitnessType.CleaningItem)
		{
			this.Subtitle.UpdateLabel(SubtitleType.TeacherInsanityReaction, 1, 6f);
			this.GameOverCause = GameOverType.Insanity;
		}
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x00203AAC File Offset: 0x00201CAC
	public void ReturnMisplacedWeapon()
	{
		Debug.Log(this.Name + " has returned a misplaced weapon.");
		this.StopInvestigating();
		if (this.StudentManager.BloodReporter == this)
		{
			this.StudentManager.BloodReporter = null;
		}
		this.BloodPool.parent = null;
		this.BloodPool.position = this.BloodPool.GetComponent<WeaponScript>().StartingPosition;
		this.BloodPool.eulerAngles = this.BloodPool.GetComponent<WeaponScript>().StartingRotation;
		this.BloodPool.GetComponent<WeaponScript>().Prompt.enabled = true;
		this.BloodPool.GetComponent<WeaponScript>().enabled = true;
		this.BloodPool.GetComponent<WeaponScript>().DoNotRelocate = true;
		this.BloodPool.GetComponent<WeaponScript>().Drop();
		this.BloodPool.GetComponent<WeaponScript>().MyRigidbody.useGravity = false;
		this.BloodPool.GetComponent<WeaponScript>().MyRigidbody.isKinematic = true;
		this.BloodPool.GetComponent<WeaponScript>().Returner = null;
		this.BloodPool = null;
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		if (this.CurrentAction == StudentActionType.Sunbathe && this.SunbathePhase > 1)
		{
			Debug.Log(this.Name + " was sunbathing at the time.");
			this.CurrentDestination = this.StudentManager.SunbatheSpots[this.StudentID];
			this.Pathfinding.target = this.StudentManager.SunbatheSpots[this.StudentID];
		}
		if (this.ResumeFollowingAfter)
		{
			this.ResumeFollowing();
		}
		if (this.Club == ClubType.Council || this.Teacher)
		{
			this.Handkerchief.SetActive(false);
		}
		this.Pathfinding.speed = this.WalkSpeed;
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		this.ReturningMisplacedWeapon = false;
		this.WitnessedSomething = false;
		this.VerballyReacted = false;
		this.WitnessedWeapon = false;
		this.YandereInnocent = false;
		this.ReportingBlood = false;
		this.Distracted = false;
		this.Routine = true;
		this.ReturningMisplacedWeaponPhase = 0;
		this.WitnessCooldownTimer = 0f;
		this.Yandere.WeaponManager.ReturnWeaponID = -1;
		this.Yandere.WeaponManager.ReturnStudentID = -1;
		if (this.BeforeReturnAnim == "")
		{
			this.BeforeReturnAnim = this.OriginalWalkAnim;
		}
		this.WalkAnim = this.BeforeReturnAnim;
		this.Hurry = this.WasHurrying;
		Debug.Log(this.Name + "'s WalkAnim is now: " + this.WalkAnim);
		if (this.Sleuthing)
		{
			Debug.Log("The character who just returned a misplaced item was ''Sleuthing'' before they did, so they are using special logic...");
			if (this.SleuthTarget != null)
			{
				if (this.SleuthTarget.GetComponent<StudentScript>() == null)
				{
					Debug.Log("SleuthTarget was not a student!");
					this.GetSleuthTarget();
					return;
				}
				Debug.Log("SleuthTarget was a student!");
				this.CurrentDestination = this.SleuthTarget.transform;
				this.Pathfinding.target = this.SleuthTarget.transform;
				return;
			}
			else
			{
				Debug.Log("SleuthTarget was null!");
				this.GetSleuthTarget();
			}
		}
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x00203DD8 File Offset: 0x00201FD8
	public void StopMusic()
	{
		if (this.StudentID == 51)
		{
			if (this.InstrumentBag[this.ClubMemberID].transform.parent == null)
			{
				this.Instruments[this.ClubMemberID].transform.parent = null;
				if (!this.StudentManager.Eighties)
				{
					this.Instruments[this.ClubMemberID].transform.position = new Vector3(-0.5f, 4.5f, 22.45666f);
					this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, 0f, 0f);
				}
				else
				{
					this.Instruments[this.ClubMemberID].transform.position = new Vector3(2.105f, 4.5f, 25.5f);
					this.Instruments[this.ClubMemberID].transform.eulerAngles = new Vector3(-15f, -90f, 0f);
				}
				this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().playOnAwake = false;
				this.Instruments[this.ClubMemberID].GetComponent<AudioSource>().Stop();
			}
			else
			{
				this.Instruments[this.ClubMemberID].SetActive(false);
			}
		}
		else
		{
			this.Instruments[this.ClubMemberID].SetActive(false);
		}
		this.Drumsticks[0].SetActive(false);
		this.Drumsticks[1].SetActive(false);
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x00203F5C File Offset: 0x0020215C
	public void DropPuzzle()
	{
		this.PuzzleCube.enabled = true;
		this.PuzzleCube.DoNotRelocate = true;
		this.PuzzleCube.Drop();
		this.SolvingPuzzle = false;
		this.Distracted = false;
		this.PuzzleTimer = 0f;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x00203F9C File Offset: 0x0020219C
	public void ReturnToNormal()
	{
		Debug.Log(this.Name + " has been instructed to forget everything and return to normal.");
		if (this.SolvingPuzzle)
		{
			Debug.Log("Student was solving a puzzle at the time. Should be forgetting about the puzzle now.");
			this.PuzzleTimer = 0f;
			this.DropPuzzle();
		}
		if (this.StudentManager.Reporter == this)
		{
			this.StudentManager.CorpseLocation.position = Vector3.zero;
			this.StudentManager.Reporter = null;
		}
		else if (this.StudentManager.BloodReporter == this)
		{
			this.StudentManager.BloodLocation.position = Vector3.zero;
			this.StudentManager.BloodReporter = null;
		}
		if (this.Yandere.Pursuer == this)
		{
			this.Yandere.Pursuer = null;
			this.Yandere.PreparedForStruggle = false;
		}
		this.StudentManager.UpdateStudents(0);
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.Pathfinding.speed = this.WalkSpeed;
		this.TargetDistance = 1f;
		this.ReportPhase = 0;
		this.ReportTimer = 0f;
		this.AlarmTimer = 0f;
		this.AmnesiaTimer = 10f;
		if (this.Actions[this.Phase] != StudentActionType.ClubAction || this.Club != ClubType.Cooking || this.ClubActivityPhase <= 0)
		{
			this.RandomAnim = this.BulliedIdleAnim;
			this.IdleAnim = this.BulliedIdleAnim;
			this.WalkAnim = this.BulliedWalkAnim;
		}
		if (this.WitnessedBloodPool || this.WitnessedLimb || this.WitnessedWeapon)
		{
			this.Persona = this.OriginalPersona;
		}
		this.BloodPool = null;
		this.Corpse = null;
		this.Witnessed = StudentWitnessType.None;
		this.WitnessedBloodyWeapon = false;
		this.WitnessedBloodPool = false;
		this.WitnessedSomething = false;
		this.WitnessedCorpse = false;
		this.WitnessedMurder = false;
		this.WitnessedWeapon = false;
		this.WitnessedLimb = false;
		this.SmartPhone.SetActive(false);
		this.LostTeacherTrust = true;
		this.ReportingMurder = false;
		this.ReportingBlood = false;
		this.PinDownWitness = false;
		this.YandereVisible = false;
		this.AwareOfCorpse = false;
		this.HeardScream = false;
		this.Distracted = false;
		this.Reacted = false;
		this.Alarmed = false;
		this.Fleeing = false;
		this.Witness = false;
		this.Routine = true;
		this.Grudge = false;
		this.Halt = false;
		this.VisibleCorpses.Clear();
		if (this.Club == ClubType.Council)
		{
			this.Persona = PersonaType.Dangerous;
		}
		this.ID = 0;
		while (this.ID < this.Outlines.Length)
		{
			if (this.Outlines[this.ID] != null)
			{
				this.Outlines[this.ID].color = new Color(1f, 1f, 0f, 1f);
			}
			this.ID++;
		}
		this.Countdown.gameObject.SetActive(false);
		this.Countdown.Sprite.fillAmount = 1f;
		this.ChaseCamera.SetActive(false);
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x002042E4 File Offset: 0x002024E4
	public void ForgetAboutBloodPool()
	{
		Debug.Log(this.Name + " was told to ForgetAboutBloodPool()");
		this.Subtitle.UpdateLabel(SubtitleType.StudentFarewell, 0, 3f);
		if (this.Club == ClubType.Cooking && this.CurrentAction == StudentActionType.ClubAction && this.MyPlate != null && this.MyPlate.parent == this.RightHand)
		{
			this.GetFoodTarget();
		}
		else
		{
			this.CurrentDestination = this.Destinations[this.Phase];
			this.Pathfinding.target = this.Destinations[this.Phase];
		}
		this.InvestigatingBloodPool = false;
		this.WitnessedBloodyWeapon = false;
		this.WitnessedBloodPool = false;
		this.WitnessedSomething = false;
		this.WitnessedWeapon = false;
		this.Distracted = false;
		if (!this.Shoving)
		{
			this.Routine = true;
		}
		this.WitnessCooldownTimer = 5f;
		if (this.BloodPool != null && this.CanSeeObject(this.Yandere.gameObject, this.Yandere.HeadPosition) && this.BloodPool.parent == this.Yandere.RightHand)
		{
			this.YandereVisible = true;
			this.ReportTimer = 0f;
			this.ReportPhase = 0;
			this.Alarmed = false;
			this.Fleeing = false;
			this.Reacted = false;
			if (this.BloodPool.GetComponent<WeaponScript>() != null && this.BloodPool.GetComponent<WeaponScript>().Suspicious && !this.Yandere.Invisible)
			{
				Debug.Log(this.Name + " is about to call the BecomeAlarmed() function from the ForgetAboutBloodPool() function.");
				this.WitnessCooldownTimer = 5f;
				this.AlarmTimer = 0f;
				this.Alarm = 200f;
				this.BecomeAlarmed();
			}
		}
		if (this.BeforeReturnAnim != "")
		{
			this.WalkAnim = this.BeforeReturnAnim;
		}
		this.BloodPool = null;
		if (this.Giggle != null)
		{
			this.ForgetGiggle();
		}
		if (!this.ReturningMisplacedWeapon && this.Club == ClubType.Sports && this.CurrentAction == StudentActionType.ClubAction && this.ClubActivityPhase > 2 && this.ClubActivityPhase < 14)
		{
			Debug.Log("Student was jogging before they started investigating, and will now return to jogging.");
			this.Jog();
		}
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x00204530 File Offset: 0x00202730
	public void SimpleForgetAboutBloodPool()
	{
		this.InvestigatingBloodPool = false;
		this.WitnessedBloodyWeapon = false;
		this.WitnessedBloodPool = false;
		this.WitnessedSomething = false;
		this.WitnessedWeapon = false;
		this.Distracted = false;
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x0020455C File Offset: 0x0020275C
	private void SummonWitnessCamera()
	{
		if (this.WitnessCamera != null)
		{
			this.WitnessCamera.transform.parent = this.WitnessPOV;
			this.WitnessCamera.transform.localPosition = Vector3.zero;
			this.WitnessCamera.transform.localEulerAngles = Vector3.zero;
			this.WitnessCamera.MyCamera.enabled = true;
			this.WitnessCamera.Show = true;
		}
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x002045D4 File Offset: 0x002027D4
	public void SilentlyForgetBloodPool()
	{
		Debug.Log(this.Name + " was told to SilentlyForgetBloodPool()");
		this.InvestigatingBloodPool = false;
		this.WitnessedBloodyWeapon = false;
		this.WitnessedBloodPool = false;
		this.WitnessedSomething = false;
		this.WitnessedWeapon = false;
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x00204610 File Offset: 0x00202810
	private void CheckForEndRaibaruEvent()
	{
		if (this.StudentManager.Students[46] == null || this.StudentManager.Students[46].Phase > this.Phase)
		{
			Debug.Log("Raibaru has just finished mentoring the Martial Arts Club.");
			if (this.FollowTarget != null)
			{
				if (this.FollowTarget.Alive)
				{
					this.Destinations[this.Phase] = this.FollowTarget.transform;
					this.Pathfinding.target = this.FollowTarget.transform;
					this.CurrentDestination = this.FollowTarget.transform;
				}
				else
				{
					this.Destinations[this.Phase] = this.StudentManager.LastKnownOsana;
					this.Pathfinding.target = this.StudentManager.LastKnownOsana;
					this.CurrentDestination = this.StudentManager.LastKnownOsana;
				}
				this.FollowTarget.Follower = this;
				this.Actions[this.Phase] = StudentActionType.Follow;
				this.CurrentAction = StudentActionType.Follow;
			}
			else
			{
				this.Destinations[this.Phase] = this.StudentManager.MournSpots[this.StudentID];
				this.Pathfinding.target = this.StudentManager.MournSpots[this.StudentID];
				this.CurrentDestination = this.StudentManager.MournSpots[this.StudentID];
				this.Actions[this.Phase] = StudentActionType.Mourn;
				this.CurrentAction = StudentActionType.Mourn;
			}
			this.SpeechLines.Stop();
			this.InEvent = false;
			this.NoMentor = true;
			this.Routine = true;
		}
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x002047AC File Offset: 0x002029AC
	private void RaibaruOsanaDeathScheduleChanges()
	{
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[1];
		scheduleBlock.destination = "Mourn";
		scheduleBlock.action = "Mourn";
		ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[2];
		scheduleBlock2.destination = "Mourn";
		scheduleBlock2.action = "Mourn";
		ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[4];
		scheduleBlock3.destination = "LunchSpot";
		scheduleBlock3.action = "Eat";
		this.Persona = PersonaType.Heroic;
		this.IdleAnim = this.BulliedIdleAnim;
		this.WalkAnim = this.BulliedWalkAnim;
		this.OriginalIdleAnim = this.IdleAnim;
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x0020483C File Offset: 0x00202A3C
	private void RaibaruStopsFollowingOsana()
	{
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[3];
		scheduleBlock.destination = "Seat";
		scheduleBlock.action = "Sit";
		ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[5];
		scheduleBlock2.destination = "Seat";
		scheduleBlock2.action = "Sit";
		ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[6];
		scheduleBlock3.destination = "Locker";
		scheduleBlock3.action = "Shoes";
		ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[7];
		scheduleBlock4.destination = "Exit";
		scheduleBlock4.action = "Exit";
		ScheduleBlock scheduleBlock5 = this.ScheduleBlocks[8];
		scheduleBlock5.destination = "Exit";
		scheduleBlock5.action = "Exit";
		ScheduleBlock scheduleBlock6 = this.ScheduleBlocks[9];
		scheduleBlock6.destination = "Exit";
		scheduleBlock6.action = "Exit";
		this.FollowTarget = null;
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x00204900 File Offset: 0x00202B00
	private void StopFollowingGravureModel()
	{
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[2];
		scheduleBlock.destination = "Seat";
		scheduleBlock.action = "Sit";
		ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[4];
		scheduleBlock2.destination = "LunchSpot";
		scheduleBlock2.action = "Eat";
		ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[6];
		scheduleBlock3.destination = "Locker";
		scheduleBlock3.action = "Shoes";
		ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[7];
		scheduleBlock4.destination = "Exit";
		scheduleBlock4.action = "Stand";
		this.GetDestinations();
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		if (this.StudentID > 1)
		{
			this.IdleAnim = this.BulliedIdleAnim;
			this.WalkAnim = this.BulliedWalkAnim;
		}
		this.TargetDistance = 0.5f;
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x002049E0 File Offset: 0x00202BE0
	public void StopDrinking()
	{
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		this.DrinkingFountain.Occupied = false;
		this.EquipCleaningItems();
		this.EatingSnack = false;
		this.Private = false;
		this.Routine = true;
		this.StudentManager.UpdateMe(this.StudentID);
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x00204A34 File Offset: 0x00202C34
	public void GoToClass()
	{
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase];
		scheduleBlock.destination = "Seat";
		scheduleBlock.action = "Sit";
		this.Actions[this.Phase] = StudentActionType.SitAndTakeNotes;
		this.CurrentAction = StudentActionType.SitAndTakeNotes;
		this.GetDestinations();
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x00204AAC File Offset: 0x00202CAC
	public void RaibaruCannotFindOsana()
	{
		this.SpeechLines.Stop();
		this.CharacterAnimation.CrossFade(this.LookLeftRightAnim);
		this.SnackTimer += Time.deltaTime;
		if (this.SnackTimer > 5f)
		{
			this.Subtitle.UpdateLabel(SubtitleType.RaibaruRivalDeathReaction, 5, 10f);
			this.RaibaruOsanaDeathScheduleChanges();
			this.RaibaruStopsFollowingOsana();
			this.GetDestinations();
			this.CurrentDestination = this.Destinations[this.Phase];
			this.Pathfinding.target = this.Destinations[this.Phase];
			this.SnackTimer = 0f;
		}
		if (this.Actions[this.Phase] == StudentActionType.SitAndEatBento && this.Schoolwear == 2)
		{
			Debug.Log(this.Name + " needs to change clothing before doing whatever they're supposed to do next.");
			this.MustChangeClothing = true;
			this.ChangeClothingPhase = 0;
			this.CurrentDestination = this.StudentManager.StrippingPositions[this.GirlID];
			this.Pathfinding.target = this.StudentManager.StrippingPositions[this.GirlID];
		}
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x00204BC4 File Offset: 0x00202DC4
	public void CannotFindInfatuationTarget()
	{
		Debug.Log("Student #" + this.StudentID.ToString() + " cannot find the student they are supposed to be following.");
		this.CharacterAnimation.CrossFade(this.LookLeftRightAnim);
		this.SnackTimer += Time.deltaTime;
		if (this.SnackTimer > 5f)
		{
			Debug.Log("The student has decided to give up on following the gravure model.");
			this.StopFollowingGravureModel();
			this.SnackTimer = 0f;
		}
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x00204C3C File Offset: 0x00202E3C
	public void DisableProps()
	{
		this.RandomCheerAnim = this.CheerAnims[UnityEngine.Random.Range(0, this.CheerAnims.Length)];
		this.HealthBar.transform.parent.gameObject.SetActive(false);
		this.FollowCountdown.gameObject.SetActive(false);
		this.VomitEmitter.gameObject.SetActive(false);
		this.ChaseCamera.gameObject.SetActive(false);
		this.Countdown.gameObject.SetActive(false);
		this.MiyukiGameScreen.SetActive(false);
		this.Chopsticks[0].SetActive(false);
		this.Chopsticks[1].SetActive(false);
		this.Handkerchief.SetActive(false);
		this.PepperSpray.SetActive(false);
		this.RetroCamera.SetActive(false);
		this.WateringCan.SetActive(false);
		this.Sketchbook.SetActive(false);
		this.OccultBook.SetActive(false);
		this.Paintbrush.SetActive(false);
		this.Cigarette.SetActive(false);
		this.EventBook.SetActive(false);
		this.Handcuffs.SetActive(false);
		this.WeaponBag.SetActive(false);
		this.Scrubber.SetActive(false);
		this.Armband.SetActive(false);
		this.Lighter.SetActive(false);
		this.Octodog.SetActive(false);
		this.Palette.SetActive(false);
		this.Eraser.SetActive(false);
		this.Pencil.SetActive(false);
		this.Bento.SetActive(false);
		this.Pen.SetActive(false);
		this.SpeechLines.Stop();
		if (this.SmartPhone.transform.parent != null)
		{
			this.SmartPhone.SetActive(false);
		}
		foreach (GameObject gameObject in this.ScienceProps)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		foreach (GameObject gameObject2 in this.Fingerfood)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
		}
		foreach (GameObject gameObject3 in this.InstrumentBag)
		{
			if (gameObject3 != null)
			{
				gameObject3.SetActive(false);
			}
		}
		if (this.Male)
		{
			this.DisableMaleProps();
			return;
		}
		this.DisableFemaleProps();
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x00204EA0 File Offset: 0x002030A0
	public void DisableFemaleProps()
	{
		this.GiftBag.SetActive(false);
		this.SkirtOrigins[0] = this.Skirt[0].transform.localPosition;
		this.SkirtOrigins[1] = this.Skirt[1].transform.localPosition;
		this.SkirtOrigins[2] = this.Skirt[2].transform.localPosition;
		this.SkirtOrigins[3] = this.Skirt[3].transform.localPosition;
		this.PickRandomGossipAnim();
		this.DramaticCamera.gameObject.SetActive(false);
		this.MapMarker.gameObject.SetActive(false);
		this.EightiesPhone.SetActive(false);
		this.AnimatedBook.SetActive(false);
		this.Handkerchief.SetActive(false);
		this.Sketchbook.SetActive(false);
		foreach (GameObject gameObject in this.Instruments)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		this.PicnicProps[0].SetActive(false);
		this.PicnicProps[1].SetActive(false);
		this.PicnicProps[2].SetActive(false);
		this.Drumsticks[0].SetActive(false);
		this.Drumsticks[1].SetActive(false);
		if (this.Club >= ClubType.Teacher)
		{
			this.BecomeTeacher();
		}
		if (GameGlobals.CensorPanties && !this.Teacher)
		{
			this.Cosmetic.CensorPanties();
		}
		this.DisableEffects();
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x00205028 File Offset: 0x00203228
	public void DisableMaleProps()
	{
		this.MapMarker.gameObject.SetActive(false);
		this.DelinquentSpeechLines.Stop();
		this.PinkSeifuku.SetActive(false);
		this.Earpiece.SetActive(false);
		this.PickRandomGossipAnim();
		ParticleSystem[] liquidEmitters = this.LiquidEmitters;
		for (int i = 0; i < liquidEmitters.Length; i++)
		{
			liquidEmitters[i].gameObject.SetActive(false);
		}
		this.DisableEffects();
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x00205098 File Offset: 0x00203298
	public void TriggerBeatEmUpMinigame()
	{
		GameGlobals.BeatEmUpDifficulty = 1;
		SceneManager.LoadScene("BeatEmUpScene", LoadSceneMode.Additive);
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			rootGameObjects[i].SetActive(false);
		}
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x002050DC File Offset: 0x002032DC
	public void PlaceBag()
	{
		Debug.Log("Current rival - " + this.Name + " - just fired PlaceBag() and put her bookbag on her desk.");
		if (!this.StudentManager.Eighties)
		{
			this.StudentManager.RivalBookBag.GetComponent<MeshFilter>().mesh = this.Cosmetic.ModernBookBagMesh;
			this.StudentManager.RivalBookBag.GetComponent<Renderer>().material.mainTexture = this.Cosmetic.BookbagTextures[this.StudentID];
		}
		if (this.Seat.position.x < 0f)
		{
			this.StudentManager.RivalBookBag.transform.position = this.Seat.position + new Vector3(-0.33333f, 0.342f, 0.3585f);
			this.StudentManager.RivalBookBag.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		}
		else
		{
			this.StudentManager.RivalBookBag.transform.position = this.Seat.position + new Vector3(0.33333f, 0.342f, -0.3585f);
			this.StudentManager.RivalBookBag.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		}
		this.StudentManager.RivalBookBag.CorrectPosition = this.StudentManager.RivalBookBag.transform.position;
		this.StudentManager.RivalBookBag.CorrectRotation = this.StudentManager.RivalBookBag.transform.eulerAngles;
		this.StudentManager.RivalBookBag.gameObject.SetActive(true);
		this.StudentManager.RivalBookBag.Prompt.enabled = true;
		this.StudentManager.RivalBookBag.Rival = this;
		this.BookBag.SetActive(false);
		if (this.GiftBox)
		{
			this.StudentManager.WednesdayGiftBox.SetActive(true);
			if (base.transform.position.x < 0f)
			{
				this.StudentManager.WednesdayGiftBox.transform.position = this.Seat.position + new Vector3(-0.4f, 1.02f, 0f);
			}
			else
			{
				this.StudentManager.WednesdayGiftBox.transform.position = this.Seat.position + new Vector3(0.4f, 1.02f, 0f);
			}
			this.GiftBox = false;
		}
		this.StudentManager.BagPlaced = true;
		this.BagPlaced = true;
		if (this.VisitSenpaiDesk)
		{
			if (this.CurrentDestination == this.StudentManager.Students[1].Seat)
			{
				this.StudentManager.FridayTestNotes.SetActive(true);
				this.VisitSenpaiDesk = false;
			}
			this.Destinations[this.Phase] = this.StudentManager.Students[1].Seat;
			this.CurrentDestination = this.StudentManager.Students[1].Seat;
			this.Pathfinding.target = this.StudentManager.Students[1].Seat;
			return;
		}
		if (this.Bullied)
		{
			ScheduleBlock scheduleBlock = this.ScheduleBlocks[2];
			scheduleBlock.destination = "ShameSpot";
			scheduleBlock.action = "Shamed";
			scheduleBlock.time = 8f;
		}
		else if (this.StudentManager.CustomMode)
		{
			Debug.Log("Attempting to update rival's routine to whatever her Custom Mode routine is supposed to be.");
			ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[2];
			scheduleBlock2.destination = this.OriginalDestination;
			scheduleBlock2.action = this.OriginalAction;
		}
		else if (this.StudentManager.Eighties)
		{
			if (this.StudentID == 11 || this.StudentID == 12)
			{
				ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[2];
				scheduleBlock3.destination = "Hangout";
				scheduleBlock3.action = "Socialize";
			}
			else if (this.StudentID == 13)
			{
				ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[2];
				scheduleBlock4.destination = "Patrol";
				scheduleBlock4.action = "Patrol";
			}
			else if (this.StudentID == 14)
			{
				ScheduleBlock scheduleBlock5 = this.ScheduleBlocks[2];
				scheduleBlock5.destination = "Sunbathe";
				scheduleBlock5.action = "Jog";
			}
			else if (this.StudentID == 15)
			{
				ScheduleBlock scheduleBlock6 = this.ScheduleBlocks[2];
				scheduleBlock6.destination = "Sunbathe";
				scheduleBlock6.action = "Sunbathe";
			}
			else if (this.StudentID == 16)
			{
				ScheduleBlock scheduleBlock7 = this.ScheduleBlocks[2];
				scheduleBlock7.destination = "Perform";
				scheduleBlock7.action = "Perform";
			}
			else if (this.StudentID == 17)
			{
				Debug.Log("Prodigy rival is building their schedule from here.");
				ScheduleBlock scheduleBlock8 = this.ScheduleBlocks[2];
				scheduleBlock8.destination = "Hangout";
				scheduleBlock8.action = "HelpTeacher";
			}
			else if (this.StudentID == 18)
			{
				ScheduleBlock scheduleBlock9 = this.ScheduleBlocks[2];
				scheduleBlock9.destination = "Patrol";
				scheduleBlock9.action = "Patrol";
			}
			else if (this.StudentID == 19)
			{
				ScheduleBlock scheduleBlock10 = this.ScheduleBlocks[2];
				if (SchoolGlobals.SchoolAtmosphere > 0.8f && this.StudentManager.Photographers > 0)
				{
					scheduleBlock10.destination = "Sunbathe";
					scheduleBlock10.action = "GravurePose";
				}
				else
				{
					scheduleBlock10.destination = "Patrol";
					scheduleBlock10.action = "Patrol";
				}
			}
			else if (this.StudentID == 20)
			{
				ScheduleBlock scheduleBlock11 = this.ScheduleBlocks[2];
				scheduleBlock11.destination = "Guard";
				scheduleBlock11.action = "Guard";
				this.TargetDistance = 1f;
			}
		}
		else if (this.StudentID == 12)
		{
			ScheduleBlock scheduleBlock12 = this.ScheduleBlocks[2];
			scheduleBlock12.destination = "BakeSale";
			scheduleBlock12.action = "BakeSale";
		}
		this.GetDestinations();
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		this.CurrentAction = this.Actions[this.Phase];
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x002056E8 File Offset: 0x002038E8
	public void GetInitialSleuthTarget()
	{
		if (this.StudentID == 37)
		{
			this.SleuthID = 1;
			return;
		}
		if (this.StudentID == 38)
		{
			this.SleuthID = 20;
			return;
		}
		if (this.StudentID == 39)
		{
			this.SleuthID = 30;
			return;
		}
		if (this.StudentID == 40)
		{
			this.SleuthID = 40;
			return;
		}
		if (this.StudentID != 56)
		{
			if (this.StudentID == 57)
			{
				this.SleuthID = 50;
				return;
			}
			if (this.StudentID == 58)
			{
				this.SleuthID = 60;
				return;
			}
			if (this.StudentID == 59)
			{
				this.SleuthID = 70;
				return;
			}
			if (this.StudentID == 60)
			{
				this.SleuthID = 80;
			}
		}
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x00205798 File Offset: 0x00203998
	public void BecomeSleuth()
	{
		if (!this.SleuthInitialized)
		{
			this.SleuthInitialized = true;
			this.GetInitialSleuthTarget();
		}
		if (this.Club != ClubType.Newspaper && this.Club != ClubType.Photography)
		{
			ClubType club = this.Club;
		}
		bool flag = false;
		if (this.StudentManager.Eighties)
		{
			this.CameraFlash = this.RetroCameraFlash;
			this.SmartPhone = this.RetroCamera;
			if ((double)SchoolGlobals.SchoolAtmosphere < 0.8 && this.StudentID == 36)
			{
				Debug.Log("Newspaper Club girl shouldn't become a sleuth, since she needs to perform club leader activities.");
				flag = true;
			}
			if (this.StudentID == 56 || flag)
			{
				Debug.Log("Student #" + this.StudentID.ToString() + " is a Club leader, and shouldn't become a Sleuth...");
				flag = true;
				if (this.StudentManager.MissionMode)
				{
					ScheduleBlock scheduleBlock = this.ScheduleBlocks[4];
					scheduleBlock.destination = "LunchSpot";
					scheduleBlock.action = "Eat";
					this.GetDestinations();
				}
			}
		}
		if (!flag)
		{
			this.Indoors = true;
			this.Spawned = true;
			if (this.ShoeRemoval.Locker == null)
			{
				this.ShoeRemoval.Start();
			}
			this.ShoeRemoval.PutOnShoes();
			if (this.StudentID != 20)
			{
				this.SprintAnim = this.SleuthSprintAnim;
				this.IdleAnim = this.SleuthIdleAnim;
				this.WalkAnim = this.SleuthWalkAnim;
			}
			this.CameraAnims = this.HeroAnims;
			this.SmartPhone.SetActive(true);
			this.Countdown.Speed = 0.075f;
			this.Sleuthing = true;
			if (this.Male)
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.06f, -0.02f, -0.02f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(22.5f, 22.5f, 150f);
			}
			else
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.033333f, -0.015f, -0.015f);
			}
			if (this.StudentManager.Eighties)
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.033333f, -0.066666f, -0.01f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(15f, 15f, 105f);
				this.Phoneless = false;
			}
			else if (this.Sleuthing)
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.033333f, -0.015f, -0.015f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(12.5f, 120f, 180f);
			}
			else
			{
				this.SmartPhone.transform.localPosition = new Vector3(0.025f, 0.02f, 0.04f);
				this.SmartPhone.transform.localEulerAngles = new Vector3(22.5f, -157.5f, 180f);
			}
			if (this.StudentID != 20 && this.StudentID != 36)
			{
				if (this.Club == ClubType.Photography)
				{
					this.SleuthTarget = this.StudentManager.Clubs.List[this.StudentID];
				}
				else
				{
					this.StudentManager.SleuthPhase = 3;
					this.GetSleuthTarget();
				}
				if (!this.Grudge)
				{
					ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[2];
					scheduleBlock2.destination = "Sleuth";
					scheduleBlock2.action = "Sleuth";
					if (!this.StudentManager.MissionMode)
					{
						ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[4];
						scheduleBlock3.destination = "Sleuth";
						scheduleBlock3.action = "Sleuth";
					}
					else
					{
						ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[4];
						scheduleBlock4.destination = "LunchSpot";
						scheduleBlock4.action = "Eat";
					}
					if (this.ScheduleBlocks.Length > 7)
					{
						ScheduleBlock scheduleBlock5 = this.ScheduleBlocks[7];
						scheduleBlock5.destination = "Sleuth";
						scheduleBlock5.action = "Sleuth";
					}
				}
				else
				{
					this.StalkTarget = this.Yandere.transform;
					this.SleuthTarget = this.Yandere.transform;
					ScheduleBlock scheduleBlock6 = this.ScheduleBlocks[2];
					scheduleBlock6.destination = "Stalk";
					scheduleBlock6.action = "Stalk";
					ScheduleBlock scheduleBlock7 = this.ScheduleBlocks[4];
					scheduleBlock7.destination = "Stalk";
					scheduleBlock7.action = "Stalk";
					if (this.ScheduleBlocks.Length > 7)
					{
						ScheduleBlock scheduleBlock8 = this.ScheduleBlocks[7];
						scheduleBlock8.destination = "Stalk";
						scheduleBlock8.action = "Stalk";
					}
				}
			}
			if (this.SleuthID < 1)
			{
				this.SleuthID = 1;
			}
		}
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x00205C18 File Offset: 0x00203E18
	public void CheckForBento()
	{
		if (this.Bento.activeInHierarchy && this.StudentID > 1 && this.Bento.transform.parent != null)
		{
			GenericBentoScript component = this.Bento.GetComponent<GenericBentoScript>();
			component.enabled = true;
			component.Prompt.enabled = true;
			this.Bento.SetActive(true);
			this.Bento.transform.parent = base.transform;
			if (this.Male)
			{
				this.Bento.transform.localPosition = new Vector3(0f, 0.4266666f, -0.075f);
			}
			else
			{
				this.Bento.transform.localPosition = new Vector3(0f, 0.461f, -0.075f);
			}
			this.Bento.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			this.Bento.transform.parent = null;
		}
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x00205D24 File Offset: 0x00203F24
	public void BlendIntoSittingAnim()
	{
		if (this.CharacterAnimation[this.SocialSitAnim].weight != 1f)
		{
			this.CharacterAnimation[this.SocialSitAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.SocialSitAnim].weight, 1f, Time.deltaTime * 10f);
			if ((double)this.CharacterAnimation[this.SocialSitAnim].weight > 0.99)
			{
				this.CharacterAnimation[this.SocialSitAnim].weight = 1f;
			}
		}
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x00205DCC File Offset: 0x00203FCC
	public void BlendOutOfSittingAnim()
	{
		if (this.CharacterAnimation[this.SocialSitAnim].weight != 0f)
		{
			this.CharacterAnimation[this.SocialSitAnim].weight = Mathf.Lerp(this.CharacterAnimation[this.SocialSitAnim].weight, 0f, Time.deltaTime * 10f);
			if ((double)this.CharacterAnimation[this.SocialSitAnim].weight < 0.01)
			{
				this.CharacterAnimation[this.SocialSitAnim].weight = 0f;
			}
		}
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x00205E74 File Offset: 0x00204074
	public void Oversleep()
	{
		if (this.StudentID != 15 && this.ScheduleBlocks.Length == 10)
		{
			Debug.Log("Giving " + this.Name + " the ''Oversleep'' routine.");
			ScheduleBlock scheduleBlock = this.ScheduleBlocks[6];
			scheduleBlock.destination = "SleepSpot";
			scheduleBlock.action = "Sleep";
			scheduleBlock.time = 99999f;
			ScheduleBlock scheduleBlock2 = this.ScheduleBlocks[7];
			scheduleBlock2.destination = "SleepSpot";
			scheduleBlock2.action = "Sleep";
			scheduleBlock2.time = 99999f;
			ScheduleBlock scheduleBlock3 = this.ScheduleBlocks[8];
			scheduleBlock3.destination = "SleepSpot";
			scheduleBlock3.action = "Sleep";
			scheduleBlock3.time = 99999f;
			ScheduleBlock scheduleBlock4 = this.ScheduleBlocks[9];
			scheduleBlock4.destination = "SleepSpot";
			scheduleBlock4.action = "Sleep";
			scheduleBlock4.time = 99999f;
		}
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x00205F58 File Offset: 0x00204158
	public void UpdateGemaAppearance()
	{
		Debug.Log("Gema is now updating his appearance.");
		this.Cosmetic.FacialHairstyle = 0;
		this.Cosmetic.EyewearID = 9;
		this.Cosmetic.Hairstyle = 49;
		this.Cosmetic.Accessory = 0;
		this.Cosmetic.Start();
		this.OriginalIdleAnim = "properIdle_00";
		this.IdleAnim = "properIdle_00";
		this.OriginalOriginalWalkAnim = "properWalk_00";
		this.OriginalWalkAnim = "properWalk_00";
		this.WalkAnim = "properWalk_00";
		this.ClubAnim = "properGaming_00";
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase];
		if (this.Clock.HourTime < 8f || (this.Clock.HourTime > 13f && this.Clock.HourTime < 13.5f) || this.Clock.HourTime > 15.5f)
		{
			scheduleBlock.destination = "Club";
			scheduleBlock.action = "Club";
		}
		else
		{
			scheduleBlock.destination = "Seat";
			scheduleBlock.action = "Sit";
		}
		this.GetDestinations();
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		this.StudentManager.ReactedToGameLeader = true;
		for (int i = 1; i < 6; i++)
		{
			Debug.Log("Attempting to update the schedule for " + this.StudentManager.Students[80 + i].Name);
			if (this.Clock.HourTime < 13f)
			{
				ScheduleBlock scheduleBlock2 = this.StudentManager.Students[80 + i].ScheduleBlocks[4];
				scheduleBlock2.destination = "Shock";
				scheduleBlock2.action = "Shock";
			}
			else if (this.Clock.HourTime > 13f && this.Clock.HourTime < 13.5f)
			{
				ScheduleBlock scheduleBlock3 = this.StudentManager.Students[80 + i].ScheduleBlocks[7];
				scheduleBlock3.destination = "Shock";
				scheduleBlock3.action = "Shock";
			}
			this.StudentManager.Students[80 + i].AdmireAnim = this.StudentManager.Students[80 + i].AdmireAnims[UnityEngine.Random.Range(0, 3)];
			this.StudentManager.Students[80 + i].GetDestinations();
		}
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x002061BC File Offset: 0x002043BC
	public void FinishPyro()
	{
		this.StudentManager.PyroFlames.Stop();
		this.StudentManager.PyroFlameSounds[1].Stop();
		this.StudentManager.PyroFlameSounds[2].Stop();
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase];
		scheduleBlock.destination = "Hangout";
		scheduleBlock.action = "Socialize";
		if (this.StudentManager.Week > 2)
		{
			scheduleBlock.action = "Gaming";
		}
		this.GetDestinations();
		this.Pathfinding.target = this.Destinations[this.Phase];
		this.CurrentDestination = this.Destinations[this.Phase];
		this.PyroPhase = 1;
		this.PyroTimer = 0f;
	}

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x060021E6 RID: 8678 RVA: 0x0020627D File Offset: 0x0020447D
	public int OnlyDefault
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x00206280 File Offset: 0x00204480
	private void CheckForWallBehind()
	{
		Debug.Log("Checking for a wall behind this student.");
		this.RaycastOrigin = this.Hips;
		Vector3 direction = this.RaycastOrigin.TransformDirection(base.transform.worldToLocalMatrix.MultiplyVector(base.transform.forward) * -1f);
		if (Physics.Raycast(this.RaycastOrigin.position, direction, out this.hit, 2f, this.OnlyDefault))
		{
			Debug.Log("Yeah, hit a wall behind.");
			this.TooCloseToWall = true;
		}
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x0020630C File Offset: 0x0020450C
	private void CheckForWallInFront(float Distance)
	{
		this.RaycastOrigin = this.Hips;
		Vector3 direction = this.RaycastOrigin.TransformDirection(base.transform.worldToLocalMatrix.MultiplyVector(base.transform.forward));
		if (Physics.Raycast(this.RaycastOrigin.position, direction, out this.hit, Distance, this.OnlyDefault))
		{
			this.TooCloseToWall = true;
		}
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x00206378 File Offset: 0x00204578
	public void CheckForWallToLeft()
	{
		this.RaycastOriginVector = base.transform.position + Vector3.up * 0.5f;
		Vector3 right = base.transform.right;
		Debug.DrawRay(this.RaycastOriginVector, right, Color.red);
		if (Physics.Raycast(this.RaycastOriginVector, right, out this.hit, 2f, this.OnlyDefault))
		{
			Debug.Log("Yeah, hit a wall to the left.");
			this.TooCloseToWall = true;
		}
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x002063F8 File Offset: 0x002045F8
	public void CheckForWallToRight()
	{
		Debug.Log("Checking for a wall to the right of this character.");
		this.RaycastOriginVector = base.transform.position + Vector3.up * 0.5f;
		Vector3 vector = base.transform.right * -1f;
		Debug.DrawRay(this.RaycastOriginVector, vector, Color.red);
		if (Physics.Raycast(this.RaycastOriginVector, vector, out this.hit, 2.5f, this.OnlyDefault))
		{
			Debug.Log("Yeah, hit a wall to the right.");
			this.TooCloseToWall = true;
		}
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x0020648C File Offset: 0x0020468C
	public void ResumeFollowing()
	{
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[4];
		scheduleBlock.destination = "Follow";
		scheduleBlock.action = "Follow";
		this.GetDestinations();
		this.Pathfinding.target = this.FollowTarget.transform;
		this.CurrentDestination = this.FollowTarget.transform;
		this.ResumeFollowingAfter = false;
		this.CanTalk = true;
		Debug.Log("Raibaru was told to resume ''Follow'' protocol.");
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x002064FC File Offset: 0x002046FC
	public void SetOriginalScheduleBlocks()
	{
		this.OriginalTimes = new float[this.ScheduleBlocks.Length];
		this.OriginalDests = new string[this.ScheduleBlocks.Length];
		this.OriginalActs = new string[this.ScheduleBlocks.Length];
		for (int i = 0; i < this.ScheduleBlocks.Length; i++)
		{
			this.OriginalTimes[i] = this.ScheduleBlocks[i].time;
			this.OriginalDests[i] = this.ScheduleBlocks[i].destination;
			this.OriginalActs[i] = this.ScheduleBlocks[i].action;
		}
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x00206594 File Offset: 0x00204794
	public void SetOriginalActions()
	{
		for (int i = 0; i < this.Actions.Length; i++)
		{
			this.OriginalActions[i] = this.Actions[i];
		}
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x002065C4 File Offset: 0x002047C4
	public void RestoreOriginalScheduleBlocks()
	{
		for (int i = 0; i < this.ScheduleBlocks.Length; i++)
		{
			this.ScheduleBlocks[i].time = this.OriginalTimes[i];
			this.ScheduleBlocks[i].destination = this.OriginalDests[i];
			this.ScheduleBlocks[i].action = this.OriginalActs[i];
		}
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x00206624 File Offset: 0x00204824
	public void RestoreOriginalActions()
	{
		for (int i = 0; i < this.Actions.Length; i++)
		{
			this.Actions[i] = this.OriginalActions[i];
		}
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x00206654 File Offset: 0x00204854
	public void ChangeClothing()
	{
		Debug.Log(this.Name + " is firing ChangeClothing() right now.");
		if (this.ChangeClothingPhase == 0)
		{
			Debug.Log(this.Name + " is changing clothing right now.");
			UnityEngine.Object.Instantiate<GameObject>(this.StudentManager.CommunalLocker.SteamCloud, base.transform.position + Vector3.up * 0.81f, Quaternion.identity).transform.parent = base.transform;
			this.CharacterAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
			this.ChangeClothingPhase++;
			return;
		}
		if (this.ChangeClothingPhase == 1)
		{
			Debug.Log(this.Name + " is stripping right now.");
			this.CharacterAnimation.CrossFade(this.StripAnim);
			this.Pathfinding.canSearch = false;
			this.Pathfinding.canMove = false;
			if (this.CharacterAnimation[this.StripAnim].time >= 1.5f)
			{
				if (this.Schoolwear != 1)
				{
					this.Schoolwear = 1;
					this.ChangeSchoolwear();
					this.WalkAnim = this.OriginalWalkAnim;
				}
				else if (this.BikiniAttacher != null && this.BikiniAttacher.newRenderer != null)
				{
					this.BikiniAttacher.newRenderer.enabled = false;
					this.MyRenderer.enabled = true;
					this.WearingBikini = false;
				}
				if (this.CharacterAnimation[this.StripAnim].time >= this.CharacterAnimation[this.StripAnim].length)
				{
					Debug.Log(this.Name + " just finished changing clothing and is choosing destination now.");
					this.CurrentAction = this.Actions[this.Phase];
					if (this.CurrentAction == StudentActionType.AtLocker)
					{
						this.Pathfinding.target = this.StudentManager.Exit;
						this.CurrentDestination = this.StudentManager.Exit;
					}
					else if (this.CurrentAction == StudentActionType.ChangeShoes)
					{
						this.Pathfinding.target = this.MyLocker;
						this.CurrentDestination = this.MyLocker;
					}
					else if (this.CurrentAction == StudentActionType.SitAndEatBento)
					{
						Debug.Log("Should be going to eat now.");
						this.Pathfinding.target = this.StudentManager.LunchSpots.List[this.StudentID];
						this.CurrentDestination = this.StudentManager.LunchSpots.List[this.StudentID];
					}
					else
					{
						Debug.Log("Should be going to class now.");
						this.Pathfinding.target = this.Seat;
						this.CurrentDestination = this.Seat;
					}
					this.ChangeClothingPhase++;
					this.MustChangeClothing = false;
					this.Confessing = this.ConfessAfterwards;
					if (this.Confessing)
					{
						Debug.Log(this.Name + " will resume confessing now.");
						this.ChooseLocker();
						this.Routine = false;
					}
					this.ConfessAfterwards = false;
					this.DistanceToDestination = 999f;
				}
			}
		}
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x0020695C File Offset: 0x00204B5C
	public void UpdatePyroUrge()
	{
		this.PyroTimer += Time.deltaTime;
		if (this.PyroTimer > this.PyroLimit)
		{
			this.SpeechLines.Stop();
			ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase];
			scheduleBlock.destination = "LightFire";
			scheduleBlock.action = "LightFire";
			this.GetDestinations();
			this.Pathfinding.target = this.Destinations[this.Phase];
			this.CurrentDestination = this.Destinations[this.Phase];
			this.PyroPhase = 1;
			this.PyroTimer = 0f;
		}
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x002069FC File Offset: 0x00204BFC
	public void ChooseLocker()
	{
		Debug.Log(this.Name + " is now choosing what boy's locker to put a note in.");
		if (this.StudentManager.LoveManager.ConfessToSuitor)
		{
			this.StudentManager.SuitorLocker = this.StudentManager.LockerPositions[this.StudentManager.SuitorID];
			this.CurrentDestination = this.StudentManager.SuitorLocker;
			this.Pathfinding.target = this.StudentManager.SuitorLocker;
			return;
		}
		this.CurrentDestination = this.StudentManager.SenpaiLocker;
		this.Pathfinding.target = this.StudentManager.SenpaiLocker;
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x00206AA4 File Offset: 0x00204CA4
	public void DropWeaponInBox()
	{
		Debug.Log("DropWeaponInBox() was just fired.");
		this.StudentManager.BloodLocation.position = Vector3.zero;
		this.BloodPool.parent = null;
		this.BloodPool.position = this.StudentManager.WeaponBoxSpot.parent.position + new Vector3(0f, 1f, 0f);
		this.BloodPool.eulerAngles = new Vector3(0f, 90f, 0f);
		if (this.BloodPool.GetComponent<WeaponScript>() != null && this.BloodPool.GetComponent<WeaponScript>().Type == WeaponType.Scythe)
		{
			this.BloodPool.position = this.StudentManager.WeaponBoxSpot.parent.position + new Vector3(-0.1f, 1.5f, 0.1f);
			this.BloodPool.eulerAngles = new Vector3(-75f, 0f, 90f);
		}
		this.BloodPool.GetComponent<WeaponScript>().Prompt.enabled = true;
		this.BloodPool.GetComponent<WeaponScript>().Returner = null;
		this.BloodPool.GetComponent<WeaponScript>().enabled = true;
		this.BloodPool.GetComponent<WeaponScript>().DoNotRelocate = true;
		this.BloodPool.GetComponent<WeaponScript>().Drop();
		this.Handkerchief.SetActive(false);
		this.StopInvestigating();
		this.BloodPool = null;
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x00206C24 File Offset: 0x00204E24
	public void GoPose()
	{
		Debug.Log(this.Name + " is being instructed to go pose for a photoshoot.");
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.Stripping = false;
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		this.Destinations[this.Phase] = this.StudentManager.Hangouts.List[19];
		this.Pathfinding.target = this.StudentManager.Hangouts.List[19];
		this.CurrentDestination = this.StudentManager.Hangouts.List[19];
		this.StudentManager.CommunalLocker.Student = null;
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x00206CDC File Offset: 0x00204EDC
	public void EnableOutlines()
	{
		this.ID = 0;
		while (this.ID < this.Outlines.Length)
		{
			if (this.Outlines[this.ID] != null)
			{
				this.Outlines[this.ID].enabled = true;
				this.Outlines[this.ID].color = new Color(0f, 1f, 0f);
			}
			this.ID++;
		}
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x00206D60 File Offset: 0x00204F60
	public void ResetEyes()
	{
		this.LeftEye.localPosition = new Vector3(this.LeftEye.localPosition.x, this.LeftEye.localPosition.y, this.LeftEyeOrigin.z - this.EyeShrink * 0.01f);
		this.RightEye.localPosition = new Vector3(this.RightEye.localPosition.x, this.RightEye.localPosition.y, this.RightEyeOrigin.z + this.EyeShrink * 0.01f);
		this.LeftEye.localScale = new Vector3(1f - this.EyeShrink * 0.5f, 1f - this.EyeShrink * 0.5f, this.LeftEye.localScale.z);
		this.RightEye.localScale = new Vector3(1f - this.EyeShrink * 0.5f, 1f - this.EyeShrink * 0.5f, this.RightEye.localScale.z);
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x00206E88 File Offset: 0x00205088
	public void GoSitInInfirmary()
	{
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		ScheduleBlock scheduleBlock = this.ScheduleBlocks[this.Phase];
		scheduleBlock.destination = "InfirmarySeat";
		scheduleBlock.action = "Relax";
		this.GetDestinations();
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		this.Pathfinding.speed = 2f;
		this.RelaxAnim = this.HeadacheSitAnim;
		this.SeekingMedicine = false;
		this.SitInInfirmary = true;
		this.Routine = true;
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x00206F34 File Offset: 0x00205134
	public void StopFollowing()
	{
		this.Subtitle.UpdateLabel(SubtitleType.StudentFarewell, 0, 3f);
		this.Prompt.Label[0].text = "     Talk";
		this.Prompt.Circle[0].fillAmount = 1f;
		this.Hearts.emission.enabled = false;
		this.FollowCountdown.gameObject.SetActive(false);
		this.Yandere.Follower = null;
		this.Yandere.Followers--;
		this.Following = false;
		this.Routine = true;
		this.CurrentDestination = this.Destinations[this.Phase];
		this.Pathfinding.target = this.Destinations[this.Phase];
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.Pathfinding.speed = this.WalkSpeed;
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x0020702C File Offset: 0x0020522C
	public void TurnOutlinesGreen()
	{
		this.ID = 0;
		while (this.ID < this.Outlines.Length)
		{
			if (this.Outlines[this.ID] != null)
			{
				this.Outlines[this.ID].enabled = true;
				this.Outlines[this.ID].color = new Color(0f, 1f, 0f);
			}
			this.ID++;
		}
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x002070B0 File Offset: 0x002052B0
	public void CheckForKnifeInInventory()
	{
		Debug.Log("Now checking for a knife in the player's inventory.");
		if (this.Yandere.Weapon[1] != null && this.Yandere.Weapon[1].Type == WeaponType.Knife && !this.Yandere.Weapon[1].Broken)
		{
			Debug.Log("Weapon #1 is not broken.");
			this.Yandere.WeaponMenu.Selected = 1;
			this.Yandere.WeaponMenu.Equip();
			return;
		}
		if (this.Yandere.Weapon[2] != null && this.Yandere.Weapon[2].Type == WeaponType.Knife && !this.Yandere.Weapon[2].Broken)
		{
			Debug.Log("Weapon #2 is not broken.");
			this.Yandere.WeaponMenu.Selected = 2;
			this.Yandere.WeaponMenu.Equip();
			return;
		}
		if (this.Yandere.Container != null && this.Yandere.Container.TrashCan != null && this.Yandere.Container.TrashCan.ConcealedWeapon != null && this.Yandere.Container.TrashCan.ConcealedWeapon.Type == WeaponType.Knife && !this.Yandere.Container.TrashCan.ConcealedWeapon.Broken)
		{
			Debug.Log("The weapon concealed in the bag is not broken.");
			WeaponScript concealedWeapon = this.Yandere.Container.TrashCan.ConcealedWeapon;
			this.Yandere.Container.TrashCan.RemoveContents();
			concealedWeapon.Equip();
			concealedWeapon.gameObject.SetActive(true);
		}
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x00207268 File Offset: 0x00205468
	public void InvincibleTakedown()
	{
		Debug.Log("Something just fired InvincibleTakedown()");
		if (this.Yandere.Aiming)
		{
			this.Yandere.StopAiming();
		}
		this.Yandere.Mopping = false;
		this.Yandere.EmptyHands();
		this.Subtitle.PreviousSubtitle = SubtitleType.AcceptFood;
		this.Subtitle.UpdateLabel(SubtitleType.ObstacleMurderReaction, 4, 0f);
		this.AttackReaction();
		this.CharacterAnimation["f02_moCounterB_00"].time = 6f;
		this.Yandere.CharacterAnimation["f02_moCounterA_00"].time = 6f;
		this.Yandere.ShoulderCamera.ObstacleCounter = true;
		this.Yandere.ShoulderCamera.Timer = 6f;
		this.Police.Show = false;
		this.Yandere.CameraEffects.MurderWitnessed();
		this.Yandere.Jukebox.GameOver();
		this.CheckForNearbyWalls();
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x00207364 File Offset: 0x00205564
	public void GoPuke()
	{
		Debug.Log(this.Name + " has ingested emetic poison, and should be going to a toilet.");
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		if (this.Male)
		{
			this.WalkAnim = "stomachPainWalk_00";
			this.StudentManager.GetMaleVomitSpot(this);
			this.Pathfinding.target = this.StudentManager.MaleVomitSpot;
			this.CurrentDestination = this.StudentManager.MaleVomitSpot;
		}
		else
		{
			this.WalkAnim = "f02_stomachPainWalk_00";
			this.StudentManager.GetFemaleVomitSpot(this);
			this.Pathfinding.target = this.StudentManager.FemaleVomitSpot;
			this.CurrentDestination = this.StudentManager.FemaleVomitSpot;
		}
		if (!this.StudentManager.Eighties && this.StudentID == 10)
		{
			this.Pathfinding.target = this.StudentManager.AltFemaleVomitSpot;
			this.CurrentDestination = this.StudentManager.AltFemaleVomitSpot;
			this.VomitDoor = this.StudentManager.AltFemaleVomitDoor;
		}
		this.CharacterAnimation.CrossFade(this.WalkAnim);
		this.DistanceToDestination = 100f;
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.Pathfinding.speed = 2f;
		this.MyBento.Tampered = false;
		this.Distracted = true;
		this.Vomiting = true;
		this.Routine = false;
		this.Private = true;
		this.CanTalk = false;
		this.Chopsticks[0].SetActive(false);
		this.Chopsticks[1].SetActive(false);
		this.Bento.SetActive(false);
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x00207504 File Offset: 0x00205704
	public void ScootAway()
	{
		for (int i = 2; i < 6; i++)
		{
			if (i != this.StudentID && this.StudentManager.Students[i] != null && Vector3.Distance(base.transform.position, this.StudentManager.Students[i].transform.position) < 0.5f)
			{
				Vector3 a = this.StudentManager.Students[i].transform.position - base.transform.position;
				this.MyController.Move(a * (Time.deltaTime * -1f / Time.timeScale));
				this.StudentManager.Students[i].MyController.Move(a * (Time.deltaTime / Time.timeScale));
			}
		}
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x002075EC File Offset: 0x002057EC
	public void PlayMusicalInstrument()
	{
		this.CharacterAnimation.CrossFade(this.ClubAnim);
		if (this.StudentID == 52)
		{
			if (!this.Instruments[this.ClubMemberID].activeInHierarchy)
			{
				this.Instruments[this.ClubMemberID].SetActive(true);
				this.Instruments[this.ClubMemberID].transform.parent = this.Spine;
				this.Instruments[this.ClubMemberID].transform.localPosition = new Vector3(0.275f, -0.16f, 0.095f);
				this.Instruments[this.ClubMemberID].transform.localEulerAngles = new Vector3(-22.5f, 30f, 60f);
				return;
			}
		}
		else if (this.StudentID == 53)
		{
			if (!this.Instruments[this.ClubMemberID].activeInHierarchy)
			{
				this.Instruments[this.ClubMemberID].SetActive(true);
				this.Instruments[this.ClubMemberID].transform.parent = this.Spine;
				this.Instruments[this.ClubMemberID].transform.localPosition = new Vector3(0.275f, -0.16f, 0.095f);
				this.Instruments[this.ClubMemberID].transform.localEulerAngles = new Vector3(-22.5f, 30f, 60f);
				return;
			}
		}
		else if (this.StudentID == 54)
		{
			this.Drumsticks[0].SetActive(true);
			this.Drumsticks[1].SetActive(true);
		}
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x00207788 File Offset: 0x00205988
	public void GetHeadache()
	{
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		if (this.Male)
		{
			this.SprintAnim = "headacheWalk_00";
			this.IdleAnim = "headacheIdle_00";
		}
		else
		{
			this.SprintAnim = "f02_headacheWalk_00";
			this.IdleAnim = "f02_headacheIdle_00";
		}
		this.CharacterAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		this.CharacterAnimation.CrossFade(this.SprintAnim);
		this.DistanceToDestination = 100f;
		this.Pathfinding.canSearch = true;
		this.Pathfinding.canMove = true;
		this.Pathfinding.speed = 2f;
		this.MyBento.Tampered = false;
		this.SeekingMedicine = true;
		this.Routine = false;
		this.Private = true;
		this.Chopsticks[0].SetActive(false);
		this.Chopsticks[1].SetActive(false);
		this.Bento.SetActive(false);
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x00207870 File Offset: 0x00205A70
	public void SpawnTimeRespectingAudioSource(AudioClip Clip)
	{
		UnityEngine.Object.Instantiate<GameObject>(this.AudioSourceObject, base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<AudioSourceScript>().MyClip = Clip;
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x002078BC File Offset: 0x00205ABC
	public void CheckForNearbyWalls()
	{
		this.CheckForWallInFront(1f);
		this.CheckForWallBehind();
		this.CheckForWallToRight();
		this.CheckForWallToLeft();
		if (this.TooCloseToWall)
		{
			this.Yandere.transform.position = new Vector3(0f, 100f, 0f);
			base.transform.position = new Vector3(0f, 100f, 0.5f);
			this.StudentManager.BlackCube.SetActive(true);
			this.Yandere.MainCamera.transform.position = new Vector3(0.5f, 101.5f, -1f);
			this.Yandere.MainCamera.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			Physics.SyncTransforms();
		}
	}

	// Token: 0x040042D8 RID: 17112
	public Quaternion targetRotation;

	// Token: 0x040042D9 RID: 17113
	public Quaternion OriginalRotation;

	// Token: 0x040042DA RID: 17114
	public Quaternion OriginalPlateRotation;

	// Token: 0x040042DB RID: 17115
	public SelectiveGrayscale ChaseSelectiveGrayscale;

	// Token: 0x040042DC RID: 17116
	public YanSaveIdentifier BloodSpawnerIdentifier;

	// Token: 0x040042DD RID: 17117
	public DrinkingFountainScript DrinkingFountain;

	// Token: 0x040042DE RID: 17118
	public ModernRivalEventScript EventToSabotage;

	// Token: 0x040042DF RID: 17119
	public DetectionMarkerScript DetectionMarker;

	// Token: 0x040042E0 RID: 17120
	public ChemistScannerScript ChemistScanner;

	// Token: 0x040042E1 RID: 17121
	public StudentManagerScript StudentManager;

	// Token: 0x040042E2 RID: 17122
	public RiggedAccessoryAttacher TaroApron;

	// Token: 0x040042E3 RID: 17123
	public CameraEffectsScript CameraEffects;

	// Token: 0x040042E4 RID: 17124
	public ChangingBoothScript ChangingBooth;

	// Token: 0x040042E5 RID: 17125
	public DialogueWheelScript DialogueWheel;

	// Token: 0x040042E6 RID: 17126
	public WitnessCameraScript WitnessCamera;

	// Token: 0x040042E7 RID: 17127
	public YanSaveIdentifier BentoIdentifier;

	// Token: 0x040042E8 RID: 17128
	public YanSaveIdentifier HipsIdentifier;

	// Token: 0x040042E9 RID: 17129
	public StudentScript DistractionTarget;

	// Token: 0x040042EA RID: 17130
	public CookingEventScript CookingEvent;

	// Token: 0x040042EB RID: 17131
	public EventManagerScript EventManager;

	// Token: 0x040042EC RID: 17132
	public GradingPaperScript GradingPaper;

	// Token: 0x040042ED RID: 17133
	public CountdownScript FollowCountdown;

	// Token: 0x040042EE RID: 17134
	public ClubManagerScript ClubManager;

	// Token: 0x040042EF RID: 17135
	public LightSwitchScript LightSwitch;

	// Token: 0x040042F0 RID: 17136
	public MovingEventScript MovingEvent;

	// Token: 0x040042F1 RID: 17137
	public ShoeRemovalScript ShoeRemoval;

	// Token: 0x040042F2 RID: 17138
	public SnapStudentScript SnapStudent;

	// Token: 0x040042F3 RID: 17139
	public StruggleBarScript StruggleBar;

	// Token: 0x040042F4 RID: 17140
	public ToiletEventScript ToiletEvent;

	// Token: 0x040042F5 RID: 17141
	public WeaponScript WeaponToTakeAway;

	// Token: 0x040042F6 RID: 17142
	public DynamicGridObstacle Obstacle;

	// Token: 0x040042F7 RID: 17143
	public StudentScript StudentToMourn;

	// Token: 0x040042F8 RID: 17144
	public PhoneEventScript PhoneEvent;

	// Token: 0x040042F9 RID: 17145
	public PickpocketScript PickPocket;

	// Token: 0x040042FA RID: 17146
	public ReputationScript Reputation;

	// Token: 0x040042FB RID: 17147
	public SimpleLookScript SimpleLook;

	// Token: 0x040042FC RID: 17148
	public StudentScript TargetStudent;

	// Token: 0x040042FD RID: 17149
	public GenericBentoScript MyBento;

	// Token: 0x040042FE RID: 17150
	public StudentScript FollowTarget;

	// Token: 0x040042FF RID: 17151
	public CountdownScript Countdown;

	// Token: 0x04004300 RID: 17152
	public Renderer SmartPhoneScreen;

	// Token: 0x04004301 RID: 17153
	public YanSaveIdentifier YanSave;

	// Token: 0x04004302 RID: 17154
	public StudentScript Distractor;

	// Token: 0x04004303 RID: 17155
	public StudentScript HuntTarget;

	// Token: 0x04004304 RID: 17156
	public StudentScript MyReporter;

	// Token: 0x04004305 RID: 17157
	public StudentScript MyTeacher;

	// Token: 0x04004306 RID: 17158
	public BoneSetsScript BoneSets;

	// Token: 0x04004307 RID: 17159
	public CosmeticScript Cosmetic;

	// Token: 0x04004308 RID: 17160
	public PickUpScript PuzzleCube;

	// Token: 0x04004309 RID: 17161
	public SaveLoadScript SaveLoad;

	// Token: 0x0400430A RID: 17162
	public SubtitleScript Subtitle;

	// Token: 0x0400430B RID: 17163
	public StudentScript Follower;

	// Token: 0x0400430C RID: 17164
	public DynamicBone OsanaHairL;

	// Token: 0x0400430D RID: 17165
	public DynamicBone OsanaHairR;

	// Token: 0x0400430E RID: 17166
	public ARMiyukiScript Miyuki;

	// Token: 0x0400430F RID: 17167
	public WeaponScript MyWeapon;

	// Token: 0x04004310 RID: 17168
	public StudentScript Partner;

	// Token: 0x04004311 RID: 17169
	public RagdollScript Ragdoll;

	// Token: 0x04004312 RID: 17170
	public YandereScript Yandere;

	// Token: 0x04004313 RID: 17171
	public Camera DramaticCamera;

	// Token: 0x04004314 RID: 17172
	public RagdollScript Corpse;

	// Token: 0x04004315 RID: 17173
	public StudentScript Hunter;

	// Token: 0x04004316 RID: 17174
	public DoorScript VomitDoor;

	// Token: 0x04004317 RID: 17175
	public BrokenScript Broken;

	// Token: 0x04004318 RID: 17176
	public DynamicBone MaiHair;

	// Token: 0x04004319 RID: 17177
	public PoliceScript Police;

	// Token: 0x0400431A RID: 17178
	public PromptScript Prompt;

	// Token: 0x0400431B RID: 17179
	public AIPath Pathfinding;

	// Token: 0x0400431C RID: 17180
	public TalkingScript Talk;

	// Token: 0x0400431D RID: 17181
	public CheerScript Cheer;

	// Token: 0x0400431E RID: 17182
	public ClockScript Clock;

	// Token: 0x0400431F RID: 17183
	public RadioScript Radio;

	// Token: 0x04004320 RID: 17184
	public Renderer Painting;

	// Token: 0x04004321 RID: 17185
	public JsonScript JSON;

	// Token: 0x04004322 RID: 17186
	public NapeScript Nape;

	// Token: 0x04004323 RID: 17187
	public SuckScript Suck;

	// Token: 0x04004324 RID: 17188
	public Renderer Tears;

	// Token: 0x04004325 RID: 17189
	public Rigidbody MyRigidbody;

	// Token: 0x04004326 RID: 17190
	public Collider HorudaCollider;

	// Token: 0x04004327 RID: 17191
	public Collider NapeCollider;

	// Token: 0x04004328 RID: 17192
	public Collider MyCollider;

	// Token: 0x04004329 RID: 17193
	public CharacterController MyController;

	// Token: 0x0400432A RID: 17194
	public Animation CharacterAnimation;

	// Token: 0x0400432B RID: 17195
	public Projector LiquidProjector;

	// Token: 0x0400432C RID: 17196
	public float VisionFOV;

	// Token: 0x0400432D RID: 17197
	public float VisionBonus;

	// Token: 0x0400432E RID: 17198
	public float VisionDistance;

	// Token: 0x0400432F RID: 17199
	public ParticleSystem DelinquentSpeechLines;

	// Token: 0x04004330 RID: 17200
	public ParticleSystem PepperSprayEffect;

	// Token: 0x04004331 RID: 17201
	public ParticleSystem DrowningSplashes;

	// Token: 0x04004332 RID: 17202
	public ParticleSystem BloodFountain;

	// Token: 0x04004333 RID: 17203
	public ParticleSystem VomitEmitter;

	// Token: 0x04004334 RID: 17204
	public ParticleSystem SpeechLines;

	// Token: 0x04004335 RID: 17205
	public ParticleSystem BullyDust;

	// Token: 0x04004336 RID: 17206
	public ParticleSystem ChalkDust;

	// Token: 0x04004337 RID: 17207
	public ParticleSystem Hearts;

	// Token: 0x04004338 RID: 17208
	public Texture[] PhoneTextures;

	// Token: 0x04004339 RID: 17209
	public Texture KokonaPhoneTexture;

	// Token: 0x0400433A RID: 17210
	public Texture MidoriPhoneTexture;

	// Token: 0x0400433B RID: 17211
	public Texture SocialMediaTexture;

	// Token: 0x0400433C RID: 17212
	public Texture OsanaPhoneTexture;

	// Token: 0x0400433D RID: 17213
	public Texture RedBookTexture;

	// Token: 0x0400433E RID: 17214
	public Texture BloodTexture;

	// Token: 0x0400433F RID: 17215
	public Texture BrownTexture;

	// Token: 0x04004340 RID: 17216
	public Texture WaterTexture;

	// Token: 0x04004341 RID: 17217
	public Texture JokichiHead;

	// Token: 0x04004342 RID: 17218
	public Texture GasTexture;

	// Token: 0x04004343 RID: 17219
	public SkinnedMeshRenderer EightiesTeacherRenderer;

	// Token: 0x04004344 RID: 17220
	public SkinnedMeshRenderer MyRenderer;

	// Token: 0x04004345 RID: 17221
	public Renderer BookRenderer;

	// Token: 0x04004346 RID: 17222
	public Transform FollowTargetDestination;

	// Token: 0x04004347 RID: 17223
	public Transform LastSuspiciousObject2;

	// Token: 0x04004348 RID: 17224
	public Transform LastSuspiciousObject;

	// Token: 0x04004349 RID: 17225
	public Transform ExamineCorpseTarget;

	// Token: 0x0400434A RID: 17226
	public Transform CurrentDestination;

	// Token: 0x0400434B RID: 17227
	public Transform LeftMiddleFinger;

	// Token: 0x0400434C RID: 17228
	public Transform TrashDestination;

	// Token: 0x0400434D RID: 17229
	public Transform WeaponBagParent;

	// Token: 0x0400434E RID: 17230
	public Transform LeftItemParent;

	// Token: 0x0400434F RID: 17231
	public Transform PetDestination;

	// Token: 0x04004350 RID: 17232
	public Transform SketchPosition;

	// Token: 0x04004351 RID: 17233
	public Transform CleaningSpot;

	// Token: 0x04004352 RID: 17234
	public Transform SleuthTarget;

	// Token: 0x04004353 RID: 17235
	public Transform WeirdStudent;

	// Token: 0x04004354 RID: 17236
	public Transform Distraction;

	// Token: 0x04004355 RID: 17237
	public Transform StalkTarget;

	// Token: 0x04004356 RID: 17238
	public Transform CorpseHead;

	// Token: 0x04004357 RID: 17239
	public Transform ItemParent;

	// Token: 0x04004358 RID: 17240
	public Transform WitnessPOV;

	// Token: 0x04004359 RID: 17241
	public Transform RightDrill;

	// Token: 0x0400435A RID: 17242
	public Transform BloodPool;

	// Token: 0x0400435B RID: 17243
	public Transform LeftDrill;

	// Token: 0x0400435C RID: 17244
	public Transform LeftPinky;

	// Token: 0x0400435D RID: 17245
	public Transform MapMarker;

	// Token: 0x0400435E RID: 17246
	public Transform RightHand;

	// Token: 0x0400435F RID: 17247
	public Transform LeftHand;

	// Token: 0x04004360 RID: 17248
	public Transform MeetSpot;

	// Token: 0x04004361 RID: 17249
	public Transform MyLocker;

	// Token: 0x04004362 RID: 17250
	public Transform MyPlate;

	// Token: 0x04004363 RID: 17251
	public Transform Spine;

	// Token: 0x04004364 RID: 17252
	public Transform Eyes;

	// Token: 0x04004365 RID: 17253
	public Transform Head;

	// Token: 0x04004366 RID: 17254
	public Transform Hips;

	// Token: 0x04004367 RID: 17255
	public Transform Neck;

	// Token: 0x04004368 RID: 17256
	public Transform Seat;

	// Token: 0x04004369 RID: 17257
	public Transform LipL;

	// Token: 0x0400436A RID: 17258
	public Transform LipR;

	// Token: 0x0400436B RID: 17259
	public Transform Jaw;

	// Token: 0x0400436C RID: 17260
	public ParticleSystem[] LiquidEmitters;

	// Token: 0x0400436D RID: 17261
	public ParticleSystem[] SplashEmitters;

	// Token: 0x0400436E RID: 17262
	public ParticleSystem[] FireEmitters;

	// Token: 0x0400436F RID: 17263
	public ScheduleBlock[] ScheduleBlocks;

	// Token: 0x04004370 RID: 17264
	public ScheduleBlock[] OriginalScheduleBlocks;

	// Token: 0x04004371 RID: 17265
	public Transform[] Destinations;

	// Token: 0x04004372 RID: 17266
	public Transform[] LongHair;

	// Token: 0x04004373 RID: 17267
	public Transform[] Skirt;

	// Token: 0x04004374 RID: 17268
	public Transform[] Arm;

	// Token: 0x04004375 RID: 17269
	public DynamicBone[] BlackHoleEffect;

	// Token: 0x04004376 RID: 17270
	public OutlineScript[] Outlines;

	// Token: 0x04004377 RID: 17271
	public GameObject[] InstrumentBag;

	// Token: 0x04004378 RID: 17272
	public GameObject[] ScienceProps;

	// Token: 0x04004379 RID: 17273
	public GameObject[] Instruments;

	// Token: 0x0400437A RID: 17274
	public GameObject[] PicnicProps;

	// Token: 0x0400437B RID: 17275
	public GameObject[] Chopsticks;

	// Token: 0x0400437C RID: 17276
	public GameObject[] Drumsticks;

	// Token: 0x0400437D RID: 17277
	public GameObject[] Fingerfood;

	// Token: 0x0400437E RID: 17278
	public GameObject[] Bones;

	// Token: 0x0400437F RID: 17279
	public string[] DelinquentAnims;

	// Token: 0x04004380 RID: 17280
	public string[] AnimationNames;

	// Token: 0x04004381 RID: 17281
	public string[] GravureAnims;

	// Token: 0x04004382 RID: 17282
	public string[] GossipAnims;

	// Token: 0x04004383 RID: 17283
	public string[] SleuthAnims;

	// Token: 0x04004384 RID: 17284
	public string[] CheerAnims;

	// Token: 0x04004385 RID: 17285
	[SerializeField]
	private List<int> VisibleCorpses = new List<int>();

	// Token: 0x04004386 RID: 17286
	[SerializeField]
	private int[] CorpseLayers = new int[]
	{
		11,
		14
	};

	// Token: 0x04004387 RID: 17287
	[SerializeField]
	private LayerMask YandereCheckMask;

	// Token: 0x04004388 RID: 17288
	[SerializeField]
	private LayerMask Mask;

	// Token: 0x04004389 RID: 17289
	public StudentActionType CurrentAction;

	// Token: 0x0400438A RID: 17290
	public StudentActionType[] Actions;

	// Token: 0x0400438B RID: 17291
	public StudentActionType[] OriginalActions;

	// Token: 0x0400438C RID: 17292
	public AudioClip MurderSuicideKiller;

	// Token: 0x0400438D RID: 17293
	public AudioClip MurderSuicideVictim;

	// Token: 0x0400438E RID: 17294
	public AudioClip MurderSuicideSounds;

	// Token: 0x0400438F RID: 17295
	public AudioClip PoisonDeathClip;

	// Token: 0x04004390 RID: 17296
	public AudioClip PepperSpraySFX;

	// Token: 0x04004391 RID: 17297
	public AudioClip BurningClip;

	// Token: 0x04004392 RID: 17298
	public AudioSource AirGuitar;

	// Token: 0x04004393 RID: 17299
	public AudioClip[] FemaleAttacks;

	// Token: 0x04004394 RID: 17300
	public AudioClip[] BullyGiggles;

	// Token: 0x04004395 RID: 17301
	public AudioClip[] BullyLaughs;

	// Token: 0x04004396 RID: 17302
	public AudioClip[] MaleAttacks;

	// Token: 0x04004397 RID: 17303
	public SphereCollider HipCollider;

	// Token: 0x04004398 RID: 17304
	public Collider RightHandCollider;

	// Token: 0x04004399 RID: 17305
	public Collider LeftHandCollider;

	// Token: 0x0400439A RID: 17306
	public Collider NotFaceCollider;

	// Token: 0x0400439B RID: 17307
	public Collider PantyCollider;

	// Token: 0x0400439C RID: 17308
	public Collider SkirtCollider;

	// Token: 0x0400439D RID: 17309
	public Collider FaceCollider;

	// Token: 0x0400439E RID: 17310
	public Collider PoolStairs;

	// Token: 0x0400439F RID: 17311
	public Collider NEStairs;

	// Token: 0x040043A0 RID: 17312
	public Collider NWStairs;

	// Token: 0x040043A1 RID: 17313
	public Collider SEStairs;

	// Token: 0x040043A2 RID: 17314
	public Collider SWStairs;

	// Token: 0x040043A3 RID: 17315
	public GameObject EightiesTeacherAttacher;

	// Token: 0x040043A4 RID: 17316
	public GameObject EnterGuardStateCollider;

	// Token: 0x040043A5 RID: 17317
	public GameObject HeadacheMedicinePrompt;

	// Token: 0x040043A6 RID: 17318
	public GameObject BloodSprayCollider;

	// Token: 0x040043A7 RID: 17319
	public GameObject BullyPhotoCollider;

	// Token: 0x040043A8 RID: 17320
	public GameObject SquishyBloodEffect;

	// Token: 0x040043A9 RID: 17321
	public GameObject AudioSourceObject;

	// Token: 0x040043AA RID: 17322
	public GameObject WhiteQuestionMark;

	// Token: 0x040043AB RID: 17323
	public GameObject MiyukiGameScreen;

	// Token: 0x040043AC RID: 17324
	public GameObject RetroCameraFlash;

	// Token: 0x040043AD RID: 17325
	public GameObject EmptyGameObject;

	// Token: 0x040043AE RID: 17326
	public GameObject StabBloodEffect;

	// Token: 0x040043AF RID: 17327
	public GameObject BountyCollider;

	// Token: 0x040043B0 RID: 17328
	public GameObject BigWaterSplash;

	// Token: 0x040043B1 RID: 17329
	public GameObject SecurityCamera;

	// Token: 0x040043B2 RID: 17330
	public GameObject RightEmptyEye;

	// Token: 0x040043B3 RID: 17331
	public GameObject LeftEmptyEye;

	// Token: 0x040043B4 RID: 17332
	public GameObject AnimatedBook;

	// Token: 0x040043B5 RID: 17333
	public GameObject BloodyScream;

	// Token: 0x040043B6 RID: 17334
	public GameObject EdgyAttacher;

	// Token: 0x040043B7 RID: 17335
	public GameObject Handkerchief;

	// Token: 0x040043B8 RID: 17336
	public GameObject BloodEffect;

	// Token: 0x040043B9 RID: 17337
	public GameObject CameraFlash;

	// Token: 0x040043BA RID: 17338
	public GameObject ChaseCamera;

	// Token: 0x040043BB RID: 17339
	public GameObject DeathScream;

	// Token: 0x040043BC RID: 17340
	public GameObject PepperSpray;

	// Token: 0x040043BD RID: 17341
	public GameObject PinkSeifuku;

	// Token: 0x040043BE RID: 17342
	public GameObject RetroCamera;

	// Token: 0x040043BF RID: 17343
	public GameObject WateringCan;

	// Token: 0x040043C0 RID: 17344
	public GameObject BagOfChips;

	// Token: 0x040043C1 RID: 17345
	public GameObject BloodSpray;

	// Token: 0x040043C2 RID: 17346
	public GameObject CrushedCan;

	// Token: 0x040043C3 RID: 17347
	public GameObject GarbageBag;

	// Token: 0x040043C4 RID: 17348
	public GameObject Sketchbook;

	// Token: 0x040043C5 RID: 17349
	public GameObject SmartPhone;

	// Token: 0x040043C6 RID: 17350
	public GameObject OccultBook;

	// Token: 0x040043C7 RID: 17351
	public GameObject Paintbrush;

	// Token: 0x040043C8 RID: 17352
	public GameObject AlarmDisc;

	// Token: 0x040043C9 RID: 17353
	public GameObject Character;

	// Token: 0x040043CA RID: 17354
	public GameObject Cigarette;

	// Token: 0x040043CB RID: 17355
	public GameObject EventBook;

	// Token: 0x040043CC RID: 17356
	public GameObject Handcuffs;

	// Token: 0x040043CD RID: 17357
	public GameObject HealthBar;

	// Token: 0x040043CE RID: 17358
	public GameObject OsanaHair;

	// Token: 0x040043CF RID: 17359
	public GameObject PaperFire;

	// Token: 0x040043D0 RID: 17360
	public GameObject WeaponBag;

	// Token: 0x040043D1 RID: 17361
	public GameObject CandyBar;

	// Token: 0x040043D2 RID: 17362
	public GameObject Earpiece;

	// Token: 0x040043D3 RID: 17363
	public GameObject Scrubber;

	// Token: 0x040043D4 RID: 17364
	public GameObject Armband;

	// Token: 0x040043D5 RID: 17365
	public GameObject BookBag;

	// Token: 0x040043D6 RID: 17366
	public GameObject Drawing;

	// Token: 0x040043D7 RID: 17367
	public GameObject GiftBag;

	// Token: 0x040043D8 RID: 17368
	public GameObject Lighter;

	// Token: 0x040043D9 RID: 17369
	public GameObject MyPaper;

	// Token: 0x040043DA RID: 17370
	public GameObject Octodog;

	// Token: 0x040043DB RID: 17371
	public GameObject Palette;

	// Token: 0x040043DC RID: 17372
	public GameObject SodaCan;

	// Token: 0x040043DD RID: 17373
	public GameObject Eraser;

	// Token: 0x040043DE RID: 17374
	public GameObject Giggle;

	// Token: 0x040043DF RID: 17375
	public GameObject Marker;

	// Token: 0x040043E0 RID: 17376
	public GameObject Pencil;

	// Token: 0x040043E1 RID: 17377
	public GameObject Weapon;

	// Token: 0x040043E2 RID: 17378
	public GameObject Bento;

	// Token: 0x040043E3 RID: 17379
	public GameObject Paper;

	// Token: 0x040043E4 RID: 17380
	public GameObject Note;

	// Token: 0x040043E5 RID: 17381
	public GameObject Pen;

	// Token: 0x040043E6 RID: 17382
	public GameObject Lid;

	// Token: 0x040043E7 RID: 17383
	public bool InvestigatingMysteriousDisappearance;

	// Token: 0x040043E8 RID: 17384
	public bool InvestigatingPossibleBlood;

	// Token: 0x040043E9 RID: 17385
	public bool InvestigatingPossibleDeath;

	// Token: 0x040043EA RID: 17386
	public bool InvestigatingPossibleLimb;

	// Token: 0x040043EB RID: 17387
	public bool SpecialRivalDeathReaction;

	// Token: 0x040043EC RID: 17388
	public bool WitnessedMindBrokenMurder;

	// Token: 0x040043ED RID: 17389
	public bool ReturningMisplacedWeapon;

	// Token: 0x040043EE RID: 17390
	public bool SenpaiWitnessingRivalDie;

	// Token: 0x040043EF RID: 17391
	public bool ReportingMurderToSenpai;

	// Token: 0x040043F0 RID: 17392
	public bool IgnoringThingsOnGround;

	// Token: 0x040043F1 RID: 17393
	public bool PlayerHeldBloodyWeapon;

	// Token: 0x040043F2 RID: 17394
	public bool TargetedForDistraction;

	// Token: 0x040043F3 RID: 17395
	public bool SchoolwearUnavailable;

	// Token: 0x040043F4 RID: 17396
	public bool TakingUpAHeadacheSpot;

	// Token: 0x040043F5 RID: 17397
	public bool WitnessedBloodyWeapon;

	// Token: 0x040043F6 RID: 17398
	public bool IgnoringPettyActions;

	// Token: 0x040043F7 RID: 17399
	public bool ManuallyAdvancePhase;

	// Token: 0x040043F8 RID: 17400
	public bool ReturnToRoutineAfter;

	// Token: 0x040043F9 RID: 17401
	public bool TakingUpASedatedSpot;

	// Token: 0x040043FA RID: 17402
	public bool ActivateIncinerator;

	// Token: 0x040043FB RID: 17403
	public bool MustChangeClothing;

	// Token: 0x040043FC RID: 17404
	public bool SawCorpseThisFrame;

	// Token: 0x040043FD RID: 17405
	public bool WillRemoveTripwire;

	// Token: 0x040043FE RID: 17406
	public bool WitnessedBloodPool;

	// Token: 0x040043FF RID: 17407
	public bool WitnessedSomething;

	// Token: 0x04004400 RID: 17408
	public bool ConfessAfterwards;

	// Token: 0x04004401 RID: 17409
	public bool FoundFriendCorpse;

	// Token: 0x04004402 RID: 17410
	public bool LovestruckWaiting;

	// Token: 0x04004403 RID: 17411
	public bool MurderedByFragile;

	// Token: 0x04004404 RID: 17412
	public bool MurderedByStudent;

	// Token: 0x04004405 RID: 17413
	public bool OriginallyTeacher;

	// Token: 0x04004406 RID: 17414
	public bool ReturningFromSave;

	// Token: 0x04004407 RID: 17415
	public bool GoingToLockerRoom;

	// Token: 0x04004408 RID: 17416
	public bool WillRemoveBucket;

	// Token: 0x04004409 RID: 17417
	public bool DramaticReaction;

	// Token: 0x0400440A RID: 17418
	public bool EventInterrupted;

	// Token: 0x0400440B RID: 17419
	public bool EventSpecialCase;

	// Token: 0x0400440C RID: 17420
	public bool FoundEnemyCorpse;

	// Token: 0x0400440D RID: 17421
	public bool ImmuneToLaughter;

	// Token: 0x0400440E RID: 17422
	public bool LostTeacherTrust;

	// Token: 0x0400440F RID: 17423
	public bool TemporarilyBlind;

	// Token: 0x04004410 RID: 17424
	public bool WitnessedCoverUp;

	// Token: 0x04004411 RID: 17425
	public bool WitnessedCorpse;

	// Token: 0x04004412 RID: 17426
	public bool WitnessedMurder;

	// Token: 0x04004413 RID: 17427
	public bool WitnessedWeapon;

	// Token: 0x04004414 RID: 17428
	public bool CrushedByBucket;

	// Token: 0x04004415 RID: 17429
	public bool VerballyReacted;

	// Token: 0x04004416 RID: 17430
	public bool VisitSenpaiDesk;

	// Token: 0x04004417 RID: 17431
	public bool YandereInnocent;

	// Token: 0x04004418 RID: 17432
	public bool GetNewAnimation = true;

	// Token: 0x04004419 RID: 17433
	public bool AttackWillFail;

	// Token: 0x0400441A RID: 17434
	public bool CanStillNotice;

	// Token: 0x0400441B RID: 17435
	public bool FocusOnStudent;

	// Token: 0x0400441C RID: 17436
	public bool FocusOnYandere;

	// Token: 0x0400441D RID: 17437
	public bool ManualRotation;

	// Token: 0x0400441E RID: 17438
	public bool NotActuallyWet;

	// Token: 0x0400441F RID: 17439
	public bool PinDownWitness;

	// Token: 0x04004420 RID: 17440
	public bool RepeatReaction;

	// Token: 0x04004421 RID: 17441
	public bool RivalBodyguard;

	// Token: 0x04004422 RID: 17442
	public bool SitInInfirmary;

	// Token: 0x04004423 RID: 17443
	public bool StalkerFleeing;

	// Token: 0x04004424 RID: 17444
	public bool YandereVisible;

	// Token: 0x04004425 RID: 17445
	public bool WitnessedSlave;

	// Token: 0x04004426 RID: 17446
	public bool AwareOfCorpse;

	// Token: 0x04004427 RID: 17447
	public bool AwareOfMurder;

	// Token: 0x04004428 RID: 17448
	public bool CrimeReported;

	// Token: 0x04004429 RID: 17449
	public bool ExplainedKick;

	// Token: 0x0400442A RID: 17450
	public bool FleeWhenClean;

	// Token: 0x0400442B RID: 17451
	public bool MurderSuicide;

	// Token: 0x0400442C RID: 17452
	public bool PhotoEvidence;

	// Token: 0x0400442D RID: 17453
	public bool RespectEarned;

	// Token: 0x0400442E RID: 17454
	public bool SpottedYakuza;

	// Token: 0x0400442F RID: 17455
	public bool WitnessedLimb;

	// Token: 0x04004430 RID: 17456
	public bool BeenSplashed;

	// Token: 0x04004431 RID: 17457
	public bool BoobsResized;

	// Token: 0x04004432 RID: 17458
	public bool CanTakeSnack;

	// Token: 0x04004433 RID: 17459
	public bool CheckingNote;

	// Token: 0x04004434 RID: 17460
	public bool ClubActivity;

	// Token: 0x04004435 RID: 17461
	public bool Complimented;

	// Token: 0x04004436 RID: 17462
	public bool Electrocuted;

	// Token: 0x04004437 RID: 17463
	public bool FragileSlave;

	// Token: 0x04004438 RID: 17464
	public bool HoldingHands;

	// Token: 0x04004439 RID: 17465
	public bool PlayingAudio;

	// Token: 0x0400443A RID: 17466
	public bool StopRotating;

	// Token: 0x0400443B RID: 17467
	public bool SawFriendDie;

	// Token: 0x0400443C RID: 17468
	public bool SentToLocker;

	// Token: 0x0400443D RID: 17469
	public bool TurnOffRadio;

	// Token: 0x0400443E RID: 17470
	public bool BusyAtLunch;

	// Token: 0x0400443F RID: 17471
	public bool CanGiveHelp;

	// Token: 0x04004440 RID: 17472
	public bool Electrified;

	// Token: 0x04004441 RID: 17473
	public bool HeardScream;

	// Token: 0x04004442 RID: 17474
	public bool HelpOffered;

	// Token: 0x04004443 RID: 17475
	public bool IgnoreBlood;

	// Token: 0x04004444 RID: 17476
	public bool MusumeRight;

	// Token: 0x04004445 RID: 17477
	public bool NeckSnapped;

	// Token: 0x04004446 RID: 17478
	public bool StopSliding;

	// Token: 0x04004447 RID: 17479
	public bool Traumatized;

	// Token: 0x04004448 RID: 17480
	public bool UpdateSkirt;

	// Token: 0x04004449 RID: 17481
	public bool WillCombust;

	// Token: 0x0400444A RID: 17482
	public bool AlreadyFed;

	// Token: 0x0400444B RID: 17483
	public bool ClubAttire;

	// Token: 0x0400444C RID: 17484
	public bool ClubLeader;

	// Token: 0x0400444D RID: 17485
	public bool Confessing;

	// Token: 0x0400444E RID: 17486
	public bool Distracted;

	// Token: 0x0400444F RID: 17487
	public bool DoNotMourn;

	// Token: 0x04004450 RID: 17488
	public bool DoNotShove;

	// Token: 0x04004451 RID: 17489
	public bool ExtraBento;

	// Token: 0x04004452 RID: 17490
	public bool KilledMood;

	// Token: 0x04004453 RID: 17491
	public bool InDarkness;

	// Token: 0x04004454 RID: 17492
	public bool Infatuated;

	// Token: 0x04004455 RID: 17493
	public bool LewdPhotos;

	// Token: 0x04004456 RID: 17494
	public bool SwitchBack;

	// Token: 0x04004457 RID: 17495
	public bool Threatened;

	// Token: 0x04004458 RID: 17496
	public bool BatheFast;

	// Token: 0x04004459 RID: 17497
	public bool Counselor;

	// Token: 0x0400445A RID: 17498
	public bool Depressed;

	// Token: 0x0400445B RID: 17499
	public bool DoNotFeed;

	// Token: 0x0400445C RID: 17500
	public bool DiscCheck;

	// Token: 0x0400445D RID: 17501
	public bool DressCode;

	// Token: 0x0400445E RID: 17502
	public bool Drownable;

	// Token: 0x0400445F RID: 17503
	public bool DyedBrown;

	// Token: 0x04004460 RID: 17504
	public bool EndSearch;

	// Token: 0x04004461 RID: 17505
	public bool NewFriend;

	// Token: 0x04004462 RID: 17506
	public bool GasWarned;

	// Token: 0x04004463 RID: 17507
	public bool KeyStolen;

	// Token: 0x04004464 RID: 17508
	public bool KnifeDown;

	// Token: 0x04004465 RID: 17509
	public bool LongSkirt;

	// Token: 0x04004466 RID: 17510
	public bool Mentoring;

	// Token: 0x04004467 RID: 17511
	public bool NoBreakUp;

	// Token: 0x04004468 RID: 17512
	public bool NoRagdoll;

	// Token: 0x04004469 RID: 17513
	public bool Phoneless;

	// Token: 0x0400446A RID: 17514
	public bool RingReact;

	// Token: 0x0400446B RID: 17515
	public bool TrueAlone;

	// Token: 0x0400446C RID: 17516
	public bool WillChase;

	// Token: 0x0400446D RID: 17517
	public bool Attacked;

	// Token: 0x0400446E RID: 17518
	public bool BakeSale;

	// Token: 0x0400446F RID: 17519
	public bool CanBeFed;

	// Token: 0x04004470 RID: 17520
	public bool Headache;

	// Token: 0x04004471 RID: 17521
	public bool Gossiped;

	// Token: 0x04004472 RID: 17522
	public bool MustTrip;

	// Token: 0x04004473 RID: 17523
	public bool Pushable;

	// Token: 0x04004474 RID: 17524
	public bool PyroUrge;

	// Token: 0x04004475 RID: 17525
	public bool Reflexes;

	// Token: 0x04004476 RID: 17526
	public bool Replaced;

	// Token: 0x04004477 RID: 17527
	public bool Restless;

	// Token: 0x04004478 RID: 17528
	public bool SentHome;

	// Token: 0x04004479 RID: 17529
	public bool Splashed;

	// Token: 0x0400447A RID: 17530
	public bool Tranquil;

	// Token: 0x0400447B RID: 17531
	public bool WalkBack;

	// Token: 0x0400447C RID: 17532
	public bool WaterLow;

	// Token: 0x0400447D RID: 17533
	public bool Alarmed;

	// Token: 0x0400447E RID: 17534
	public bool BadTime;

	// Token: 0x0400447F RID: 17535
	public bool Bullied;

	// Token: 0x04004480 RID: 17536
	public bool Drowned;

	// Token: 0x04004481 RID: 17537
	public bool Forgave;

	// Token: 0x04004482 RID: 17538
	public bool GiftBox;

	// Token: 0x04004483 RID: 17539
	public bool Indoors;

	// Token: 0x04004484 RID: 17540
	public bool InEvent;

	// Token: 0x04004485 RID: 17541
	public bool Injured;

	// Token: 0x04004486 RID: 17542
	public bool Nemesis;

	// Token: 0x04004487 RID: 17543
	public bool Private;

	// Token: 0x04004488 RID: 17544
	public bool Reacted;

	// Token: 0x04004489 RID: 17545
	public bool Removed;

	// Token: 0x0400448A RID: 17546
	public bool SawMask;

	// Token: 0x0400448B RID: 17547
	public bool Sedated;

	// Token: 0x0400448C RID: 17548
	public bool SlideIn;

	// Token: 0x0400448D RID: 17549
	public bool Spawned;

	// Token: 0x0400448E RID: 17550
	public bool Stabbed;

	// Token: 0x0400448F RID: 17551
	public bool Stalker;

	// Token: 0x04004490 RID: 17552
	public bool Started;

	// Token: 0x04004491 RID: 17553
	public bool Suicide;

	// Token: 0x04004492 RID: 17554
	public bool Teacher;

	// Token: 0x04004493 RID: 17555
	public bool Tripped;

	// Token: 0x04004494 RID: 17556
	public bool Witness;

	// Token: 0x04004495 RID: 17557
	public bool Bloody;

	// Token: 0x04004496 RID: 17558
	public bool CanTalk = true;

	// Token: 0x04004497 RID: 17559
	public bool Emetic;

	// Token: 0x04004498 RID: 17560
	public bool Lethal;

	// Token: 0x04004499 RID: 17561
	public bool Routine = true;

	// Token: 0x0400449A RID: 17562
	public bool Friend;

	// Token: 0x0400449B RID: 17563
	public bool GoAway;

	// Token: 0x0400449C RID: 17564
	public bool Grudge;

	// Token: 0x0400449D RID: 17565
	public bool Hungry;

	// Token: 0x0400449E RID: 17566
	public bool Hunted;

	// Token: 0x0400449F RID: 17567
	public bool NoTalk;

	// Token: 0x040044A0 RID: 17568
	public bool Paired;

	// Token: 0x040044A1 RID: 17569
	public bool Pushed;

	// Token: 0x040044A2 RID: 17570
	public bool Shovey;

	// Token: 0x040044A3 RID: 17571
	public bool Sleepy;

	// Token: 0x040044A4 RID: 17572
	public bool Urgent;

	// Token: 0x040044A5 RID: 17573
	public bool Warned;

	// Token: 0x040044A6 RID: 17574
	public bool Alone;

	// Token: 0x040044A7 RID: 17575
	public bool Blind;

	// Token: 0x040044A8 RID: 17576
	public bool Eaten;

	// Token: 0x040044A9 RID: 17577
	public bool Hurry;

	// Token: 0x040044AA RID: 17578
	public bool Rival;

	// Token: 0x040044AB RID: 17579
	public bool Slave;

	// Token: 0x040044AC RID: 17580
	public bool Calm;

	// Token: 0x040044AD RID: 17581
	public bool Halt;

	// Token: 0x040044AE RID: 17582
	public bool Lost;

	// Token: 0x040044AF RID: 17583
	public bool Male;

	// Token: 0x040044B0 RID: 17584
	public bool Rose;

	// Token: 0x040044B1 RID: 17585
	public bool Safe;

	// Token: 0x040044B2 RID: 17586
	public bool Stop;

	// Token: 0x040044B3 RID: 17587
	public bool AoT;

	// Token: 0x040044B4 RID: 17588
	public bool Fed;

	// Token: 0x040044B5 RID: 17589
	public bool Gas;

	// Token: 0x040044B6 RID: 17590
	public bool Shy;

	// Token: 0x040044B7 RID: 17591
	public bool Wet;

	// Token: 0x040044B8 RID: 17592
	public bool Won;

	// Token: 0x040044B9 RID: 17593
	public bool DK;

	// Token: 0x040044BA RID: 17594
	public bool MorningRivalWitness;

	// Token: 0x040044BB RID: 17595
	public bool LunchRivalWitness;

	// Token: 0x040044BC RID: 17596
	public bool AfterRivalWitness;

	// Token: 0x040044BD RID: 17597
	public bool NotAlarmedByYandereChan;

	// Token: 0x040044BE RID: 17598
	public bool InvestigatingBloodPool;

	// Token: 0x040044BF RID: 17599
	public bool ResumeTakingOutTrash;

	// Token: 0x040044C0 RID: 17600
	public bool RetreivingMedicine;

	// Token: 0x040044C1 RID: 17601
	public bool ListeningToReport;

	// Token: 0x040044C2 RID: 17602
	public bool MissionModeTarget;

	// Token: 0x040044C3 RID: 17603
	public bool ResumeDistracting;

	// Token: 0x040044C4 RID: 17604
	public bool UpdateAppearance;

	// Token: 0x040044C5 RID: 17605
	public bool BreakingUpFight;

	// Token: 0x040044C6 RID: 17606
	public bool SeekingMedicine;

	// Token: 0x040044C7 RID: 17607
	public bool ReportingMurder;

	// Token: 0x040044C8 RID: 17608
	public bool CameraReacting;

	// Token: 0x040044C9 RID: 17609
	public bool UsingRigidbody;

	// Token: 0x040044CA RID: 17610
	public bool ReportingBlood;

	// Token: 0x040044CB RID: 17611
	public bool TakingOutTrash;

	// Token: 0x040044CC RID: 17612
	public bool FightingSlave;

	// Token: 0x040044CD RID: 17613
	public bool Investigating;

	// Token: 0x040044CE RID: 17614
	public bool SolvingPuzzle;

	// Token: 0x040044CF RID: 17615
	public bool ChangingShoes;

	// Token: 0x040044D0 RID: 17616
	public bool Distracting;

	// Token: 0x040044D1 RID: 17617
	public bool EatingSnack;

	// Token: 0x040044D2 RID: 17618
	public bool HitReacting;

	// Token: 0x040044D3 RID: 17619
	public bool PinningDown;

	// Token: 0x040044D4 RID: 17620
	public bool WasHurrying;

	// Token: 0x040044D5 RID: 17621
	public bool Struggling;

	// Token: 0x040044D6 RID: 17622
	public bool Following;

	// Token: 0x040044D7 RID: 17623
	public bool NotEating;

	// Token: 0x040044D8 RID: 17624
	public bool Sleuthing;

	// Token: 0x040044D9 RID: 17625
	public bool Stripping;

	// Token: 0x040044DA RID: 17626
	public bool Fighting;

	// Token: 0x040044DB RID: 17627
	public bool Guarding;

	// Token: 0x040044DC RID: 17628
	public bool Ignoring;

	// Token: 0x040044DD RID: 17629
	public bool Pursuing;

	// Token: 0x040044DE RID: 17630
	public bool Spraying;

	// Token: 0x040044DF RID: 17631
	public bool Tripping;

	// Token: 0x040044E0 RID: 17632
	public bool Vomiting;

	// Token: 0x040044E1 RID: 17633
	public bool Burning;

	// Token: 0x040044E2 RID: 17634
	public bool Chasing;

	// Token: 0x040044E3 RID: 17635
	public bool Curious;

	// Token: 0x040044E4 RID: 17636
	public bool Fleeing;

	// Token: 0x040044E5 RID: 17637
	public bool Hunting;

	// Token: 0x040044E6 RID: 17638
	public bool Leaving;

	// Token: 0x040044E7 RID: 17639
	public bool Meeting;

	// Token: 0x040044E8 RID: 17640
	public bool Shoving;

	// Token: 0x040044E9 RID: 17641
	public bool Talking;

	// Token: 0x040044EA RID: 17642
	public bool Waiting;

	// Token: 0x040044EB RID: 17643
	public bool Dodging;

	// Token: 0x040044EC RID: 17644
	public bool Posing;

	// Token: 0x040044ED RID: 17645
	public bool Dying;

	// Token: 0x040044EE RID: 17646
	public float DistanceToDestination;

	// Token: 0x040044EF RID: 17647
	public float InvestigationDistance;

	// Token: 0x040044F0 RID: 17648
	public float FollowTargetDistance;

	// Token: 0x040044F1 RID: 17649
	public float DistanceToPlayer;

	// Token: 0x040044F2 RID: 17650
	public float TargetDistance;

	// Token: 0x040044F3 RID: 17651
	public float ThreatDistance;

	// Token: 0x040044F4 RID: 17652
	public float LockerRoomCheckTimer;

	// Token: 0x040044F5 RID: 17653
	public float WitnessCooldownTimer;

	// Token: 0x040044F6 RID: 17654
	public float InstantNoticeTimer;

	// Token: 0x040044F7 RID: 17655
	public float InvestigationTimer;

	// Token: 0x040044F8 RID: 17656
	public float PersonalSpaceTimer;

	// Token: 0x040044F9 RID: 17657
	public float CameraPoseTimer;

	// Token: 0x040044FA RID: 17658
	public float IgnoreFoodTimer;

	// Token: 0x040044FB RID: 17659
	public float RivalDeathTimer;

	// Token: 0x040044FC RID: 17660
	public float CuriosityTimer;

	// Token: 0x040044FD RID: 17661
	public float DistractTimer;

	// Token: 0x040044FE RID: 17662
	public float DramaticTimer;

	// Token: 0x040044FF RID: 17663
	public float MedicineTimer;

	// Token: 0x04004500 RID: 17664
	public float ReactionTimer;

	// Token: 0x04004501 RID: 17665
	public float WalkBackTimer;

	// Token: 0x04004502 RID: 17666
	public float AmnesiaTimer;

	// Token: 0x04004503 RID: 17667
	public float ElectroTimer;

	// Token: 0x04004504 RID: 17668
	public float StretchTimer;

	// Token: 0x04004505 RID: 17669
	public float PuzzleTimer;

	// Token: 0x04004506 RID: 17670
	public float GiggleTimer;

	// Token: 0x04004507 RID: 17671
	public float GoAwayTimer;

	// Token: 0x04004508 RID: 17672
	public float IgnoreTimer;

	// Token: 0x04004509 RID: 17673
	public float LyricsTimer;

	// Token: 0x0400450A RID: 17674
	public float MiyukiTimer;

	// Token: 0x0400450B RID: 17675
	public float MusumeTimer;

	// Token: 0x0400450C RID: 17676
	public float PatrolTimer;

	// Token: 0x0400450D RID: 17677
	public float PursueTimer;

	// Token: 0x0400450E RID: 17678
	public float ReportTimer;

	// Token: 0x0400450F RID: 17679
	public float SplashTimer;

	// Token: 0x04004510 RID: 17680
	public float ThreatTimer;

	// Token: 0x04004511 RID: 17681
	public float UpdateTimer;

	// Token: 0x04004512 RID: 17682
	public float AlarmTimer;

	// Token: 0x04004513 RID: 17683
	public float BatheTimer;

	// Token: 0x04004514 RID: 17684
	public float BullyTimer;

	// Token: 0x04004515 RID: 17685
	public float ChaseTimer;

	// Token: 0x04004516 RID: 17686
	public float CheerTimer;

	// Token: 0x04004517 RID: 17687
	public float CleanTimer;

	// Token: 0x04004518 RID: 17688
	public float LaughTimer;

	// Token: 0x04004519 RID: 17689
	public float RadioTimer;

	// Token: 0x0400451A RID: 17690
	public float SnackTimer;

	// Token: 0x0400451B RID: 17691
	public float SprayTimer;

	// Token: 0x0400451C RID: 17692
	public float StuckTimer;

	// Token: 0x0400451D RID: 17693
	public float ClubTimer;

	// Token: 0x0400451E RID: 17694
	public float HuntTimer;

	// Token: 0x0400451F RID: 17695
	public float MeetTimer;

	// Token: 0x04004520 RID: 17696
	public float PyroTimer;

	// Token: 0x04004521 RID: 17697
	public float ReadTimer;

	// Token: 0x04004522 RID: 17698
	public float SulkTimer;

	// Token: 0x04004523 RID: 17699
	public float TalkTimer;

	// Token: 0x04004524 RID: 17700
	public float WaitTimer;

	// Token: 0x04004525 RID: 17701
	public float SewTimer;

	// Token: 0x04004526 RID: 17702
	public float TargetWeaponDistance;

	// Token: 0x04004527 RID: 17703
	public float OriginalYPosition;

	// Token: 0x04004528 RID: 17704
	public float PreviousEyeShrink;

	// Token: 0x04004529 RID: 17705
	public float PhotoPatience;

	// Token: 0x0400452A RID: 17706
	public float PreviousAlarm;

	// Token: 0x0400452B RID: 17707
	public float ClubThreshold = 6f;

	// Token: 0x0400452C RID: 17708
	public float RepDeduction;

	// Token: 0x0400452D RID: 17709
	public float GoAwayLimit = 10f;

	// Token: 0x0400452E RID: 17710
	public float RepRecovery;

	// Token: 0x0400452F RID: 17711
	public float BreastSize;

	// Token: 0x04004530 RID: 17712
	public float DodgeSpeed = 2f;

	// Token: 0x04004531 RID: 17713
	public float Hesitation;

	// Token: 0x04004532 RID: 17714
	public float PendingRep;

	// Token: 0x04004533 RID: 17715
	public float Perception = 1f;

	// Token: 0x04004534 RID: 17716
	public float EyeShrink;

	// Token: 0x04004535 RID: 17717
	public float PyroLimit;

	// Token: 0x04004536 RID: 17718
	public float WalkSpeed = 1f;

	// Token: 0x04004537 RID: 17719
	public float MeetTime;

	// Token: 0x04004538 RID: 17720
	public float Paranoia;

	// Token: 0x04004539 RID: 17721
	public float RepLoss;

	// Token: 0x0400453A RID: 17722
	public float Health = 100f;

	// Token: 0x0400453B RID: 17723
	public float Alarm;

	// Token: 0x0400453C RID: 17724
	public float OriginalHairR = 1f;

	// Token: 0x0400453D RID: 17725
	public float OriginalHairG = 1f;

	// Token: 0x0400453E RID: 17726
	public float OriginalHairB = 1f;

	// Token: 0x0400453F RID: 17727
	public float OriginalEyeR = 1f;

	// Token: 0x04004540 RID: 17728
	public float OriginalEyeG = 1f;

	// Token: 0x04004541 RID: 17729
	public float OriginalEyeB = 1f;

	// Token: 0x04004542 RID: 17730
	public int ReturningMisplacedWeaponPhase;

	// Token: 0x04004543 RID: 17731
	public int RetrieveMedicinePhase;

	// Token: 0x04004544 RID: 17732
	public int WitnessRivalDiePhase;

	// Token: 0x04004545 RID: 17733
	public int ChangeClothingPhase;

	// Token: 0x04004546 RID: 17734
	public int InvestigationPhase;

	// Token: 0x04004547 RID: 17735
	public int MurderSuicidePhase;

	// Token: 0x04004548 RID: 17736
	public int ClubActivityPhase;

	// Token: 0x04004549 RID: 17737
	public int SeekMedicinePhase;

	// Token: 0x0400454A RID: 17738
	public int CameraReactPhase;

	// Token: 0x0400454B RID: 17739
	public int CuriosityPhase;

	// Token: 0x0400454C RID: 17740
	public int BakeSalePhase;

	// Token: 0x0400454D RID: 17741
	public int DramaticPhase;

	// Token: 0x0400454E RID: 17742
	public int GraffitiPhase;

	// Token: 0x0400454F RID: 17743
	public int SentHomePhase;

	// Token: 0x04004550 RID: 17744
	public int SunbathePhase;

	// Token: 0x04004551 RID: 17745
	public int ConfessPhase = 1;

	// Token: 0x04004552 RID: 17746
	public int SciencePhase;

	// Token: 0x04004553 RID: 17747
	public int StretchPhase;

	// Token: 0x04004554 RID: 17748
	public int LyricsPhase;

	// Token: 0x04004555 RID: 17749
	public int ReportPhase;

	// Token: 0x04004556 RID: 17750
	public int SplashPhase;

	// Token: 0x04004557 RID: 17751
	public int ThreatPhase = 1;

	// Token: 0x04004558 RID: 17752
	public int BathePhase;

	// Token: 0x04004559 RID: 17753
	public int BullyPhase;

	// Token: 0x0400455A RID: 17754
	public int RadioPhase = 1;

	// Token: 0x0400455B RID: 17755
	public int SnackPhase;

	// Token: 0x0400455C RID: 17756
	public int TrashPhase;

	// Token: 0x0400455D RID: 17757
	public int VomitPhase;

	// Token: 0x0400455E RID: 17758
	public int ClubPhase;

	// Token: 0x0400455F RID: 17759
	public int PyroPhase;

	// Token: 0x04004560 RID: 17760
	public int SulkPhase;

	// Token: 0x04004561 RID: 17761
	public int TaskPhase;

	// Token: 0x04004562 RID: 17762
	public int ReadPhase;

	// Token: 0x04004563 RID: 17763
	public int PinPhase;

	// Token: 0x04004564 RID: 17764
	public int Phase;

	// Token: 0x04004565 RID: 17765
	public PersonaType OriginalPersona;

	// Token: 0x04004566 RID: 17766
	public StudentInteractionType Interaction;

	// Token: 0x04004567 RID: 17767
	public int TimesWeaponWitnessed;

	// Token: 0x04004568 RID: 17768
	public int TimesBloodWitnessed;

	// Token: 0x04004569 RID: 17769
	public int TimesAlarmed;

	// Token: 0x0400456A RID: 17770
	public int TimesAnnoyed;

	// Token: 0x0400456B RID: 17771
	public int StinkBombSpecialCase;

	// Token: 0x0400456C RID: 17772
	public int ElectrocutionVictim;

	// Token: 0x0400456D RID: 17773
	public int AfterWitnessBonus;

	// Token: 0x0400456E RID: 17774
	public int BloodPoolsSpawned;

	// Token: 0x0400456F RID: 17775
	public int AnnoyedByGiggles;

	// Token: 0x04004570 RID: 17776
	public int LovestruckTarget;

	// Token: 0x04004571 RID: 17777
	public int MurdersWitnessed;

	// Token: 0x04004572 RID: 17778
	public int WeaponWitnessed;

	// Token: 0x04004573 RID: 17779
	public int AnnoyedByRadio;

	// Token: 0x04004574 RID: 17780
	public int MurderReaction;

	// Token: 0x04004575 RID: 17781
	public int PhaseFromSave;

	// Token: 0x04004576 RID: 17782
	public int TimesFollowed;

	// Token: 0x04004577 RID: 17783
	public int WarningsGiven;

	// Token: 0x04004578 RID: 17784
	public int CleaningRole;

	// Token: 0x04004579 RID: 17785
	public int StruggleWait;

	// Token: 0x0400457A RID: 17786
	public int TaskRejected;

	// Token: 0x0400457B RID: 17787
	public int WitnessBonus;

	// Token: 0x0400457C RID: 17788
	public int GossipBonus;

	// Token: 0x0400457D RID: 17789
	public int DeathCause;

	// Token: 0x0400457E RID: 17790
	public int Schoolwear;

	// Token: 0x0400457F RID: 17791
	public int SkinColor = 3;

	// Token: 0x04004580 RID: 17792
	public int Attempts;

	// Token: 0x04004581 RID: 17793
	public int Patience = 5;

	// Token: 0x04004582 RID: 17794
	public int Pestered;

	// Token: 0x04004583 RID: 17795
	public int RepBonus;

	// Token: 0x04004584 RID: 17796
	public int Strength;

	// Token: 0x04004585 RID: 17797
	public int Concern;

	// Token: 0x04004586 RID: 17798
	public int Defeats;

	// Token: 0x04004587 RID: 17799
	public int Crush;

	// Token: 0x04004588 RID: 17800
	public int CuddlePartnerID;

	// Token: 0x04004589 RID: 17801
	public int GenericTaskID;

	// Token: 0x0400458A RID: 17802
	public int InfatuationID;

	// Token: 0x0400458B RID: 17803
	public int PinDownID;

	// Token: 0x0400458C RID: 17804
	public StudentWitnessType PreviouslyWitnessed;

	// Token: 0x0400458D RID: 17805
	public StudentWitnessType Witnessed;

	// Token: 0x0400458E RID: 17806
	public GameOverType GameOverCause;

	// Token: 0x0400458F RID: 17807
	public DeathType DeathType;

	// Token: 0x04004590 RID: 17808
	public string OriginalDestination = string.Empty;

	// Token: 0x04004591 RID: 17809
	public string OriginalAction = string.Empty;

	// Token: 0x04004592 RID: 17810
	public string GenderPrefix = string.Empty;

	// Token: 0x04004593 RID: 17811
	public string CurrentAnim = string.Empty;

	// Token: 0x04004594 RID: 17812
	public string RivalPrefix = string.Empty;

	// Token: 0x04004595 RID: 17813
	public string RandomAnim = string.Empty;

	// Token: 0x04004596 RID: 17814
	public string Accessory = string.Empty;

	// Token: 0x04004597 RID: 17815
	public string Hairstyle = string.Empty;

	// Token: 0x04004598 RID: 17816
	public string Suffix = string.Empty;

	// Token: 0x04004599 RID: 17817
	public string Name = string.Empty;

	// Token: 0x0400459A RID: 17818
	public string OriginalOriginalWalkAnim = string.Empty;

	// Token: 0x0400459B RID: 17819
	public string OriginalOriginalSprintAnim = string.Empty;

	// Token: 0x0400459C RID: 17820
	public string OriginalIdleAnim = string.Empty;

	// Token: 0x0400459D RID: 17821
	public string OriginalWalkAnim = string.Empty;

	// Token: 0x0400459E RID: 17822
	public string OriginalSprintAnim = string.Empty;

	// Token: 0x0400459F RID: 17823
	public string OriginalLeanAnim = string.Empty;

	// Token: 0x040045A0 RID: 17824
	public string WalkAnim = string.Empty;

	// Token: 0x040045A1 RID: 17825
	public string RunAnim = string.Empty;

	// Token: 0x040045A2 RID: 17826
	public string SprintAnim = string.Empty;

	// Token: 0x040045A3 RID: 17827
	public string IdleAnim = string.Empty;

	// Token: 0x040045A4 RID: 17828
	public string TalkAnim = string.Empty;

	// Token: 0x040045A5 RID: 17829
	public string Nod1Anim = string.Empty;

	// Token: 0x040045A6 RID: 17830
	public string Nod2Anim = string.Empty;

	// Token: 0x040045A7 RID: 17831
	public string DefendAnim = string.Empty;

	// Token: 0x040045A8 RID: 17832
	public string DeathAnim = string.Empty;

	// Token: 0x040045A9 RID: 17833
	public string ScaredAnim = string.Empty;

	// Token: 0x040045AA RID: 17834
	public string EvilWitnessAnim = string.Empty;

	// Token: 0x040045AB RID: 17835
	public string LookDownAnim = string.Empty;

	// Token: 0x040045AC RID: 17836
	public string PhoneAnim = string.Empty;

	// Token: 0x040045AD RID: 17837
	public string AngryFaceAnim = string.Empty;

	// Token: 0x040045AE RID: 17838
	public string ToughFaceAnim = string.Empty;

	// Token: 0x040045AF RID: 17839
	public string InspectAnim = string.Empty;

	// Token: 0x040045B0 RID: 17840
	public string GuardAnim = string.Empty;

	// Token: 0x040045B1 RID: 17841
	public string CallAnim = string.Empty;

	// Token: 0x040045B2 RID: 17842
	public string CounterAnim = string.Empty;

	// Token: 0x040045B3 RID: 17843
	public string PushedAnim = string.Empty;

	// Token: 0x040045B4 RID: 17844
	public string GameAnim = string.Empty;

	// Token: 0x040045B5 RID: 17845
	public string BentoAnim = string.Empty;

	// Token: 0x040045B6 RID: 17846
	public string EatAnim = string.Empty;

	// Token: 0x040045B7 RID: 17847
	public string DrownAnim = string.Empty;

	// Token: 0x040045B8 RID: 17848
	public string WetAnim = string.Empty;

	// Token: 0x040045B9 RID: 17849
	public string SplashedAnim = string.Empty;

	// Token: 0x040045BA RID: 17850
	public string StripAnim = string.Empty;

	// Token: 0x040045BB RID: 17851
	public string ParanoidAnim = string.Empty;

	// Token: 0x040045BC RID: 17852
	public string GossipAnim = string.Empty;

	// Token: 0x040045BD RID: 17853
	public string SadSitAnim = string.Empty;

	// Token: 0x040045BE RID: 17854
	public string BrokenAnim = string.Empty;

	// Token: 0x040045BF RID: 17855
	public string BrokenSitAnim = string.Empty;

	// Token: 0x040045C0 RID: 17856
	public string BrokenWalkAnim = string.Empty;

	// Token: 0x040045C1 RID: 17857
	public string FistAnim = string.Empty;

	// Token: 0x040045C2 RID: 17858
	public string AttackAnim = string.Empty;

	// Token: 0x040045C3 RID: 17859
	public string SuicideAnim = string.Empty;

	// Token: 0x040045C4 RID: 17860
	public string RelaxAnim = string.Empty;

	// Token: 0x040045C5 RID: 17861
	public string SitAnim = string.Empty;

	// Token: 0x040045C6 RID: 17862
	public string ShyAnim = string.Empty;

	// Token: 0x040045C7 RID: 17863
	public string PeekAnim = string.Empty;

	// Token: 0x040045C8 RID: 17864
	public string ClubAnim = string.Empty;

	// Token: 0x040045C9 RID: 17865
	public string StruggleAnim = string.Empty;

	// Token: 0x040045CA RID: 17866
	public string StruggleWonAnim = string.Empty;

	// Token: 0x040045CB RID: 17867
	public string StruggleLostAnim = string.Empty;

	// Token: 0x040045CC RID: 17868
	public string SocialSitAnim = string.Empty;

	// Token: 0x040045CD RID: 17869
	public string CarryAnim = string.Empty;

	// Token: 0x040045CE RID: 17870
	public string ActivityAnim = string.Empty;

	// Token: 0x040045CF RID: 17871
	public string GrudgeAnim = string.Empty;

	// Token: 0x040045D0 RID: 17872
	public string SadFaceAnim = string.Empty;

	// Token: 0x040045D1 RID: 17873
	public string CowardAnim = string.Empty;

	// Token: 0x040045D2 RID: 17874
	public string EvilAnim = string.Empty;

	// Token: 0x040045D3 RID: 17875
	public string SocialReportAnim = string.Empty;

	// Token: 0x040045D4 RID: 17876
	public string SocialFearAnim = string.Empty;

	// Token: 0x040045D5 RID: 17877
	public string SocialTerrorAnim = string.Empty;

	// Token: 0x040045D6 RID: 17878
	public string BuzzSawDeathAnim = string.Empty;

	// Token: 0x040045D7 RID: 17879
	public string SwingDeathAnim = string.Empty;

	// Token: 0x040045D8 RID: 17880
	public string CyborgDeathAnim = string.Empty;

	// Token: 0x040045D9 RID: 17881
	public string WalkBackAnim = string.Empty;

	// Token: 0x040045DA RID: 17882
	public string PatrolAnim = string.Empty;

	// Token: 0x040045DB RID: 17883
	public string RadioAnim = string.Empty;

	// Token: 0x040045DC RID: 17884
	public string BookSitAnim = string.Empty;

	// Token: 0x040045DD RID: 17885
	public string BookReadAnim = string.Empty;

	// Token: 0x040045DE RID: 17886
	public string LovedOneAnim = string.Empty;

	// Token: 0x040045DF RID: 17887
	public string CuddleAnim = string.Empty;

	// Token: 0x040045E0 RID: 17888
	public string VomitAnim = string.Empty;

	// Token: 0x040045E1 RID: 17889
	public string WashFaceAnim = string.Empty;

	// Token: 0x040045E2 RID: 17890
	public string EmeticAnim = string.Empty;

	// Token: 0x040045E3 RID: 17891
	public string BurningAnim = string.Empty;

	// Token: 0x040045E4 RID: 17892
	public string JojoReactAnim = string.Empty;

	// Token: 0x040045E5 RID: 17893
	public string TeachAnim = string.Empty;

	// Token: 0x040045E6 RID: 17894
	public string LeanAnim = string.Empty;

	// Token: 0x040045E7 RID: 17895
	public string DeskTextAnim = string.Empty;

	// Token: 0x040045E8 RID: 17896
	public string CarryShoulderAnim = string.Empty;

	// Token: 0x040045E9 RID: 17897
	public string ReadyToFightAnim = string.Empty;

	// Token: 0x040045EA RID: 17898
	public string SearchPatrolAnim = string.Empty;

	// Token: 0x040045EB RID: 17899
	public string DiscoverPhoneAnim = string.Empty;

	// Token: 0x040045EC RID: 17900
	public string WaitAnim = string.Empty;

	// Token: 0x040045ED RID: 17901
	public string ShoveAnim = string.Empty;

	// Token: 0x040045EE RID: 17902
	public string SprayAnim = string.Empty;

	// Token: 0x040045EF RID: 17903
	public string SithReactAnim = string.Empty;

	// Token: 0x040045F0 RID: 17904
	public string EatVictimAnim = string.Empty;

	// Token: 0x040045F1 RID: 17905
	public string RandomGossipAnim = string.Empty;

	// Token: 0x040045F2 RID: 17906
	public string CuteAnim = string.Empty;

	// Token: 0x040045F3 RID: 17907
	public string BulliedIdleAnim = string.Empty;

	// Token: 0x040045F4 RID: 17908
	public string BulliedWalkAnim = string.Empty;

	// Token: 0x040045F5 RID: 17909
	public string BullyVictimAnim = string.Empty;

	// Token: 0x040045F6 RID: 17910
	public string SadDeskSitAnim = string.Empty;

	// Token: 0x040045F7 RID: 17911
	public string ConfusedSitAnim = string.Empty;

	// Token: 0x040045F8 RID: 17912
	public string SentHomeAnim = string.Empty;

	// Token: 0x040045F9 RID: 17913
	public string RandomCheerAnim = string.Empty;

	// Token: 0x040045FA RID: 17914
	public string ParanoidWalkAnim = string.Empty;

	// Token: 0x040045FB RID: 17915
	public string SleuthIdleAnim = string.Empty;

	// Token: 0x040045FC RID: 17916
	public string SleuthWalkAnim = string.Empty;

	// Token: 0x040045FD RID: 17917
	public string SleuthCalmAnim = string.Empty;

	// Token: 0x040045FE RID: 17918
	public string SleuthScanAnim = string.Empty;

	// Token: 0x040045FF RID: 17919
	public string SleuthReactAnim = string.Empty;

	// Token: 0x04004600 RID: 17920
	public string SleuthSprintAnim = string.Empty;

	// Token: 0x04004601 RID: 17921
	public string SleuthReportAnim = string.Empty;

	// Token: 0x04004602 RID: 17922
	public string RandomSleuthAnim = string.Empty;

	// Token: 0x04004603 RID: 17923
	public string BreakUpAnim = string.Empty;

	// Token: 0x04004604 RID: 17924
	public string PaintAnim = string.Empty;

	// Token: 0x04004605 RID: 17925
	public string SketchAnim = string.Empty;

	// Token: 0x04004606 RID: 17926
	public string RummageAnim = string.Empty;

	// Token: 0x04004607 RID: 17927
	public string ThinkAnim = string.Empty;

	// Token: 0x04004608 RID: 17928
	public string ActAnim = string.Empty;

	// Token: 0x04004609 RID: 17929
	public string OriginalClubAnim = string.Empty;

	// Token: 0x0400460A RID: 17930
	public string MiyukiAnim = string.Empty;

	// Token: 0x0400460B RID: 17931
	public string VictoryAnim = string.Empty;

	// Token: 0x0400460C RID: 17932
	public string PlateIdleAnim = string.Empty;

	// Token: 0x0400460D RID: 17933
	public string PlateWalkAnim = string.Empty;

	// Token: 0x0400460E RID: 17934
	public string PlateEatAnim = string.Empty;

	// Token: 0x0400460F RID: 17935
	public string PrepareFoodAnim = string.Empty;

	// Token: 0x04004610 RID: 17936
	public string PoisonDeathAnim = string.Empty;

	// Token: 0x04004611 RID: 17937
	public string HeadacheAnim = string.Empty;

	// Token: 0x04004612 RID: 17938
	public string HeadacheSitAnim = string.Empty;

	// Token: 0x04004613 RID: 17939
	public string ElectroAnim = string.Empty;

	// Token: 0x04004614 RID: 17940
	public string EatChipsAnim = string.Empty;

	// Token: 0x04004615 RID: 17941
	public string DrinkFountainAnim = string.Empty;

	// Token: 0x04004616 RID: 17942
	public string PullBoxCutterAnim = string.Empty;

	// Token: 0x04004617 RID: 17943
	public string TossNoteAnim = string.Empty;

	// Token: 0x04004618 RID: 17944
	public string KeepNoteAnim = string.Empty;

	// Token: 0x04004619 RID: 17945
	public string BathingAnim = string.Empty;

	// Token: 0x0400461A RID: 17946
	public string DodgeAnim = string.Empty;

	// Token: 0x0400461B RID: 17947
	public string InspectBloodAnim = string.Empty;

	// Token: 0x0400461C RID: 17948
	public string PickUpAnim = string.Empty;

	// Token: 0x0400461D RID: 17949
	public string PuzzleAnim = string.Empty;

	// Token: 0x0400461E RID: 17950
	public string LandLineAnim = string.Empty;

	// Token: 0x0400461F RID: 17951
	public string SulkAnim = string.Empty;

	// Token: 0x04004620 RID: 17952
	public string BeforeReturnAnim = string.Empty;

	// Token: 0x04004621 RID: 17953
	public string AdmireAnim = string.Empty;

	// Token: 0x04004622 RID: 17954
	public string StretchAnim = string.Empty;

	// Token: 0x04004623 RID: 17955
	public string LookLeftRightAnim = string.Empty;

	// Token: 0x04004624 RID: 17956
	public string PinDownAnim = string.Empty;

	// Token: 0x04004625 RID: 17957
	public string WaveAnim = string.Empty;

	// Token: 0x04004626 RID: 17958
	public string MurderSuicideAnim = string.Empty;

	// Token: 0x04004627 RID: 17959
	public string CustomHangoutAnim = string.Empty;

	// Token: 0x04004628 RID: 17960
	public string CustomPatrolAnim = string.Empty;

	// Token: 0x04004629 RID: 17961
	public string[] AdmireAnims;

	// Token: 0x0400462A RID: 17962
	public string[] CleanAnims;

	// Token: 0x0400462B RID: 17963
	public string[] CameraAnims;

	// Token: 0x0400462C RID: 17964
	public string[] SocialAnims;

	// Token: 0x0400462D RID: 17965
	public string[] CowardAnims;

	// Token: 0x0400462E RID: 17966
	public string[] EvilAnims;

	// Token: 0x0400462F RID: 17967
	public string[] HeroAnims;

	// Token: 0x04004630 RID: 17968
	public string[] TaskAnims;

	// Token: 0x04004631 RID: 17969
	public string[] PhoneAnims;

	// Token: 0x04004632 RID: 17970
	public string[] StretchAnims;

	// Token: 0x04004633 RID: 17971
	public string[] IdleAnims;

	// Token: 0x04004634 RID: 17972
	public string[] WalkAnims;

	// Token: 0x04004635 RID: 17973
	public int ClubMemberID;

	// Token: 0x04004636 RID: 17974
	public int AnimSetID;

	// Token: 0x04004637 RID: 17975
	public int StudentID;

	// Token: 0x04004638 RID: 17976
	public int WitnessID;

	// Token: 0x04004639 RID: 17977
	public int CorpseID;

	// Token: 0x0400463A RID: 17978
	public int PatrolID;

	// Token: 0x0400463B RID: 17979
	public int SleuthID;

	// Token: 0x0400463C RID: 17980
	public int BullyID;

	// Token: 0x0400463D RID: 17981
	public int CleanID;

	// Token: 0x0400463E RID: 17982
	public int GuardID;

	// Token: 0x0400463F RID: 17983
	public int GirlID;

	// Token: 0x04004640 RID: 17984
	public int Class;

	// Token: 0x04004641 RID: 17985
	public int ID;

	// Token: 0x04004642 RID: 17986
	public PersonaType Persona;

	// Token: 0x04004643 RID: 17987
	public ClubType OriginalClub;

	// Token: 0x04004644 RID: 17988
	public ClubType Club;

	// Token: 0x04004645 RID: 17989
	public Vector3 OriginalBloodPoolLocation;

	// Token: 0x04004646 RID: 17990
	public Vector3 OriginalPlatePosition;

	// Token: 0x04004647 RID: 17991
	public Vector3 OriginalPosition;

	// Token: 0x04004648 RID: 17992
	public Vector3 LastKnownCorpse;

	// Token: 0x04004649 RID: 17993
	public Vector3 DistractionSpot;

	// Token: 0x0400464A RID: 17994
	public Vector3 LastKnownBlood;

	// Token: 0x0400464B RID: 17995
	public Vector3 RightEyeOrigin;

	// Token: 0x0400464C RID: 17996
	public Vector3 LeftEyeOrigin;

	// Token: 0x0400464D RID: 17997
	public Vector3 PreviousSkirt;

	// Token: 0x0400464E RID: 17998
	public Vector3 LastPosition;

	// Token: 0x0400464F RID: 17999
	public Vector3 BurnTarget;

	// Token: 0x04004650 RID: 18000
	public Vector3 OriginalEyePos;

	// Token: 0x04004651 RID: 18001
	public Vector3 OriginalEyeRot;

	// Token: 0x04004652 RID: 18002
	public Transform RightBreast;

	// Token: 0x04004653 RID: 18003
	public Transform LeftBreast;

	// Token: 0x04004654 RID: 18004
	public Transform RightEye;

	// Token: 0x04004655 RID: 18005
	public Transform LeftEye;

	// Token: 0x04004656 RID: 18006
	public int Frame;

	// Token: 0x04004657 RID: 18007
	private float MaxSpeed = 10f;

	// Token: 0x04004658 RID: 18008
	private const string RIVAL_PREFIX = "Rival ";

	// Token: 0x04004659 RID: 18009
	public Vector3[] SkirtPositions;

	// Token: 0x0400465A RID: 18010
	public Vector3[] SkirtRotations;

	// Token: 0x0400465B RID: 18011
	public Vector3[] SkirtOrigins;

	// Token: 0x0400465C RID: 18012
	public Transform DefaultTarget;

	// Token: 0x0400465D RID: 18013
	public Transform GushTarget;

	// Token: 0x0400465E RID: 18014
	public bool Gush;

	// Token: 0x0400465F RID: 18015
	public float LookSpeed = 2f;

	// Token: 0x04004660 RID: 18016
	public float TimeOfDeath;

	// Token: 0x04004661 RID: 18017
	public int Fate;

	// Token: 0x04004662 RID: 18018
	public float TeacherTimer;

	// Token: 0x04004663 RID: 18019
	public LowPolyStudentScript LowPoly;

	// Token: 0x04004664 RID: 18020
	public GameObject EightiesPhone;

	// Token: 0x04004665 RID: 18021
	public Material BloodMaterial;

	// Token: 0x04004666 RID: 18022
	public Material BrownMaterial;

	// Token: 0x04004667 RID: 18023
	public Material WaterMaterial;

	// Token: 0x04004668 RID: 18024
	public Material GasMaterial;

	// Token: 0x04004669 RID: 18025
	public bool NoScream;

	// Token: 0x0400466A RID: 18026
	public GameObject JojoHitEffect;

	// Token: 0x0400466B RID: 18027
	public GameObject[] ElectroSteam;

	// Token: 0x0400466C RID: 18028
	public GameObject[] CensorSteam;

	// Token: 0x0400466D RID: 18029
	public Texture NudeTexture;

	// Token: 0x0400466E RID: 18030
	public Mesh BaldNudeMesh;

	// Token: 0x0400466F RID: 18031
	public Mesh NudeMesh;

	// Token: 0x04004670 RID: 18032
	public Texture TowelTexture;

	// Token: 0x04004671 RID: 18033
	public Mesh TowelMesh;

	// Token: 0x04004672 RID: 18034
	public Mesh SwimmingTrunks;

	// Token: 0x04004673 RID: 18035
	public Mesh SchoolSwimsuit;

	// Token: 0x04004674 RID: 18036
	public Mesh GymUniform;

	// Token: 0x04004675 RID: 18037
	public Texture GyaruSwimsuitTexture;

	// Token: 0x04004676 RID: 18038
	public Texture EightiesGymTexture;

	// Token: 0x04004677 RID: 18039
	public Texture SwimsuitTexture;

	// Token: 0x04004678 RID: 18040
	public Texture UniformTexture;

	// Token: 0x04004679 RID: 18041
	public Texture GymTexture;

	// Token: 0x0400467A RID: 18042
	public Texture TitanBodyTexture;

	// Token: 0x0400467B RID: 18043
	public Texture TitanFaceTexture;

	// Token: 0x0400467C RID: 18044
	public bool Spooky;

	// Token: 0x0400467D RID: 18045
	public Mesh JudoGiMesh;

	// Token: 0x0400467E RID: 18046
	public Texture JudoGiTexture;

	// Token: 0x0400467F RID: 18047
	public RiggedAccessoryAttacher Attacher;

	// Token: 0x04004680 RID: 18048
	public bool WearingBikini;

	// Token: 0x04004681 RID: 18049
	public Mesh NoArmsNoTorso;

	// Token: 0x04004682 RID: 18050
	public GameObject RiggedAccessory;

	// Token: 0x04004683 RID: 18051
	public int CoupleID;

	// Token: 0x04004684 RID: 18052
	public int PartnerID;

	// Token: 0x04004685 RID: 18053
	public bool SearchingForPhone;

	// Token: 0x04004686 RID: 18054
	public float ChameleonBonus;

	// Token: 0x04004687 RID: 18055
	public bool Chameleon;

	// Token: 0x04004688 RID: 18056
	public bool SleuthInitialized;

	// Token: 0x04004689 RID: 18057
	public int TeacherID = 90;

	// Token: 0x0400468A RID: 18058
	public RiggedAccessoryAttacher LabcoatAttacher;

	// Token: 0x0400468B RID: 18059
	public RiggedAccessoryAttacher BikiniAttacher;

	// Token: 0x0400468C RID: 18060
	public RiggedAccessoryAttacher ApronAttacher;

	// Token: 0x0400468D RID: 18061
	public Mesh HeadAndHands;

	// Token: 0x0400468E RID: 18062
	private bool NoMentor;

	// Token: 0x0400468F RID: 18063
	public bool BagPlaced;

	// Token: 0x04004690 RID: 18064
	public RaycastHit hit;

	// Token: 0x04004691 RID: 18065
	public Transform RaycastOrigin;

	// Token: 0x04004692 RID: 18066
	public bool TooCloseToWall;

	// Token: 0x04004693 RID: 18067
	private Vector3 RaycastOriginVector;

	// Token: 0x04004694 RID: 18068
	public bool ResumeFollowingAfter;

	// Token: 0x04004695 RID: 18069
	public float[] OriginalTimes;

	// Token: 0x04004696 RID: 18070
	public string[] OriginalDests;

	// Token: 0x04004697 RID: 18071
	public string[] OriginalActs;

	// Token: 0x04004698 RID: 18072
	public OutlineScript[] RiggedAccessoryOutlines;

	// Token: 0x04004699 RID: 18073
	public int RiggedAccessoryOutlineID;

	// Token: 0x0400469A RID: 18074
	public MiniMapComponent MiniMapIcon;

	// Token: 0x0400469B RID: 18075
	public Sprite SenpaiSprite;

	// Token: 0x0400469C RID: 18076
	public Sprite RivalSprite;

	// Token: 0x0400469D RID: 18077
	public float SavePositionX;

	// Token: 0x0400469E RID: 18078
	public float SavePositionY;

	// Token: 0x0400469F RID: 18079
	public float SavePositionZ;

	// Token: 0x040046A0 RID: 18080
	public string ReturnDestination;

	// Token: 0x040046A1 RID: 18081
	public string ReturnAction;
}
