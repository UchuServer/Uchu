
-----------------------------------------------------------
-- Checks Mission Status to start Air Strike
-- Updated 3/22 Darren McKinsey
-----------------------------------------------------------
-- function onStartup(self)
    -- math.randomseed( os.time() )
-- end


function onMissionDialogueOK(self,msg)
	print("called mission giver")
	if (msg.bIsComplete == true) then       
        print('Bomb Now')       
    end
end


-- function onUse(self, msg)
    -- if self:GetLOT().objtemplate == 6859 and not self:GetVar('isInUse') then
        --print('pass sever')
        -- local obj = self:GetObjectsInGroup{ group = "Jet_FX", ignoreSpawners = true }.objects[1]
            
        -- self:NotifyClientObject{name = 'toggleInUse', param1 = 1}
        -- self:SetVar('isInUse', true)
        -- obj:PlayAnimation{ animationID = "jetFX" }
        -- self:PlayFXEffect{name = "radarDish", effectType = "create", effectID = 641}        
        -- GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "radarDish", self )
        -- GAMEOBJ:GetTimer():AddTimerWithCancel( 2.5, "PlayEffect", self )
        -- GAMEOBJ:GetTimer():AddTimerWithCancel( LEVEL:GetCinematicInfo( "Jet_Cam_01" )+5, "CineDone", self )
    -- end
-- end 

-- function onRebuildComplete(self, msg)
    -- if self:GetLOT().objtemplate == 6209 then
        -- local obj = self:GetObjectsInGroup{ group = "Jet_FX", ignoreSpawners = true }.objects[1]
        
        -- obj:PlayAnimation{ animationID = "jetFX" }
    -- end
-- end  

-- function onTimerDone (self,msg)
    -- if (msg.name == "radarDish") then   
        -- self:StopFXEffect{name = msg.name}
    -- elseif (msg.name == "PlayEffect") then  
        --group name = mortarMain
        -- local obj = self:GetObjectsInGroup{ group = "mortarMain", ignoreSpawners = true }.objects         
        -- local test = math.random(1,#obj)
        --print(test)
        -- if test > 0 then
            -- obj[test]:CastSkill{ skillID = 318 }
        -- end
    -- elseif (msg.name == "CineDone") then  
        -- self:NotifyClientObject{name = 'toggleInUse', param1 = -1}
        -- self:SetVar('isInUse', false)
    -- end
-- end