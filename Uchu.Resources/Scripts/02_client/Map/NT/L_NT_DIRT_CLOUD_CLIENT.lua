--------------------------------------------------------------
-- server side script on the dirt clouds in NT
--
-- created by Brandi - 4/1/11... need to add daily missions when they get created
--------------------------------------------------------------
require('02_client/Map/General/L_VIS_TOGGLE_OBJ')

-- table to find the mission ID based on the spawner network name
local VisibilityObjectTable = {["Dirt_Clouds_Sent"] = {1253}, 
				               ["Dirt_Clouds_Assem"] = {1276}, 
				               ["Dirt_Clouds_Para"] = {1277}, 
                               ["Dirt_Clouds_Halls"] = {1283}}
                               
----------------------------------------------------------------
-- Catch object instantiation
----------------------------------------------------------------                   
function onStartup(self)

	setGameVariables(self, VisibilityObjectTable)
	
end 
        