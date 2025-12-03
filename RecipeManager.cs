using System;
using System.IO;
using System.Web.Script.Serialization;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 레시피 파일 관리자 - JSON 형식으로 저장/불러오기
    /// </summary>
    public static class RecipeManager
    {
        /// <summary>레시피 저장 폴더 경로</summary>
        private static readonly string RecipeFolderPath;

        /// <summary>기본 레시피 파일명</summary>
        private const string DEFAULT_RECIPE_FILE = "default_recipe.json";

        /// <summary>
        /// 정적 생성자 - 레시피 폴더 초기화
        /// </summary>
        static RecipeManager()
        {
            // 실행 파일 경로 기준으로 Recipes 폴더 설정
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            RecipeFolderPath = Path.Combine(appPath, "Recipes");

            // 폴더가 없으면 생성
            if (!Directory.Exists(RecipeFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(RecipeFolderPath);
                }
                catch (Exception)
                {
                    // 폴더 생성 실패 시 무시
                }
            }
        }

        /// <summary>
        /// 레시피를 JSON 파일로 저장
        /// </summary>
        /// <param name="recipe">저장할 레시피 세트</param>
        /// <param name="fileName">파일명 (null이면 기본 파일 사용)</param>
        /// <returns>저장 성공 여부</returns>
        public static bool SaveRecipe(RecipeSet recipe, string fileName = null)
        {
            try
            {
                string filePath = GetFilePath(fileName);
                recipe.LastModified = DateTime.Now;

                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(recipe);

                File.WriteAllText(filePath, json);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// JSON 파일에서 레시피 불러오기
        /// </summary>
        /// <param name="fileName">파일명 (null이면 기본 파일 사용)</param>
        /// <returns>불러온 레시피 세트 (실패 시 새 레시피 반환)</returns>
        public static RecipeSet LoadRecipe(string fileName = null)
        {
            try
            {
                string filePath = GetFilePath(fileName);

                if (!File.Exists(filePath))
                {
                    return new RecipeSet();
                }

                string json = File.ReadAllText(filePath);
                var serializer = new JavaScriptSerializer();
                RecipeSet recipe = serializer.Deserialize<RecipeSet>(json);

                return recipe ?? new RecipeSet();
            }
            catch (Exception)
            {
                return new RecipeSet();
            }
        }

        /// <summary>
        /// 레시피 파일 존재 여부 확인
        /// </summary>
        /// <param name="fileName">파일명 (null이면 기본 파일 확인)</param>
        /// <returns>파일 존재 여부</returns>
        public static bool RecipeExists(string fileName = null)
        {
            string filePath = GetFilePath(fileName);
            return File.Exists(filePath);
        }

        /// <summary>
        /// 레시피 파일 삭제
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>삭제 성공 여부</returns>
        public static bool DeleteRecipe(string fileName)
        {
            try
            {
                string filePath = GetFilePath(fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 저장된 모든 레시피 파일명 목록 가져오기
        /// </summary>
        /// <returns>레시피 파일명 배열</returns>
        public static string[] GetAllRecipeNames()
        {
            try
            {
                if (!Directory.Exists(RecipeFolderPath))
                    return new string[0];

                string[] files = Directory.GetFiles(RecipeFolderPath, "*.json");
                string[] names = new string[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    names[i] = Path.GetFileNameWithoutExtension(files[i]);
                }

                return names;
            }
            catch
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 파일 경로 생성
        /// </summary>
        private static string GetFilePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Path.Combine(RecipeFolderPath, DEFAULT_RECIPE_FILE);
            }

            // 확장자가 없으면 .json 추가
            if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            return Path.Combine(RecipeFolderPath, fileName);
        }

        /// <summary>
        /// 레시피 폴더 경로 가져오기
        /// </summary>
        public static string GetRecipeFolderPath()
        {
            return RecipeFolderPath;
        }
    }
}
