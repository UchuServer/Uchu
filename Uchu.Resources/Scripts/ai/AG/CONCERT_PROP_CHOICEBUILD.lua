--------------------------------------------------------------------------------------------------------
-- Server-side script for Concert Props Choice Builds. Each prop can be built into 1 of 4 types.
-- If all 4 are built into the same thing, the Stage transforms, and 30 seconds later they all return to their "default" state (the object's render component)
-- Based on scripts\ai\YRK\L_TB_BENCH.lua


-- 5023 AG - Stage Rocket               OLD 4029 AG – Fog Machine Choice Build
-- 4891 AG - Stage Spot Light           OLD 4030 AG – Spotlight Choice Build	
-- 5024 AG - Stage laser                OLD 4031 AG – Laser Light Choice Build	
-- 4858 AG - Speaker                    OLD 4032 AG – Speaker Choice Build	

--HF waypoints used to spawn in choice builds
--Concert_Prop_1
--Concert_Prop_2
--Concert_Prop_3
--Concert_Prop_4
---------------------------------------------------------------------------------------------------------



--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('c_AvantGardens')




-- All the lot nums for the prop choicebuilds
LOT_NUMS = {}
LOT_NUMS[1] = { CONSTANTS["LOT_CHOICEBUILD_ROCKET"], 
				CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"],
				CONSTANTS["LOT_CHOICEBUILD_LASER"],
				CONSTANTS["LOT_CHOICEBUILD_SPEAKER"] }                -- old Lot Nums from Bench script = { 3626, 3627, 3628, 3700 }


function onStartup(self)

end


function onTripleBuildSelect(self, msg)
	
	local canswap = 0
	
	-- Make sure we are allowed to spawn the piece they are requesting
	for i = 1, #LOT_NUMS do
		for a = 1, #LOT_NUMS[i] do
			if(LOT_NUMS[i][a] == msg.lotnum) then
				canswap = 1 -- how do you break out of a loop in lua?
			end
		end
	end
	
	if(canswap == 0) then
		--print("CANT SWAP")
		return
	end
		
	local mypos = self:GetPosition().pos
	local myRot = self:GetRotation()
	
    --print (self:GetID())
    self:GetParentObj().objIDParent:NotifyObject{ObjIDSender=self, name="ChoicebuildChanged", param1=msg.lotnum}
	RESMGR:LoadObject { objectTemplate = msg.lotnum, x= mypos.x, y= mypos.y , z= mypos.z, owner = self:GetParentObj().objIDParent, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z }
	
    GAMEOBJ:DeleteObject(self)
    
	--take imagination from the player
	msg.user:SetImagination{imagination = msg.user:GetImagination{}.imagination - 1}
end

