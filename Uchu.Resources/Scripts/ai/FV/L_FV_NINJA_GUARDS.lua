--------------------------------------------------------------
-- Script for Forbidden Valley Guards.  There are two guards at the gate in FV.  If you emote a "Roar" animation in front of either, the other laughs.
-- created eb... 6/25/10
--------------------------------------------------------------

function onStartup(self,msg)
    -- Making both guards identifiable to getobjectsingroup
    self:AddObjectToGroup{group = "ninjaguard"}
end

function onEmoteReceived(self,msg)

    local ninjas = self:GetObjectsInGroup{ group = "ninjaguard"}.objects

    if (msg.emoteID ~= 392) then 
        -- If player tries any emote other than roar, just shake her head
        self:PlayAnimation{ animationID = "no" }
    else 
        if self:GetLOT().objtemplate == 7412 and msg then
            -- Make the guard look scared
            self:PlayAnimation{ animationID = "scared" }
            -- Tell the other guard to laugh
            for i = 1, table.maxn (ninjas) do
                if (ninjas[i]:GetLOT().objtemplate == 11128) then
                    ninjas[i]:PlayAnimation{ animationID = "laugh_rt" }
                end
            end
        elseif self:GetLOT().objtemplate == 11128 then
            -- Make the guard look scared
            self:PlayAnimation{ animationID = "scared" }
            -- Tell the other guard to laugh
            for i = 1, table.maxn (ninjas) do
                if (ninjas[i]:GetLOT().objtemplate == 7412) then
                    ninjas[i]:PlayAnimation{ animationID = "laugh_lt" }
                end
            end
        end
    end
end