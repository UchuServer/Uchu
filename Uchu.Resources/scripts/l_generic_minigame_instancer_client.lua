--------------------------------------------------------------
-- Client script for FV Dragon Instance.
-- Lets client know the object can be interacted with
--
-- CREATED mrb... 11/4/2010
--------------------------------------------------------------
require('BASE_INSTANCER')

--------------------------------------------------------------
-- HF Config data settings, defaults listed
--type 			-> 0:AG_Survival_01								-- UI type to use
-- Drag to interact variables, only used if Racing type. 
-- ** These dont need to be updated unless a different object is to be dragged onto the instancer. **
-- *** drag to interact instancers must have L_GENERIC_MINIGAME_INSTANCER_SERVER.lua attached server side to work. ***
--itemType 		-> 1:8092										-- LOT that can be dragged onto the instancer, default is modular race car
--failItemText	-> 0:MINIGAME_LOBBY_RACE_DRAG_ITEM_FAIL_MESSAGE	-- text to be displayed if you drag the wrong object onto the instancer
--------------------------------------------------------------

local defaultType = "AG_Survival_01"
local defaultItemType = 8092 -- allow vehicles to start the racing
local defaultFailItem = "MINIGAME_LOBBY_RACE_DRAG_ITEM_FAIL_MESSAGE"
		
function onStartup(self)
	local tVars = {}
	
	tVars.UI_Type = self:GetVar("type") or defaultType
	
    local instanceType = split(tVars.UI_Type, "_")[2]    
    
    if instanceType == "Race" then	
		tVars.itemType = self:GetVar("itemType") or defaultItemType -- allow vehicles to start the racing
		tVars.failItem = Localize(self:GetVar("failItemText") or defaultFailItem)
	end
	
    baseSetVars(self, tVars)
end 
