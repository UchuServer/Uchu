--require('o_mis')

function onClientUse(self)

--    print "Return cam to normal"

    	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

        CAMERA:SetToPrevGameCam()

        player:RemoveSkill{ skillID = 170 }
        player:AddSkill{ skillID = 174 }
        player:CastSkill{ optionalTargetID = self, skillID = 174 }
        player:SetPosition {pos = {x = -106.02,y = 189.99, z = -531}}

        local plane = self:GetObjectsInGroup{ group = "Level" }.objects

            for i = 1, table.maxn (plane) do      
                if plane[i]:GetLOT().objtemplate == 5850 then
                     plane[i]:NotifyObject{ name="Raise" }
                end              
            end

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end