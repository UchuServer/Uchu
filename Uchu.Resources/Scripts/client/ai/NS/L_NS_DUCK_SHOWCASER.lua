--------------------------------------------------------------
-- Duck Showcaser script; this script does the logic required 
-- by the duck showcase mission. Gives brick group/thinking hat.
-- created mrb... 3/02/10
--------------------------------------------------------------

local releaseVersion = 200 -- which version release # the content should be made available for Beta 1
local misID = 370
local duckBricks = {["29"] = 1, -- format is {["LOT_NUM"] = NUM_OF_BRICKS}
                    ["31"] = 2, 
                    ["32"] = 1,  
                    ["40"] = 2} 

function onStartup(self)
    math.randomseed( os.time() )
end

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse, checks to see if we 
-- in a beta 1 and sends a fail message.
----------------------------------------------
function onCheckUseRequirements(self, msg)
    local verInfo = msg.objIDUser:GetVersioningInfo()
    
    if not verInfo.bIsInternal and verInfo.iMajorRelease < 1 and verInfo.iVersionRelease < releaseVersion then
        if msg.objIDUser:GetInvItemCount{iObjTemplate = 6086}.itemCount > 0 then
            --print('remove thinking cap 1')
            msg.objIDUser:DisplayTooltip { bShow = true, strText = "Here is your Level 2 Thinking Hat, return to Mardolf.", iTime = 3000 }
            
            msg.objIDUser:RemoveItemFromInventory{iObjTemplate = 6086}
            msg.objIDUser:AddItemToInventory{iObjTemplate = 7009, itemCount = 1}
        else
            --msg.objIDUser:RemoveItemFromInventory{iObjTemplate = 7009}
            msg.objIDUser:DisplayTooltip { bShow = true, strText = "Showcases are unavailable at this time.", iTime = 3000 }
        end

        msg.bCanUse = false
    end
    
    return msg
end

function onSetIconAboveHead(self, msg)    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    if not self:GetVar('bHide') and not player:CheckPrecondition{PreconditionID = 46}.bPass and not player:CheckPrecondition{PreconditionID = 47}.bPass then      
        self:SetVar('bHide', true)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "startup", self )
    end       
end

function addInventoryGroup(self, player)
    if not player then return end        
    
    player:UpdateInventoryGroup{action = "ADD", inventoryType = 2, groupID = "Duck_Build_Bricks", groupName = "Duck Build", locked = true}
    
    for k,v in pairs(duckBricks) do        
        player:UpdateInventoryGroupContents{action = "ADD", inventoryType = 2, groupID = "Duck_Build_Bricks", templateID = tostring(k)}
        player:AddItemToInventory{iObjTemplate = tostring(k), itemCount = v}
    end
    
    player:SetInventoryFilter{inventoryType = 2, filterType = "GROUP", filterData = "Duck_Build_Bricks"}
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2.5 , "toggleBackpack", self )
end

function onMissionDialogueOK(self,msg)    
    if msg.missionID == misID then
        if msg.iMissionState < 2 then
            --print('give duck bricks')
            addInventoryGroup(self, msg.responder)
        elseif msg.iMissionState == 4 then
            --print('player has ' .. msg.responder:GetInvItemCount{iObjTemplate = 6086}.itemCount .. ' thinking caps 1')
            if msg.responder:GetInvItemCount{iObjTemplate = 6086}.itemCount > 0 then
                --print('remove thinking cap 1')
                msg.responder:RemoveItemFromInventory{iObjTemplate = 6086}
            end
        end          
    end        
end 

function onFireEvent(self, msg)
    if msg.args == 'showIcon' then    
        self:SetVar('bHide', false)
        self:SetIconAboveHead{bIconOff = false, iconMode = 0, iconType = 1}
    end
end 

function onTimerDone(self, msg)
	if (msg.name == "startup") then 
        self:SetIconAboveHead{bIconOff = true, iconMode = 0, iconType = 1}	 
    elseif (msg.name == "toggleBackpack") then         
        UI:SendMessage("ToggleBackpack", {{"visible", true}, {"tabName", "my_bricks"}})
	end
end 