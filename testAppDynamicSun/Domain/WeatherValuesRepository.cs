using Microsoft.EntityFrameworkCore;

namespace testAppDynamicSun.Domain
{
    public class WeatherValuesRepository
    {
        //класс-репозиторий напрямую обращается к контексту базы данных
        private readonly AppDbContext context;
        public WeatherValuesRepository(AppDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// выбрать записи от даты до даты
        /// </summary>
        /// <param name="start">от какой даты</param>
        /// <param name="end">до какой даты</param>
        /// <returns>колекция подходящих WeatherValue</returns>
        public IQueryable<WeatherValue> GetIntervalWeatherValues(DateTime start, DateTime end) //выбрать записи от даты до даты
        {
            var weatherValues1 = (from weatherValue in context.WeatherValues
                                  where weatherValue.Date >=  start
                                  where weatherValue.Date <=  end
                                  select weatherValue);
            return weatherValues1.OrderBy(x => x.Date);
        }
  
        /// <summary>
        /// Проверка на наличие такой же записи в БД
        /// </summary>
        /// <param name="dateTime">Date</param>
        /// <param name="time">Time</param>
        /// <returns>true - есть такое значение в базе. false -  нет такого значения в базе</returns>
        public bool isDuplicateWeatherValue(DateTime date, string time)
        {
            var duplicateWeatherValue = (from weatherValue in context.WeatherValues
                                         where weatherValue.Date == date
                                         where weatherValue.Time == time
                                         select weatherValue);
            foreach (var item in duplicateWeatherValue)
            {
                if (item != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
            
        }
        public IQueryable<WeatherValue> GetWeatherValues() //выбрать все записи из таблицы WeatherValues
        {
            //return context.WeatherValues.OrderBy(x => x.Date);
            return context.WeatherValues.OrderBy(x => x.Date);
        }

        //найти определенную запись по id
        public WeatherValue GetWeatherValueById(Guid id)
        {
            return context.WeatherValues.Single(x => x.Id == id);
        }

        //сохранить новую либо обновить существующую запись в БД
        public Guid SaveWeatherValue(WeatherValue entity)
        {
            if (entity.Id == default)
                context.Entry(entity).State = EntityState.Added;
            else
                context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();

            return entity.Id;
        }

        //удалить существующую запись
        public void DeleteWeatherValue(WeatherValue entity)
        {
            context.WeatherValues.Remove(entity);
            context.SaveChanges();
        }
    }
}
