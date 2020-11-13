using DakarRally.Common;

namespace DakarRally.DAL
{
    public abstract class BaseRepository
    {
        public Model.DakarRallyEntities GetContext()
        {
            var connextionString = @"metadata=res://*/Model.DakarRallyModel.csdl|res://*/Model.DakarRallyModel.ssdl|res://*/Model.DakarRallyModel.msl;provider=System.Data.SqlServerCe.4.0;provider connection string='Data Source={db_path}'";
            var model = new Model.DakarRallyEntities(connextionString.Replace("{db_path}", AppSettings.DatabasePath));
            model.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            return model;
        }

    }
}