var FlushIDBFS = {
FlushIDBFS : function()
{
FS.syncfs(false, function (err) {});
},
};
mergeInto(LibraryManager.library, FlushIDBFS);