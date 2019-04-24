require('State')
require('/client/mission_Temp')

function onStartup(self) 
	Miss = {}

	--/////////////////////////////////////////////////////////////////////////
	--  Enter/Exit Conduct Radius
	--/////////////////////////////////////////////////////////////////////////
		Miss['Conduct_CoolDown']  = 1       	-- Affects the Trigger time upon exiting/entering the conduct radius
		Miss['Conduct_Delay']     	= 1       	-- Delay before triggering the emote
		Miss['Main_EmoteID']      	= 69        	-- Global emote ID 
	-- Conduct Radius 
		Miss['OverRideConduct']   	= true 
		Miss['conductRadius']     	= 20
	-- Conduct -----------------------OnEnter
		Miss['Emote_enter']     	= "enter"
	-- Conduct -----------------------OnExit
		Miss['Emote_onExit']    	= "leave"


	--///////////////////////////////////////////////////////////////////////////
	--  Mission States
	--/////////////////////////////////////////////////////////////////////////// 
	-- Emote ------------------------------------------------------[[ OnAVAILABLE ]]
		Miss['Emote_onAVAILABLE'] = "missionState1"  
	-- Emote  -----------------------------------------------------[[ OnACTIVE ]]
		Miss['Emote_onACTIVE '] = "missionState2" 
	-- Emote  -----------------------------------------------------[[ OnREADY_TO_COMPLETE ]]
		Miss['Emote_onREADY_TO_COMPLETE '] = "missionState3" 
	-- Emote  -----------------------------------------------------[[ OnSTATE_COMPLETED ]]
		Miss['Emote_onSTATE_COMPLETED '] = "missionState4" 
	  
	  
	--[[
	LWO_MissSION_STATE_AVAILABLE = 0x1,
	LWO_MissSION_STATE_ACTIVE = 0x2,
	LWO_MissSION_STATE_READY_TO_COMPLETE = 0x4,
	LWO_MissSION_STATE_COMPLETED = 0x8,
	 --]]    
	 
	------ Do not change ------------------------------------------------
		self:SetVar("Miss",Miss)
		GetMissionVars(self)
		CreateMissionStates(self)
	-------------------------------------------------------------------------------

end