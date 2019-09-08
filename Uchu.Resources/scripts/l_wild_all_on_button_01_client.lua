require('o_mis')
require('L_NP_NPC')

function onClientUse(self)
	                       
	SetMouseOverDistance(self, 100)
	self:PlayFXEffect{effectType = "press"}
    local friends = self:GetObjectsInGroup{ group = "drum01" }.objects
        for i = 1, table.maxn (friends) do      
            if friends[i]:GetLOT().objtemplate == 3556 then
            friends[i]:NotifyObject{ name = "repeatloop" }
            end
        end  
end
--[[
function onUse(self, msg)
    local friends = self:GetObjectsInGroup{ group = "drum01" }.objects
        for i = 1, table.maxn (friends) do      
            if friends[i]:GetLOT().objtemplate == 3556 then
            friends[i]:NotifyObject{ name = "repeatloop" }
            end
        end  
end
--]]
