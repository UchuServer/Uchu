
require('o_mis')
require('c_GnarledForest')

---------------------------------------------------------------------------------------------------------
-- Client-side script for Concert Props Choice Builds. Each prop can be built into 1 of 4 types.
-- If all 4 are built into the same thing, the Stage transforms, and 30 seconds later they all return to their "default" state (the object's render component)
-- Based on scripts\client\ai\YRK\L_TB_BENCH.lua


-- 5023 AG - Stage Rocket               OLD 4029 AG – Fog Machine Choice Build
-- 4891 AG - Stage Spot Light           OLD 4030 AG – Spotlight Choice Build	
-- 5024 AG - Stage laser                OLD 4031 AG – Laser Light Choice Build	
-- 4858 AG - Speaker                    OLD 4032 AG – Speaker Choice Build

-- Destructible component 348
---------------------------------------------------------------------------------------------------------

-- All the lot nums for the prop choicebuilds



function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end


function onClientUse(self, msg)
	--make sure we have enough imagination
	if msg.user:GetImagination{}.imagination > 0 then
		
		-- Send every LOT number that isn't us
		local ournum = 1;
		-- Store all the lots nums that arent us
		-- TODO This should be in a table so that it would be dynamic, but not sure how to store one with SetVar yet
		local slot = self:GetVar("ourslot")
		for i = 1, #LOT_NUMS[slot] do
			if(self:GetVar("ourlotnum") ~= LOT_NUMS[slot][i]) then 
				if(ournum == 1) then
					self:SetVar("lot1", LOT_NUMS[slot][i])
				elseif(ournum == 2) then
					self:SetVar("lot2", LOT_NUMS[slot][i])
				elseif(ournum == 3) then
					self:SetVar("lot3", LOT_NUMS[slot][i])
				end
				
				ournum = ournum + 1
				
				--local nextLot = #OUR_LOTNUMS + 1
				--OUR_LOTNUMS[nextLot] = LOT_NUMS[i]
			end
		end
		print ("in onclientuse")
		UI:DisplayTripleBuild( self, true, { self:GetVar("lot1"), self:GetVar("lot2"), self:GetVar("lot3") } )
        print ( tostring(self:GetVar("lot1")).. tostring(self:GetVar("lot2")).. tostring(self:GetVar("lot3")))
		
		print("Displaying UI - -- ---- -------------------------------------")
		--UI:DisplayTripleBuild( self, true, { OUR_LOTNUMS[1], OUR_LOTNUMS[2], OUR_LOTNUMS[3] } )
	else
        --display tooltip
        msg.user:DisplayTooltip { bShow = true, strText = Localize("AG_TOOLTIP_CON_IMAGINATION"), iTime = 5000 }

	end
end

--[[
function onStartup(self)
	-- Save our LOT number
    self:SetVar("ourlotnum", self:GetLOT{}.objtemplate)
	for i = 1, #LOT_NUMS do
		for a = 1, #LOT_NUMS[i] do
			if(self:GetVar("ourlotnum") == LOT_NUMS[i][a]) then
				self:SetVar("ourslot", i)
				return
			end
		end
	end
end
]]--