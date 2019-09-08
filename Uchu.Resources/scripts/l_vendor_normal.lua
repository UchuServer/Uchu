--[[require('State')
require('c_Main')
require('o_mis')

function onStartup(self) 
Ven = {}

	--/////////////////////////////////////////////////////////////////////////
	--  Enter/Exit Conduct Radius
	--/////////////////////////////////////////////////////////////////////////

    Ven['Conduct_CoolDown']  	= 1       -- Effects the Trigger time upone exiting/entering the conduct radius.
    Ven['Conduct_Delay']     	= 1        -- Delay before triggering the emote.
    Ven['Main_EmoteID']      	= 69       -- Global emote ID 
-- Conduct Radius 
    Ven['OverRideConduct']  	= true 
    Ven['conductRadius']     		= 20
   
-- Conduct -----------------------     OnEnter
    Ven['Emote_enterType']        = "enter"
-- Conduct -----------------------      OnExit_Buy or Sell
    Ven['Emote_onExitBuySell']    = "leave"  
 -- Conduct -----------------------      OnExit_Null
    Ven['Emote_onExitType']   =     "leave" 
-- Conduct -----------------------      OnExit_Sell
    Ven['Emote_onExitSellType']   = "cancel"  
  
	--///////////////////////////////////////////////////////////////////////////
	--  Vendor States
	--///////////////////////////////////////////////////////////////////////////      
	-- Emote OnBuy ---------------------    OnBuy
		Ven['Emote_onBuyType']    = "buy"  
	-- Emote OnSell ---------------------   OnSell
		Ven['Emote_onSellType']   = "sell" 
    
------ Do not change ------------------------------------------------
    self:SetVar("Ven",Ven)
    LoadVenderVarsOnce(self)
    GetVenderVars(self)
    CreateVenderStates(self)
-------------------------------------------------------------------------------

end
--]]
