-- ***********************************************************

-- HF config data format
-- imageDir -> 0:directory (relative to client/res/textures)
-- ***********************************************************
 TVfileCache = {}; -- This is global to all objects running this script.  Normally this is forbidden, but I actually want this global
  
 --function onStartup()
 --end

-- Load up the first image when the render component is ready 
 function onRenderComponentReady(self, msg)
    SetNextImage(self);
    GAMEOBJ:GetTimer():AddTimerWithCancel( math.random(30, 45), "LUPTVTimer",self );
 end
 
 function TVInitDirectory(dir)
    TVfileCache[dir] = RESMGR:GetFilesInDir(dir, "dds", true, "");
    -- call like this RESMGR:GetFilesInDir(string directory, string extension (blank for all extensions), bool check sub directories, string wildcard);
 end
 

-- Swap images and reset the timer
 function onTimerDone(self, msg)
    if (msg.name == "LUPTVTimer") then
        SetNextImage(self);
        GAMEOBJ:GetTimer():AddTimerWithCancel( math.random(30, 45), "LUPTVTimer",self );
    end
    
 end
 
 
 -- You can set which directory to load from.  By default it is textures/lup/ and all subdirectories
 function TVGetDirectory(self)
    local imageDir = self:GetVar("imageDir");
    if(imageDir == nil) then
        imageDir = "textures/lup";
    end
    
    local temp = TVGetDirFromCache(imageDir);
    return temp;
 end
 
 
 -- Get the directory from the cache.
 -- If that directory hasn't been processed yet, then init that directory and then return
 function TVGetDirFromCache(imageDir)
    local dir = TVfileCache[imageDir];
    if(dir == nil) then
        TVInitDirectory(imageDir);
        dir = TVfileCache[imageDir];
    end
    return dir;
 end
 
 
 -- Load up the next image at random
 function SetNextImage(self)
    local imageDir = TVGetDirectory(self);
    local numFiles = #imageDir;
    if(numFiles >= 1) then
        local lastIdx = self:GetVar("fileIdx");
        local newIdx = math.random(1, numFiles);
        
        -- make sure we're not loading the same image twice
        if(newIdx == lastIdx) then
            newIdx = newIdx + 1;
            if(newIdx > numFiles) then
                newIdx = 1;
            end
        end
        self:SetVar("fileIdx", newIdx);
        UI:SendMessage( "SetLUPTVImage", {{"ImageName", imageDir[newIdx]}}, self );
    end
 end
 