--require('o_mis')

function onClientUse(self)

--    print "Return cam to normal"

    	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

        CAMERA:SetToPrevGameCam()

        player:RemoveSkill{ skillID = 170 }
        player:AddSkill{ skillID = 174 }
        player:CastSkill{ optionalTargetID = self, skillID = 174 }
        player:SetPosition {pos = {x=-186,y=184.28,z=-551}}
        player:SetRotation {x=0,y=-1,z=0,w=0}

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