
-- Load Once
function LoadVenderVarsOnce(self)


end


-- Saved Vars
function  GetVenderVars(self)
    self:SetVar("ConductCoolDown", false) 
    self:SetVar("ConductTimer_Started",false)
    self:SetVar("Emote_onExitBuyActive",false)
    
    self:SetVar("Vendor_BuyCount", 0) 
    self:SetVar("Vendor_SellCount", 0)
end

function CreateVenderStates(self)

    if self:GetVar('Ven.OverRideConduct') then
        self:SetProximityRadius { radius = self:GetVar("Ven.conductRadius") , name = "conductRadius" }
    end 


    self:UseStateMachine{} 
    -- Idle State
    VenderIdle = State.create()
    VenderIdle.onEnter = function(self)
     --
    
    end 
    VenderIdle.onArrived = function(self)

    end  
    -- Emote State
    VenderEmote = State.create()
    VenderEmote.onEnter = function(self)
        
       -- self:FaceTarget{ target = myTarget, degreesOff = 5, keepFacingTarget = true }
   --     Emote.emote(self, getMyVendorTarget(self), self:GetVar("Ven.Emote_enterType") ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "BreakPlayEmote", self )
       
    end
    VenderEmote.onArrived = function(self)
     
    end
    
    addState(VenderIdle, "VenderIdle", "VenderIdle", self)
    addState(VenderEmote, "VenderEmote", "VenderEmote", self)
    beginStateMachine("VenderIdle", self) 
    VenderIdle.onEnter(self)    
    
end

--[[ function onProximityUpdate(self, msg)
 
           
    if msg.objType == "Enemies" or msg.objType == "NPC" then
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////

        if  msg.name == "conductRadius" and msg.status == "ENTER" then
      
                storeVendorTarget(self, msg.objId)
                SetVendorEmote(self, self:GetVar("Ven.Emote_enterType"))
                setState("VenderEmote",self)
              
            end
        end
        if  msg.name == "conductRadius" and msg.status == "LEAVE" and not self:GetVar("ExitConductTimer_Started")then
        
                 storeVendorTarget(self, msg.objId)
                if self:GetVar("Vendor_SellCount") ~= 0 or  self:GetVar("Vendor_BuyCount") ~= 0 then
                    SetVendorEmote(self, self:GetVar("Ven.Emote_onExitBuySell"))
                    setState("VenderEmote",self)
                else
                    SetVendorEmote(self, self:GetVar("Ven.Emote_onExitType"))
                    setState("VenderEmote",self)
                end
                     self:SetVar("Vendor_BuyCount",0)
                     self:SetVar("Vendor_SellCount",0)
            
        end
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////  

    
end

onTimerDone = function(self, msg)

    if msg.name == "BreakPlayEmote" then
     setState("VenderIdle",self)
    end

end


--------  Vendor Buy
onBuyFromVendor = function(self, msg)
  
      self:SetVar("Vendor_BuyCount", msg.count)
      Emote.emote(self, target, self:GetVar("Ven.Emote_onBuyType") ) 
end

-------  Vendor Sell
onSellToVendor = function(self, msg)
 
     Emote.emote(self, target, self:GetVar("Ven.Emote_onSellType") ) 
     self:SetVar("Vendor_SellCount", msg.count)

end

------- Vendor Exit
onTerminateInteraction = function(self, msg) 

    
end 

-------------------------- MIS

function SetVendorEmote(self, type)
    self:SetVar("EmoteType", type)
end

function storeVendorTarget(self, target)
    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("myVendorTarget", finalID)
end

function getMyVendorTarget(self)
    targetID = self:GetVar("myVendorTarget")
    return GAMEOBJ:GetObjectByID(targetID)
end

function emote(self,target, skillType)
       
        self:SetVar("EmbeddedTime", self:GetAnimationTime{  animationID = "interact" }.time)
        self:PlayFXEffect {priority = 1.2, effectType = skillType}
end

--]]
