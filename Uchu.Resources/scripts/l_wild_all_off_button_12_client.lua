require('o_mis')
require('L_NP_NPC')

function onClientUse(self)
	                       
	SetMouseOverDistance(self, 100)
	self:PlayFXEffect{effectType = "press"}
    local friends = self:GetObjectsInGroup{ group = "drum12" }.objects
        for i = 1, table.maxn (friends) do      
            if friends[i]:GetLOT().objtemplate == 3567 then
            friends[i]:NotifyObject{ name = "stop" }
            end
        end  
end